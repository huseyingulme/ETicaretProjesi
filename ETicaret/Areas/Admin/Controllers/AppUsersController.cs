using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ETicaret.Core.Entities;
using ETicaret.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text;

namespace ETicaret.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AppUsersController : Controller
    {
        private readonly DatabaseContext _context;

        public AppUsersController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Admin/AppUsers
        public async Task<IActionResult> Index()
        {
            return View(await _context.AppUsers.ToListAsync());
        }

        // GET: Admin/AppUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUser = await _context.AppUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        // GET: Admin/AppUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AppUsers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                // Şifreyi hash'le
                appUser.Password = HashPassword(appUser.Password);
                appUser.CreateDate = DateTime.UtcNow;
                appUser.UserGuid = Guid.NewGuid();
                appUser.IsDeletable = true;
                
                _context.Add(appUser);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Kullanıcı başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            return View(appUser);
        }

        // GET: Admin/AppUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUser = await _context.AppUsers.FindAsync(id);
            if (appUser == null)
            {
                return NotFound();
            }
            return View(appUser);
        }

        // POST: Admin/AppUsers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppUser appUser)
        {
            if (id != appUser.Id)
            {
                return NotFound();
            }

            // Kullanıcının kendisini admin yetkisinden çıkarmasını engelle
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (id == currentUserId && !appUser.IsAdmin)
            {
                ModelState.AddModelError("IsAdmin", "Kendi admin yetkinizi kaldıramazsınız!");
                return View(appUser);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Mevcut kullanıcıyı veritabanından al
                    var existingUser = await _context.AppUsers.FindAsync(id);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    // Şifre değiştirilmişse hash'le
                    if (!string.IsNullOrEmpty(appUser.Password))
                    {
                        existingUser.Password = HashPassword(appUser.Password);
                    }

                    // Diğer alanları güncelle
                    existingUser.Name = appUser.Name;
                    existingUser.SurName = appUser.SurName;
                    existingUser.Email = appUser.Email;
                    existingUser.Phone = appUser.Phone;
                    existingUser.UserName = appUser.UserName;
                    existingUser.IsActive = appUser.IsActive;
                    existingUser.IsAdmin = appUser.IsAdmin;

                    _context.Update(existingUser);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Kullanıcı başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppUserExists(appUser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(appUser);
        }

        // GET: Admin/AppUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Kullanıcının kendisini silmesini engelle
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (id == currentUserId)
            {
                TempData["ErrorMessage"] = "Kendi hesabınızı silemezsiniz!";
                return RedirectToAction(nameof(Index));
            }

            var appUser = await _context.AppUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appUser == null)
            {
                return NotFound();
            }

            // Silinemeyen kullanıcıları kontrol et
            if (!appUser.IsDeletable)
            {
                TempData["ErrorMessage"] = "Bu kullanıcı silinemez!";
                return RedirectToAction(nameof(Index));
            }

            return View(appUser);
        }

        // POST: Admin/AppUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Kullanıcının kendisini silmesini engelle
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (id == currentUserId)
            {
                TempData["ErrorMessage"] = "Kendi hesabınızı silemezsiniz!";
                return RedirectToAction(nameof(Index));
            }

            var appUser = await _context.AppUsers.FindAsync(id);
            if (appUser != null)
            {
                // Silinemeyen kullanıcıları kontrol et
                if (!appUser.IsDeletable)
                {
                    TempData["ErrorMessage"] = "Bu kullanıcı silinemez!";
                    return RedirectToAction(nameof(Index));
                }

                _context.AppUsers.Remove(appUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/AppUsers/CreateAdminUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdminUser()
        {
            try
            {
                // Admin kullanıcısı zaten var mı kontrol et
                var existingAdmin = await _context.AppUsers
                    .FirstOrDefaultAsync(u => u.Email == "admin@eticaret.com");

                if (existingAdmin != null)
                {
                    TempData["ErrorMessage"] = "Admin kullanıcısı zaten mevcut!";
                    return RedirectToAction(nameof(Index));
                }

                // Yeni admin kullanıcısı oluştur
                var adminUser = new AppUser
                {
                    Name = "Admin",
                    SurName = "User",
                    Email = "admin@eticaret.com",
                    Password = HashPassword("123456"),
                    UserName = "admin",
                    IsActive = true,
                    IsAdmin = true,
                    IsDeletable = false, // Admin kullanıcısı silinemez
                    CreateDate = DateTime.UtcNow,
                    UserGuid = Guid.NewGuid()
                };

                _context.AppUsers.Add(adminUser);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Admin kullanıcısı başarıyla oluşturuldu. E-posta: admin@eticaret.com, Şifre: 123456";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Admin kullanıcısı oluşturulurken hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Admin/AppUsers/ToggleAdminStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAdminStatus(int id)
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (id == currentUserId)
                {
                    TempData["ErrorMessage"] = "Kendi admin yetkinizi kaldıramazsınız!";
                    return RedirectToAction(nameof(Index));
                }

                var appUser = await _context.AppUsers.FindAsync(id);
                if (appUser == null)
                {
                    return NotFound();
                }

                // Admin yetkisini değiştir
                appUser.IsAdmin = !appUser.IsAdmin;
                await _context.SaveChangesAsync();

                var status = appUser.IsAdmin ? "verildi" : "kaldırıldı";
                TempData["SuccessMessage"] = $"Kullanıcıya admin yetkisi {status}.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Admin yetkisi değiştirilirken hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Admin/AppUsers/DeleteAdminUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAdminUser()
        {
            try
            {
                // Admin kullanıcısını bul
                var adminUser = await _context.AppUsers
                    .FirstOrDefaultAsync(u => u.Email == "admin@eticaret.com");

                if (adminUser == null)
                {
                    TempData["ErrorMessage"] = "Admin kullanıcısı bulunamadı!";
                    return RedirectToAction(nameof(Index));
                }

                // Admin kullanıcısını sil
                _context.AppUsers.Remove(adminUser);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Admin kullanıcısı başarıyla silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Admin kullanıcısı silinirken hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool AppUserExists(int id)
        {
            return _context.AppUsers.Any(e => e.Id == id);
        }

        // Helper method for password hashing
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
