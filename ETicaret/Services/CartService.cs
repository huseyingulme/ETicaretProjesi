using ETicaret.Core.Entities;
using ETicaret.Data;
using ETicaret.Models;
using Microsoft.EntityFrameworkCore;

namespace ETicaret.Services
{
    public class CartService : ICartService
    {
        private readonly DatabaseContext _context;

        public CartService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetOrCreateCartAsync(string sessionId, int? userId = null)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId && c.IsActive);

            if (cart == null)
            {
                cart = new Cart
                {
                    SessionId = sessionId,
                    UserId = userId,
                    CreateDate = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        public async Task<CartItem> AddToCartAsync(int productId, int quantity, string sessionId, int? userId = null)
        {
            var cart = await GetOrCreateCartAsync(sessionId, userId);
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
                throw new ArgumentException("Ürün bulunamadı.");

            if (product.Stock < quantity)
                throw new InvalidOperationException("Yetersiz stok.");

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.UpdateDate = DateTime.UtcNow;
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.Price,
                    CreateDate = DateTime.UtcNow,
                    IsActive = true
                };

                cart.CartItems.Add(cartItem);
            }

            cart.UpdateDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return existingItem ?? cart.CartItems.Last();
        }

        public async Task<bool> UpdateCartItemAsync(int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.IsActive);

            if (cartItem == null)
                return false;

            if (cartItem.Product.Stock < quantity)
                return false;

            cartItem.Quantity = quantity;
            cartItem.UpdateDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromCartAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.IsActive);

            if (cartItem == null)
                return false;


            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCartAsync(string sessionId, int? userId = null)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId && c.IsActive);

            if (cart == null)
                return false;


            _context.CartItems.RemoveRange(cart.CartItems);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CartViewModel> GetCartViewModelAsync(string sessionId, int? userId = null)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId && c.IsActive);

            if (cart == null)
                return new CartViewModel { SessionId = sessionId };

            var cartViewModel = new CartViewModel
            {
                Id = cart.Id,
                SessionId = cart.SessionId,
                UserId = cart.UserId,
                Items = cart.CartItems
                    .Where(ci => ci.Product != null)
                    .Select(ci => new CartItemViewModel
                    {
                        Id = ci.Id,
                        ProductId = ci.ProductId,
                        ProductName = ci.Product?.Name ?? "Ürün Bulunamadı",
                        ProductImage = ci.Product?.Image ?? "default.jpg",
                        ProductCode = ci.Product?.ProductCode ?? "",
                        Quantity = ci.Quantity,
                        Price = ci.Price,
                        Stock = ci.Product?.Stock ?? 0
                    }).ToList()
            };

            return cartViewModel;
        }

        public async Task<CartSummaryViewModel> GetCartSummaryAsync(string sessionId, int? userId = null)
        {
            var cart = await GetCartViewModelAsync(sessionId, userId);
            
            return new CartSummaryViewModel
            {
                TotalItems = cart.TotalItems,
                TotalPrice = cart.TotalPrice,
                IsEmpty = cart.IsEmpty
            };
        }

        public async Task<bool> TransferCartToUserAsync(string sessionId, int userId)
        {
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.SessionId == sessionId && c.IsActive);

            if (cart == null)
                return false;

            cart.UserId = userId;
            cart.UpdateDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetCartItemCountAsync(string sessionId, int? userId = null)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId && c.IsActive);

            return cart?.CartItems.Count ?? 0;
        }


        public async Task<List<CartItem>> GetUserCartItemsAsync(int userId)
        {
            var cartItems = await _context.CartItems
                .Include(ci => ci.Product)
                .Include(ci => ci.Cart)
                .Where(ci => ci.Cart.UserId == userId && ci.Cart.IsActive && ci.IsActive)
                .ToListAsync();

            return cartItems;
        }

        public async Task<bool> ClearUserCartAsync(int userId)
        {
            var cartItems = await _context.CartItems
                .Include(ci => ci.Cart)
                .Where(ci => ci.Cart.UserId == userId && ci.Cart.IsActive && ci.IsActive)
                .ToListAsync();

            if (!cartItems.Any())
                return false;


            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
