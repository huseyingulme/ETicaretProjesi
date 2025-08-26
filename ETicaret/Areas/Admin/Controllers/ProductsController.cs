using ETicaret.Core.Entities;
using ETicaret.Data;
using ETicaret.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ETicaret.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]

    public class ProductsController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly FileHelper _fileHelper;

        public ProductsController(DatabaseContext context, FileHelper fileHelper)
        {
            _context = context;
            _fileHelper = fileHelper;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {
            var databaseContext = _context.Products.Include(p => p.Brand).Include(p => p.Category);
            return View(await databaseContext.ToListAsync());
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            ViewBag.BrandId = new SelectList(_context.Brands.Where(b => b.IsActive), "Id", "Name");
            ViewBag.CategoryId = new SelectList(_context.Categories.Where(c => c.IsActive), "Id", "Name");
            return View();
        }

        // POST: Admin/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? Image)
        {
            try
            {
                // Model validation kontrolü
                if (!ModelState.IsValid)
                {
                    // Validation hatalarını logla
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    System.Diagnostics.Debug.WriteLine($"Model validation errors: {string.Join(", ", errors)}");
                    
                    ViewBag.BrandId = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
                    ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                    return View(product);
                }

                // Resim validation - Create'de zorunlu
                if (Image == null || Image.Length == 0)
                {
                    ModelState.AddModelError("Image", "Lütfen bir resim dosyası seçin.");
                    ViewBag.BrandId = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
                    ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                    return View(product);
                }
                
                // File validation using FileHelper
                if (!_fileHelper.IsValidImageFile(Image))
                {
                    ModelState.AddModelError("Image", "Geçersiz dosya formatı veya boyutu. Sadece JPG, PNG, GIF, WebP dosyaları ve maksimum 5MB kabul edilir.");
                    ViewBag.BrandId = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
                    ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                    return View(product);
                }

                // Kategori ve marka varlığını kontrol et
                var categoryExists = await _context.Categories.AnyAsync(c => c.Id == product.CategoryId);
                var brandExists = await _context.Brands.AnyAsync(b => b.Id == product.BrandId);
                
                if (!categoryExists)
                {
                    ModelState.AddModelError("CategoryId", "Seçilen kategori bulunamadı.");
                    ViewBag.BrandId = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
                    ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                    return View(product);
                }
                
                if (!brandExists)
                {
                    ModelState.AddModelError("BrandId", "Seçilen marka bulunamadı.");
                    ViewBag.BrandId = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
                    ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                    return View(product);
                }
                
                // Upload image using FileHelper
                System.Diagnostics.Debug.WriteLine($"Resim yükleniyor: {Image.FileName}, Boyut: {Image.Length}");
                product.Image = await _fileHelper.UploadProductImageAsync(Image, product.Name);
                
                if (string.IsNullOrEmpty(product.Image))
                {
                    ModelState.AddModelError("Image", "Resim yüklenirken hata oluştu. Lütfen tekrar deneyin.");
                    ViewBag.BrandId = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
                    ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                    return View(product);
                }

                // Set default values
                if (product.OrderNo <= 0)
                    product.OrderNo = 1;
                    
                product.CreateDate = DateTime.UtcNow;
                product.IsActive = true;
                
                System.Diagnostics.Debug.WriteLine($"Ürün ekleniyor: {product.Name}, Fiyat: {product.Price}, Stok: {product.Stock}, Kategori: {product.CategoryId}, Marka: {product.BrandId}");
                
                _context.Add(product);
                var result = await _context.SaveChangesAsync();
                
                System.Diagnostics.Debug.WriteLine($"SaveChanges sonucu: {result} satır etkilendi");
                
                TempData["SuccessMessage"] = "Ürün başarıyla eklendi!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ürün eklenirken hata: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                ModelState.AddModelError("", $"Ürün eklenirken hata oluştu: {ex.Message}");
                ViewBag.BrandId = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
                ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                return View(product);
            }
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.BrandId = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? Image)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (existingProduct == null)
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
                            ViewBag.BrandId = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
                            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                            return View(product);
                        }

                        // Eski resmi sil
                        if (!string.IsNullOrEmpty(existingProduct.Image))
                        {
                            _fileHelper.RemoveProductImage(existingProduct.Image);
                        }

                        // Yeni resmi yükle
                        product.Image = await _fileHelper.UploadProductImageAsync(Image, product.Name);
                        
                        if (string.IsNullOrEmpty(product.Image))
                        {
                            ModelState.AddModelError("Image", "Resim yüklenirken hata oluştu. Lütfen tekrar deneyin.");
                            ViewBag.BrandId = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
                            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                            return View(product);
                        }
                    }
                    else
                    {
                        // Resim yüklenmediyse mevcut resmi koru
                        product.Image = existingProduct.Image;
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Ürün başarıyla güncellendi!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
                    ModelState.AddModelError("", $"Ürün güncellenirken hata oluştu: {ex.Message}");
                    ViewBag.BrandId = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
                    ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                    return View(product);
                }
            }
            
            ViewBag.BrandId = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                // Resim dosyasını sil
                if (!string.IsNullOrEmpty(product.Image))
                {
                    _fileHelper.RemoveProductImage(product.Image);
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Ürün başarıyla silindi!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
