using ETicaret.Core.Entities;
using ETicaret.Data;
using ETicaret.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ETicaret.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatabaseContext _context;

        public AccountController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Account/Login
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password, bool RememberMe = false, string ReturnUrl = null)
        {
            ViewData["ReturnUrl"] = ReturnUrl;

            // Debug için log ekle
            Console.WriteLine($"Login attempt - Email: {Email}, Password: {Password}, RememberMe: {RememberMe}");
            
            // ModelState validation'ı atla, manuel kontrol yap
            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password))
            {
                var user = await _context.AppUsers
                    .FirstOrDefaultAsync(u => u.Email == Email);

                Console.WriteLine($"User found: {user != null}");
                if (user != null)
                {
                    Console.WriteLine($"User ID: {user.Id}, IsAdmin: {user.IsAdmin}, IsActive: {user.IsActive}");
                }

                if (user != null && VerifyPassword(Password, user.Password))
                {
                    Console.WriteLine("Password verified successfully");
                    
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = RememberMe,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    // Session ve Cookie'ye kullanıcı ID'sini kaydet
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    if (RememberMe)
                    {
                        Response.Cookies.Append("UserId", user.Id.ToString(), new CookieOptions
                        {
                            Expires = DateTime.UtcNow.AddDays(30),
                            HttpOnly = true,
                            IsEssential = true
                        });
                    }

                    Console.WriteLine("User signed in successfully");

                    // Admin kullanıcıları admin paneline yönlendir
                    if (user.IsAdmin)
                    {
                        return RedirectToAction("Index", "Main", new { area = "Admin" });
                    }

                    // Normal kullanıcılar için ReturnUrl kontrolü
                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }

                    // Varsayılan olarak ana sayfaya yönlendir
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    Console.WriteLine("Password verification failed");
                }

                ModelState.AddModelError(string.Empty, "Geçersiz e-posta veya şifre.");
            }
            else
            {
                Console.WriteLine("ModelState validation failed:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }
            }

            return View();
        }


        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // E-posta adresi zaten kullanılıyor mu kontrol et
                if (await _context.AppUsers.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Bu e-posta adresi zaten kullanılıyor.");
                    return View(model);
                }

                var user = new AppUser
                {
                    Name = model.Name,
                    SurName = model.Surname,
                    Email = model.Email,
                    Password = HashPassword(model.Password),
                    IsActive = true,
                    IsAdmin = false,
                    CreateDate = DateTime.UtcNow
                };

                _context.AppUsers.Add(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Hesabınız başarıyla oluşturuldu. Şimdi giriş yapabilirsiniz.";
                return RedirectToAction(nameof(Login));
            }

            return View(model);
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            // Session ve Cookie'leri temizle
            HttpContext.Session.Clear();
            Response.Cookies.Delete("UserId");
            
            TempData["SuccessMessage"] = "Başarıyla çıkış yapıldı.";
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Logout (for compatibility)
        public async Task<IActionResult> LogoutGet()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            // Session ve Cookie'leri temizle
            HttpContext.Session.Clear();
            Response.Cookies.Delete("UserId");
            
            TempData["SuccessMessage"] = "Başarıyla çıkış yapıldı.";
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Profile
        public async Task<IActionResult> Profile()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(Login));
            }

            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction(nameof(Login));
            }
            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
            {
                return NotFound();
            }

            var profileModel = new ProfileViewModel
            {
                Name = user.Name,
                Surname = user.SurName,
                Email = user.Email
            };

            return View(profileModel);
        }

        // POST: Account/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(Login));
            }

            if (ModelState.IsValid)
            {
                var userEmail = User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    return RedirectToAction(nameof(Login));
                }
                var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Email == userEmail);

                if (user == null)
                {
                    return NotFound();
                }

                user.Name = model.Name;
                user.SurName = model.Surname;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Profil bilgileriniz başarıyla güncellendi.";
                return RedirectToAction(nameof(Profile));
            }

            return View(model);
        }

        // GET: Account/ChangePassword
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(Login));
            }

            if (ModelState.IsValid)
            {
                var userEmail = User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    return RedirectToAction(nameof(Login));
                }
                var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Email == userEmail);

                if (user == null)
                {
                    return NotFound();
                }

                // Mevcut şifreyi kontrol et
                if (!VerifyPassword(model.CurrentPassword, user.Password))
                {
                    ModelState.AddModelError("CurrentPassword", "Mevcut şifre yanlış.");
                    return View(model);
                }

                // Yeni şifreyi hash'le ve güncelle
                user.Password = HashPassword(model.NewPassword);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirildi.";
                return RedirectToAction(nameof(Profile));
            }

            return View(model);
        }

        // GET: Account/ForgotPassword
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Email == model.Email);
                
                if (user != null)
                {
                    // Rastgele şifre oluştur
                    string newPassword = GenerateRandomPassword();
                    
                    // Şifreyi hash'le ve güncelle
                    user.Password = HashPassword(newPassword);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Şifreniz başarıyla sıfırlandı. Yeni şifreniz: {newPassword}";
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    // Güvenlik için aynı mesajı göster
                    TempData["SuccessMessage"] = "Şifre sıfırlama işlemi tamamlandı.";
                    return RedirectToAction(nameof(Login));
                }
            }

            return View(model);
        }

        // Helper methods
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hashedPassword;
        }

        // Rastgele şifre oluşturma metodu
        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var password = new char[8];
            
            for (int i = 0; i < 8; i++)
            {
                password[i] = chars[random.Next(chars.Length)];
            }
            
            return new string(password);
        }
    }
}
