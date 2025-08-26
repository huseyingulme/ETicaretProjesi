using ETicaret.Core.Entities;
using ETicaret.Data;
using ETicaret.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ETicaret.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]

    public class BrandsController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly FileHelper _fileHelper;

        public BrandsController(DatabaseContext context, IWebHostEnvironment webHostEnvironment, FileHelper fileHelper)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _fileHelper = fileHelper;
        }

        // GET: Admin/Brands
        public async Task<IActionResult> Index()
        {
            var brands = await _context.Brands
                .OrderBy(b => b.OrderNo)
                .ThenBy(b => b.Name)
                .ToListAsync();
            return View(brands);
        }

        // GET: Admin/Brands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // GET: Admin/Brands/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Brands/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Brand brand, IFormFile? Logo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Logo == null || Logo.Length == 0)
                    {
                        ModelState.AddModelError("Logo", "Lütfen bir logo dosyası seçin.");
                        return View(brand);
                    }

                    if (!_fileHelper.IsValidImageFile(Logo))
                    {
                        ModelState.AddModelError("Logo", "Geçersiz dosya formatı veya boyutu. Sadece JPG, PNG, GIF, WebP dosyaları ve maksimum 5MB kabul edilir.");
                        return View(brand);
                    }

                    brand.Logo = await _fileHelper.UploadLogoAsync(Logo, brand.Name);
                    
                    if (string.IsNullOrEmpty(brand.Logo))
                    {
                        ModelState.AddModelError("Logo", "Logo yüklenirken hata oluştu. Lütfen tekrar deneyin.");
                        return View(brand);
                    }
                    
                    if (brand.OrderNo <= 0)
                        brand.OrderNo = 1;
                        
                    brand.CreateDate = DateTime.UtcNow;
                    brand.IsActive = true;
                    
                    _context.Add(brand);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Marka başarıyla eklendi!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Marka eklenirken hata: {ex.Message}");
                    ModelState.AddModelError("", "Marka eklenirken hata oluştu: " + ex.Message);
                    return View(brand);
                }
            }
            return View(brand);
        }

        // GET: Admin/Brands/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        // POST: Admin/Brands/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Brand brand, IFormFile? newLogo, bool removeLogo = false)
        {
            if (id != brand.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingBrand = await _context.Brands.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
                    if (existingBrand == null)
                    {
                        return NotFound();
                    }

                    if (removeLogo)
                    {
                        if (!string.IsNullOrEmpty(existingBrand.Logo))
                        {
                            _fileHelper.RemoveLogo(existingBrand.Logo);
                            brand.Logo = null;
                        }
                    }
                    else if (newLogo != null && newLogo.Length > 0)
                    {
                        if (!_fileHelper.IsValidImageFile(newLogo))
                        {
                            ModelState.AddModelError("newLogo", "Geçersiz dosya formatı veya boyutu. Sadece JPG, PNG, GIF, WebP dosyaları ve maksimum 5MB kabul edilir.");
                            return View(brand);
                        }
                        
                        if (!string.IsNullOrEmpty(existingBrand.Logo))
                        {
                            _fileHelper.RemoveLogo(existingBrand.Logo);
                        }
                        
                        brand.Logo = await _fileHelper.UploadLogoAsync(newLogo, brand.Name);
                        
                        if (string.IsNullOrEmpty(brand.Logo))
                        {
                            ModelState.AddModelError("newLogo", "Yeni logo yüklenirken hata oluştu. Lütfen tekrar deneyin.");
                            return View(brand);
                        }
                    }
                    else
                    {
                        brand.Logo = existingBrand.Logo;
                    }
                    
                    brand.CreateDate = existingBrand.CreateDate;
                    brand.UpdateDate = DateTime.UtcNow;
                    
                    _context.Update(brand);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Marka başarıyla güncellendi!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrandExists(brand.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Marka güncellenirken hata: {ex.Message}");
                    ModelState.AddModelError("", "Marka güncellenirken hata oluştu: " + ex.Message);
                    return View(brand);
                }
            }
            return View(brand);
        }

        // GET: Admin/Brands/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // POST: Admin/Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(brand.Logo))
                    {
                        _fileHelper.RemoveLogo(brand.Logo);
                    }
                    
                    _context.Brands.Remove(brand);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Marka başarıyla silindi!";
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Marka silinirken hata: {ex.Message}");
                    TempData["ErrorMessage"] = "Marka silinirken hata oluştu: " + ex.Message;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Brands/ChangeLogo/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeLogo(int id, IFormFile newLogo)
        {
            if (newLogo == null || newLogo.Length == 0)
            {
                return Json(new { success = false, message = "Lütfen bir logo dosyası seçin." });
            }
            
            if (!_fileHelper.IsValidImageFile(newLogo))
            {
                return Json(new { success = false, message = "Geçersiz dosya formatı veya boyutu. Sadece JPG, PNG, GIF, WebP dosyaları ve maksimum 5MB kabul edilir." });
            }
            
            try
            {
                var brand = await _context.Brands.FindAsync(id);
                if (brand == null)
                {
                    return Json(new { success = false, message = "Marka bulunamadı." });
                }
                
                if (!string.IsNullOrEmpty(brand.Logo))
                {
                    _fileHelper.RemoveLogo(brand.Logo);
                }
                
                var newLogoPath = await _fileHelper.UploadLogoAsync(newLogo, brand.Name);
                
                if (string.IsNullOrEmpty(newLogoPath))
                {
                    return Json(new { success = false, message = "Logo yüklenirken hata oluştu. Lütfen tekrar deneyin." });
                }
                
                brand.Logo = newLogoPath;
                brand.UpdateDate = DateTime.UtcNow;
                
                _context.Update(brand);
                await _context.SaveChangesAsync();
                
                return Json(new { success = true, message = "Logo başarıyla güncellendi!", logoPath = newLogoPath });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Logo güncellenirken hata: {ex.Message}");
                return Json(new { success = false, message = "Logo güncellenirken hata oluştu: " + ex.Message });
            }
        }

        private bool BrandExists(int id)
        {
            return _context.Brands.Any(e => e.Id == id);
        }
    }
}
