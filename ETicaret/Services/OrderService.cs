using ETicaret.Core.Entities;
using ETicaret.Core.Models;
using ETicaret.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ETicaret.Services
{
    public class OrderService : IOrderService
    {
        private readonly DatabaseContext _context;
        private readonly ICartService _cartService;

        public OrderService(DatabaseContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        public async Task<CheckoutViewModel> GetCheckoutViewModelAsync(int userId)
        {
            var cartItems = await _cartService.GetUserCartItemsAsync(userId);
            var addresses = await _context.Addresses
                .Where(a => a.AppUserId == userId && a.IsActive)
                .Select(a => new AddressViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    FullName = a.FullName,
                    Phone = a.Phone,
                    City = a.City,
                    District = a.District,
                    FullAddress = a.FullAddress
                })
                .ToListAsync();

            var cartItemViewModels = cartItems.Select(ci => new CartItemViewModel
            {
                Id = ci.Id,
                ProductId = ci.ProductId,
                ProductName = ci.Product.Name,
                ProductImage = ci.Product.ImageUrl,
                Price = ci.Price,
                Quantity = ci.Quantity
            }).ToList();

            var totalAmount = cartItemViewModels.Sum(x => x.Price * x.Quantity);
            var shippingCost = await CalculateShippingCostAsync(totalAmount);

            return new CheckoutViewModel
            {
                CartItems = cartItemViewModels,
                Addresses = addresses,
                TotalAmount = totalAmount,
                ShippingCost = shippingCost
            };
        }

        public async Task<Order> CreateOrderAsync(CheckoutViewModel model, int userId)
        {

            if (!await ValidateOrderAsync(model, userId))
            {
                throw new InvalidOperationException("Sipariş bilgileri geçersiz.");
            }


            var cartItems = await _cartService.GetUserCartItemsAsync(userId);
            if (!cartItems.Any())
            {
                throw new InvalidOperationException("Sepetinizde ürün bulunmamaktadır.");
            }


            var calculatedTotalAmount = cartItems.Sum(ci => ci.Price * ci.Quantity);
            var calculatedShippingCost = await CalculateShippingCostAsync(calculatedTotalAmount);


            var order = new Order
            {
                OrderNumber = await GenerateOrderNumberAsync(),
                AppUserId = userId,
                AddressId = model.SelectedAddressId,
                OrderStatus = OrderStatus.Pending,
                PaymentMethod = model.PaymentMethod,
                TotalAmount = calculatedTotalAmount,
                ShippingCost = calculatedShippingCost,
                Notes = model.Notes
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();


            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Price
                };
                _context.OrderItems.Add(orderItem);
            }


            await _cartService.ClearUserCartAsync(userId);

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<OrderViewModel> GetOrderByIdAsync(int orderId, int userId)
        {
            var order = await _context.Orders
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.AppUserId == userId);

            if (order == null)
            {
                throw new ArgumentException("Sipariş bulunamadı.");
            }

            return new OrderViewModel
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderStatus = order.OrderStatus,
                PaymentMethod = order.PaymentMethod,
                TotalAmount = order.TotalAmount,
                ShippingCost = order.ShippingCost,

                CreateDate = order.CreateDate,
                Notes = order.Notes,
                Address = new AddressViewModel
                {
                    Id = order.Address.Id,
                    Title = order.Address.Title,
                    FullName = order.Address.FullName,
                    Phone = order.Address.Phone,
                    City = order.Address.City,
                    District = order.Address.District,
                    FullAddress = order.Address.FullAddress
                },
                OrderItems = order.OrderItems.Select(oi => new OrderItemViewModel
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    ProductImage = oi.Product.ImageUrl,
                    Price = oi.Price,
                    Quantity = oi.Quantity
                }).ToList()
            };
        }

        public async Task<List<OrderViewModel>> GetUserOrdersAsync(int userId)
        {
            var orders = await _context.Orders
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.AppUserId == userId && o.IsActive)
                .OrderByDescending(o => o.CreateDate)
                .ToListAsync();

            return orders.Select(order => new OrderViewModel
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderStatus = order.OrderStatus,
                PaymentMethod = order.PaymentMethod,
                TotalAmount = order.TotalAmount,
                ShippingCost = order.ShippingCost,

                CreateDate = order.CreateDate,
                Notes = order.Notes,
                Address = new AddressViewModel
                {
                    Id = order.Address.Id,
                    Title = order.Address.Title,
                    FullName = order.Address.FullName,
                    Phone = order.Address.Phone,
                    City = order.Address.City,
                    District = order.Address.District,
                    FullAddress = order.Address.FullAddress
                },
                OrderItems = order.OrderItems.Select(oi => new OrderItemViewModel
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    ProductImage = oi.Product.ImageUrl,
                    Price = oi.Price,
                    Quantity = oi.Quantity
                }).ToList()
            }).ToList();
        }

        public async Task<OrderListViewModel> GetUserOrderListAsync(int userId)
        {
            var orders = await GetUserOrdersAsync(userId);
            
            return new OrderListViewModel
            {
                Orders = orders,
                TotalOrders = orders.Count,
                PendingOrders = orders.Count(o => o.OrderStatus == OrderStatus.Pending),
                ConfirmedOrders = orders.Count(o => o.OrderStatus == OrderStatus.Confirmed),
                ShippedOrders = orders.Count(o => o.OrderStatus == OrderStatus.Shipped),
                DeliveredOrders = orders.Count(o => o.OrderStatus == OrderStatus.Delivered)
            };
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;

            order.OrderStatus = status;
            order.UpdateDate = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> GenerateOrderNumberAsync()
        {
            var today = DateTime.Now;
            var prefix = $"ORD{today:yyyyMMdd}";
            
            var lastOrder = await _context.Orders
                .Where(o => o.OrderNumber.StartsWith(prefix))
                .OrderByDescending(o => o.OrderNumber)
                .FirstOrDefaultAsync();

            int sequence = 1;
            if (lastOrder != null)
            {
                var lastSequence = lastOrder.OrderNumber.Substring(prefix.Length);
                if (int.TryParse(lastSequence, out int lastSeq))
                {
                    sequence = lastSeq + 1;
                }
            }

            return $"{prefix}{sequence:D4}";
        }

        public async Task<bool> CancelOrderAsync(int orderId, int userId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.AppUserId == userId);

            if (order == null || order.OrderStatus != OrderStatus.Pending)
            {
                return false;
            }

            order.OrderStatus = OrderStatus.Cancelled;
            order.UpdateDate = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> CalculateShippingCostAsync(decimal totalAmount)
        {

            if (totalAmount >= 500)
            {
                return 0;
            }
            

            return 25;
        }

        public async Task<bool> ValidateOrderAsync(CheckoutViewModel model, int userId)
        {

            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == model.SelectedAddressId && a.AppUserId == userId && a.IsActive);
            
            if (address == null)
            {
                return false;
            }


            var cartItems = await _cartService.GetUserCartItemsAsync(userId);
            if (!cartItems.Any())
            {
                return false;
            }


            foreach (var cartItem in cartItems)
            {
                var product = await _context.Products.FindAsync(cartItem.ProductId);
                if (product == null || product.Stock < cartItem.Quantity)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<List<OrderViewModel>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.AppUser)
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.IsActive)
                .OrderByDescending(o => o.CreateDate)
                .ToListAsync();

            return orders.Select(order => new OrderViewModel
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderStatus = order.OrderStatus,
                PaymentMethod = order.PaymentMethod,
                TotalAmount = order.TotalAmount,
                ShippingCost = order.ShippingCost,

                CreateDate = order.CreateDate,
                Notes = order.Notes,
                Address = new AddressViewModel
                {
                    Id = order.Address.Id,
                    Title = order.Address.Title,
                    FullName = order.Address.FullName,
                    Phone = order.Address.Phone,
                    City = order.Address.City,
                    District = order.Address.District,
                    FullAddress = order.Address.FullAddress
                },
                OrderItems = order.OrderItems.Select(oi => new OrderItemViewModel
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    ProductImage = oi.Product.ImageUrl,
                    Price = oi.Price,
                    Quantity = oi.Quantity
                }).ToList()
            }).ToList();
        }

        public async Task<OrderViewModel> GetOrderByIdForAdminAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.AppUser)
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                throw new ArgumentException("Sipariş bulunamadı.");
            }

            return new OrderViewModel
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderStatus = order.OrderStatus,
                PaymentMethod = order.PaymentMethod,
                TotalAmount = order.TotalAmount,
                ShippingCost = order.ShippingCost,

                CreateDate = order.CreateDate,
                Notes = order.Notes,
                Address = new AddressViewModel
                {
                    Id = order.Address.Id,
                    Title = order.Address.Title,
                    FullName = order.Address.FullName,
                    Phone = order.Address.Phone,
                    City = order.Address.City,
                    District = order.Address.District,
                    FullAddress = order.Address.FullAddress
                },
                OrderItems = order.OrderItems.Select(oi => new OrderItemViewModel
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    ProductImage = oi.Product.ImageUrl,
                    Price = oi.Price,
                    Quantity = oi.Quantity
                }).ToList()
            };
        }
    }
}
