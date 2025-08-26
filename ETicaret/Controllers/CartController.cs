using ETicaret.Services;
using ETicaret.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Http;

namespace ETicaret.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private string GetSessionId()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("CartSessionId")))
            {
                var sessionId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("CartSessionId", sessionId);
            }
            return HttpContext.Session.GetString("CartSessionId")!;
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
        }

        public async Task<IActionResult> Index()
        {
            var sessionId = GetSessionId();
            var userId = GetCurrentUserId();
            var cart = await _cartService.GetCartViewModelAsync(sessionId, userId);
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Geçersiz veri." });
            }

            try
            {
                var sessionId = GetSessionId();
                var userId = GetCurrentUserId();
                await _cartService.AddToCartAsync(model.ProductId, model.Quantity, sessionId, userId);

                return Json(new { success = true, message = "Ürün sepete eklendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(UpdateCartItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Geçersiz veri." });
            }

            var success = await _cartService.UpdateCartItemAsync(model.CartItemId, model.Quantity);
            
            if (success)
            {
                var sessionId = GetSessionId();
                var userId = GetCurrentUserId();
                var cartSummary = await _cartService.GetCartSummaryAsync(sessionId, userId);
                
                return Json(new { 
                    success = true, 
                    totalPrice = cartSummary.TotalPrice.ToString("F2") + " ₺",
                    totalItems = cartSummary.TotalItems
                });
            }

            return Json(new { success = false, message = "Güncelleme başarısız." });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            var success = await _cartService.RemoveFromCartAsync(cartItemId);
            
            if (success)
            {
                var sessionId = GetSessionId();
                var userId = GetCurrentUserId();
                var cartSummary = await _cartService.GetCartSummaryAsync(sessionId, userId);
                
                return Json(new { 
                    success = true, 
                    totalPrice = cartSummary.TotalPrice.ToString("F2") + " ₺",
                    totalItems = cartSummary.TotalItems,
                    isEmpty = cartSummary.IsEmpty
                });
            }

            return Json(new { success = false, message = "Silme işlemi başarısız." });
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var sessionId = GetSessionId();
                var userId = GetCurrentUserId();
                var success = await _cartService.ClearCartAsync(sessionId, userId);
                
                if (success)
                {
                    return Json(new { success = true, message = "Sepet temizlendi." });
                }
                else
                {
                    return Json(new { success = false, message = "Sepet temizlenemedi." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCartSummary()
        {
            var sessionId = GetSessionId();
            var userId = GetCurrentUserId();
            var cartSummary = await _cartService.GetCartSummaryAsync(sessionId, userId);
            
            return Json(new { 
                totalItems = cartSummary.TotalItems,
                totalPrice = cartSummary.TotalPrice.ToString("C2"),
                isEmpty = cartSummary.IsEmpty
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var sessionId = GetSessionId();
            var userId = GetCurrentUserId();
            var count = await _cartService.GetCartItemCountAsync(sessionId, userId);
            
            return Json(new { count });
        }

    }
}
