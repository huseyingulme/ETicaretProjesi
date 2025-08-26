using ETicaret.Core.Entities;
using ETicaret.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ETicaret.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly DatabaseContext _context;
        
        public FavoritesController(DatabaseContext context)
        {
            _context = context;
        }
        
        // Favori ekle
        [HttpPost]
        public async Task<IActionResult> Add(int productId)
        {
            try
            {
                // Kullanıcı ID'sini al
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Giriş yapmalısınız" });
                }

                // Ürün var mı kontrol et
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Ürün bulunamadı" });
                }

                // Zaten favorilerde mi kontrol et
                var existing = await _context.Favorites
                    .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);
                
                if (existing != null)
                {
                    // Eğer favori pasif ise aktif yap
                    if (!existing.IsActive)
                    {
                        existing.IsActive = true;
                        existing.CreateDate = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                        return Json(new { success = true, message = "Favori aktif hale getirildi" });
                    }
                    
                    return Json(new { success = false, message = "Zaten favorilerde" });
                }

                // Yeni favori ekle - IsActive değerini açıkça belirt
                var favorite = new Favorite
                {
                    UserId = userId,
                    ProductId = productId,
                    CreateDate = DateTime.UtcNow,
                    IsActive = true // Açıkça true olarak ayarla
                };

                _context.Favorites.Add(favorite);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Favorilere eklendi" });
            }
            catch (Exception ex)
            {
                // Hata loglama
                Console.WriteLine($"Favori ekleme hatası: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                
                // Kullanıcıya genel hata mesajı ver
                return Json(new { success = false, message = "Favori eklenirken bir hata oluştu. Lütfen tekrar deneyin." });
            }
        }

        // Favori kaldır
        [HttpPost]
        public async Task<IActionResult> Remove(int productId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Giriş yapmalısınız" });
                }

                var favorite = await _context.Favorites
                    .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);
                
                if (favorite != null)
                {
                    // Favoriyi tamamen silmek yerine pasif yap
                    favorite.IsActive = false;
                    await _context.SaveChangesAsync();
                }

                return Json(new { success = true, message = "Favorilerden kaldırıldı" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Favori kaldırma hatası: {ex.Message}");
                return Json(new { success = false, message = "Favori kaldırılırken bir hata oluştu. Lütfen tekrar deneyin." });
            }
        }

        // Favori sayısı
        [HttpGet]
        public async Task<IActionResult> Count()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { count = 0 });
                }

                var count = await _context.Favorites
                    .CountAsync(f => f.UserId == userId && f.IsActive);
                
                return Json(new { count = count });
            }
            catch
            {
                return Json(new { count = 0 });
            }
        }

        // Favori durumu
        [HttpGet]
        public async Task<IActionResult> IsFavorite(int productId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { isFavorite = false });
                }

                var isFavorite = await _context.Favorites
                    .AnyAsync(f => f.UserId == userId && f.ProductId == productId && f.IsActive);
                
                return Json(new { isFavorite = isFavorite });
            }
            catch
            {
                return Json(new { isFavorite = false });
            }
        }

        // Favoriler sayfası
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var favorites = await _context.Favorites
                .Where(f => f.UserId == userId && f.IsActive)
                .Include(f => f.Product)
                .ThenInclude(p => p.Category)
                .Include(f => f.Product)
                .ThenInclude(p => p.Brand)
                .OrderByDescending(f => f.CreateDate)
                .ToListAsync();

            var products = favorites.Select(f => f.Product).ToList();
            return View(products);
        }
    }
}
