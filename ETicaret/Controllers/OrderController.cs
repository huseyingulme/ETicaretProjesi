using ETicaret.Services;
using ETicaret.Core.Models;
using ETicaret.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ETicaret.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;

        public OrderController(IOrderService orderService, ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("Kullanıcı giriş yapmamış.");
            }
            return int.Parse(userIdClaim.Value);
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Ödeme yapmak için giriş yapmanız gerekmektedir.";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var userId = GetCurrentUserId();
                var model = await _orderService.GetCheckoutViewModelAsync(userId);
                
                if (!model.CartItems.Any())
                {
                    TempData["ErrorMessage"] = "Sepetinizde ürün bulunmamaktadır.";
                    return RedirectToAction("Index", "Cart");
                }

                if (!model.Addresses.Any())
                {
                    TempData["ErrorMessage"] = "Ödeme yapmak için önce bir adres eklemeniz gerekmektedir.";
                    return RedirectToAction("Index", "Address");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Sipariş sayfası yüklenirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("Index", "Cart");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Ödeme yapmak için giriş yapmanız gerekmektedir.";
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    var userId = GetCurrentUserId();
                    var updatedModel = await _orderService.GetCheckoutViewModelAsync(userId);
                    model.CartItems = updatedModel.CartItems;
                    model.Addresses = updatedModel.Addresses;
                    model.TotalAmount = updatedModel.TotalAmount;
                    model.ShippingCost = updatedModel.ShippingCost;
                    return View(model);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                    return RedirectToAction("Index", "Cart");
                }
            }

            try
            {
                var userId = GetCurrentUserId();
                var order = await _orderService.CreateOrderAsync(model, userId);
                
                TempData["SuccessMessage"] = $"Siparişiniz başarıyla oluşturuldu. Sipariş numaranız: {order.OrderNumber}";
                return RedirectToAction("Details", new { id = order.Id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                
                try
                {
                    var userId = GetCurrentUserId();
                    var updatedModel = await _orderService.GetCheckoutViewModelAsync(userId);
                    model.CartItems = updatedModel.CartItems;
                    model.Addresses = updatedModel.Addresses;
                    model.TotalAmount = updatedModel.TotalAmount;
                    model.ShippingCost = updatedModel.ShippingCost;
                    return View(model);
                }
                catch
                {
                    return RedirectToAction("Index", "Cart");
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Siparişlerinizi görüntülemek için giriş yapmanız gerekmektedir.";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var userId = GetCurrentUserId();
                var model = await _orderService.GetUserOrderListAsync(userId);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Sipariş detaylarını görüntülemek için giriş yapmanız gerekmektedir.";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var userId = GetCurrentUserId();
                var model = await _orderService.GetOrderByIdAsync(id, userId);
                return View(model);
            }
            catch (ArgumentException)
            {
                TempData["ErrorMessage"] = "Sipariş bulunamadı.";
                return RedirectToAction("MyOrders");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("MyOrders");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "Giriş yapmanız gerekmektedir." });
            }

            try
            {
                var userId = GetCurrentUserId();
                var success = await _orderService.CancelOrderAsync(id, userId);
                
                if (success)
                {
                    return Json(new { success = true, message = "Sipariş iptal edildi." });
                }
                else
                {
                    return Json(new { success = false, message = "Sipariş iptal edilemedi. Sadece beklemede olan siparişler iptal edilebilir." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderStatus(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "Giriş yapmanız gerekmektedir." });
            }

            try
            {
                var userId = GetCurrentUserId();
                var order = await _orderService.GetOrderByIdAsync(id, userId);
                
                return Json(new { 
                    success = true, 
                    status = order.OrderStatus.ToString(),
                    statusText = GetOrderStatusText(order.OrderStatus)
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private string GetOrderStatusText(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "Beklemede",
                OrderStatus.Confirmed => "Onaylandı",
                OrderStatus.Preparing => "Hazırlanıyor",
                OrderStatus.Shipped => "Kargoya Verildi",
                OrderStatus.Delivered => "Teslim Edildi",
                OrderStatus.Cancelled => "İptal Edildi",
                OrderStatus.Returned => "İade Edildi",
                _ => "Bilinmiyor"
            };
        }
    }
}
