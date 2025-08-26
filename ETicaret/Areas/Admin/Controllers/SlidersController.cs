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

    public class SlidersController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly FileHelper _fileHelper;

        public SlidersController(DatabaseContext context, FileHelper fileHelper)
        {
            _context = context;
            _fileHelper = fileHelper;
        }

        // GET: Admin/Sliders
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sliders.ToListAsync());
        }

        // GET: Admin/Sliders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slider = await _context.Sliders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (slider == null)
            {
                return NotFound();
            }

            return View(slider);
        }

        // GET: Admin/Sliders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Sliders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Slider slider, IFormFile? Image)
        {
            // Debug: ModelState durumunu kontrol et
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                System.Diagnostics.Debug.WriteLine($"ModelState hataları: {string.Join(", ", errors)}");
            }

            try
            {
                // Resim validation - Create'de zorunlu
                if (Image == null || Image.Length == 0)
                {
                    ModelState.AddModelError("Image", "Lütfen bir resim dosyası seçin.");
                    return View(slider);
                }
                
                // File validation using FileHelper
                if (!_fileHelper.IsValidImageFile(Image))
                {
                    ModelState.AddModelError("Image", "Geçersiz dosya formatı veya boyutu. Sadece JPG, PNG, GIF, WebP dosyaları ve maksimum 5MB kabul edilir.");
                    return View(slider);
                }
                
                // Upload image using FileHelper
                slider.Image = await _fileHelper.UploadSliderImageAsync(Image, slider.Title ?? "slider");
                
                if (string.IsNullOrEmpty(slider.Image))
                {
                    ModelState.AddModelError("Image", "Resim yüklenirken hata oluştu. Lütfen tekrar deneyin.");
                    return View(slider);
                }

                // Set default values
                slider.CreateDate = DateTime.UtcNow;
                slider.IsActive = true;
                
                // Ensure required fields are not null
                if (string.IsNullOrEmpty(slider.Title))
                    slider.Title = "Slider";
                if (string.IsNullOrEmpty(slider.Description))
                    slider.Description = "";
                if (string.IsNullOrEmpty(slider.Link))
                    slider.Link = "";
                
                System.Diagnostics.Debug.WriteLine($"Slider ekleniyor: Title={slider.Title}, Image={slider.Image}");
                
                _context.Add(slider);
                var result = await _context.SaveChangesAsync();
                
                System.Diagnostics.Debug.WriteLine($"SaveChanges sonucu: {result} satır etkilendi");
                
                TempData["SuccessMessage"] = "Slider başarıyla eklendi!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                // Database specific error
                var innerException = dbEx.InnerException;
                var errorMessage = $"Veritabanı hatası: {dbEx.Message}";
                
                if (innerException != null)
                {
                    errorMessage += $"\nDetay: {innerException.Message}";
                }
                
                System.Diagnostics.Debug.WriteLine($"DbUpdateException: {errorMessage}");
                ModelState.AddModelError("", errorMessage);
                return View(slider);
            }
            catch (Exception ex)
            {
                // General error
                var errorMessage = $"Slider eklenirken hata oluştu: {ex.Message}";
                
                if (ex.InnerException != null)
                {
                    errorMessage += $"\nDetay: {ex.InnerException.Message}";
                }
                
                System.Diagnostics.Debug.WriteLine($"Exception: {errorMessage}");
                ModelState.AddModelError("", errorMessage);
                return View(slider);
            }
        }

        // GET: Admin/Sliders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slider = await _context.Sliders.FindAsync(id);
            if (slider == null)
            {
                return NotFound();
            }
            return View(slider);
        }

        // POST: Admin/Sliders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Slider slider, IFormFile? Image)
        {
            if (id != slider.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingSlider = await _context.Sliders.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
                    if (existingSlider == null)
                    {
                        return NotFound();
                    }

                    // Eğer yeni resim yüklendiyse
                    if (Image != null && Image.Length > 0)
                    {
                        // File validation using FileHelper
                        if (!_fileHelper.IsValidImageFile(Image))
                        {
                            ModelState.AddModelError("Image", "Geçersiz dosya formatı veya boyutu. Sadece JPG, PNG, GIF, WebP dosyaları ve maksimum 5MB kabul edilir.");
                            return View(slider);
                        }

                        // Eski resmi sil
                        if (!string.IsNullOrEmpty(existingSlider.Image))
                        {
                            _fileHelper.RemoveSliderImage(existingSlider.Image);
                        }

                        // Yeni resmi yükle
                        slider.Image = await _fileHelper.UploadSliderImageAsync(Image, slider.Title ?? "slider");
                        
                        if (string.IsNullOrEmpty(slider.Image))
                        {
                            ModelState.AddModelError("Image", "Resim yüklenirken hata oluştu. Lütfen tekrar deneyin.");
                            return View(slider);
                        }
                    }
                    else
                    {
                        // Resim yüklenmediyse mevcut resmi koru
                        slider.Image = existingSlider.Image;
                    }

                    _context.Update(slider);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Slider başarıyla güncellendi!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SliderExists(slider.Id))
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
                    ModelState.AddModelError("", $"Slider güncellenirken hata oluştu: {ex.Message}");
                    return View(slider);
                }
            }
            
            return View(slider);
        }

        // GET: Admin/Sliders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slider = await _context.Sliders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (slider == null)
            {
                return NotFound();
            }

            return View(slider);
        }

        // POST: Admin/Sliders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var slider = await _context.Sliders.FindAsync(id);
            if (slider != null)
            {
                // Resim dosyasını sil
                if (!string.IsNullOrEmpty(slider.Image))
                {
                    _fileHelper.RemoveSliderImage(slider.Image);
                }

                _context.Sliders.Remove(slider);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Slider başarıyla silindi!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SliderExists(int id)
        {
            return _context.Sliders.Any(e => e.Id == id);
        }
    }
}
