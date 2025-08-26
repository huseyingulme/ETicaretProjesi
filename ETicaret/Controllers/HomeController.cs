using System.Diagnostics;
using ETicaret.Core.Models;
using ETicaret.Core.Entities;
using ETicaret.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ETicaret.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DatabaseContext _context;

        public HomeController(ILogger<HomeController> logger, DatabaseContext context)
        {
            _logger = logger;
            _context = context;
        }

        private async Task SetCommonViewBag()
        {
            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.OrderNo).ToListAsync();
            ViewBag.Brands = await _context.Brands.Where(b => b.IsActive).OrderBy(b => b.OrderNo).ToListAsync();
        }

        private async Task<List<int>> GetFavoriteProductIds()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return new List<int>();
                }
                
                var favoriteIds = await _context.Favorites
                    .Where(f => f.UserId == userId && f.IsActive)
                    .Select(f => f.ProductId)
                    .ToListAsync();
                    
                return favoriteIds;
            }
            catch
            {
                return new List<int>();
            }
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 8)
        {
            await SetCommonViewBag();
            
            var viewModel = new HomeViewModel
            {
                Sliders = await _context.Sliders.Where(s => s.IsActive).OrderBy(s => s.OrderNo).ToListAsync(),
                Categories = await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.OrderNo).ToListAsync(),
                Brands = await _context.Brands.Where(b => b.IsActive).OrderBy(b => b.OrderNo).ToListAsync()
            };

            // Ürünler için sayfalama
            var productsQuery = _context.Products
                .Where(p => p.IsActive && p.IsHome)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .OrderBy(p => p.OrderNo);

            var totalProducts = await productsQuery.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            viewModel.HomeProducts = await productsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Sayfalama bilgilerini ViewBag'e ekle
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalProducts = totalProducts;
            ViewBag.PageSize = pageSize;
            
            // Favori ürün ID'lerini ViewBag'e ekle
            ViewBag.FavoriteProductIds = await GetFavoriteProductIds();
            
            return View(viewModel);
        }

        public async Task<IActionResult> CategoryProducts(int id, int page = 1, int pageSize = 12)
        {
            await SetCommonViewBag();
            
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            var query = _context.Products
                .Where(p => p.IsActive && p.CategoryId == id)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .OrderBy(p => p.OrderNo);

            var totalProducts = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CategoryName = category.Name;
            ViewBag.CategoryId = id;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalProducts = totalProducts;
            ViewBag.PageSize = pageSize;
            ViewBag.FavoriteProductIds = await GetFavoriteProductIds();

            return View(products);
        }

        public async Task<IActionResult> BrandProducts(int id, int page = 1, int pageSize = 12)
        {
            await SetCommonViewBag();
            
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
                return NotFound();

            var query = _context.Products
                .Where(p => p.IsActive && p.BrandId == id)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .OrderBy(p => p.OrderNo);

            var totalProducts = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.BrandName = brand.Name;
            ViewBag.BrandId = id;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalProducts = totalProducts;
            ViewBag.PageSize = pageSize;
            ViewBag.FavoriteProductIds = await GetFavoriteProductIds();

            return View(products);
        }

        public async Task<IActionResult> Search(string q, int page = 1, int pageSize = 12)
        {
            await SetCommonViewBag();

            if (string.IsNullOrWhiteSpace(q))
            {
                return RedirectToAction("Index");
            }

            var query = _context.Products
                .Where(p => p.IsActive && (
                    p.Name.Contains(q) ||
                    p.Description.Contains(q) ||
                    (p.Category != null && p.Category.Name.Contains(q)) ||
                    (p.Brand != null && p.Brand.Name.Contains(q))
                ))
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .OrderBy(p => p.OrderNo);

            var totalProducts = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.SearchQuery = q;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalProducts = totalProducts;
            ViewBag.PageSize = pageSize;
            ViewBag.FavoriteProductIds = await GetFavoriteProductIds();

            return View(products);
        }

        public async Task<IActionResult> Contact()
        {
            await SetCommonViewBag();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                var contact = new Contact
                {
                    Name = model.Name,
                    Surname = string.Empty, // Boş bırakıyoruz çünkü ContactViewModel'de sadece Name var
                    Email = model.Email,
                    Subject = model.Subject,
                    Message = model.Message,
                    CreateDate = DateTime.UtcNow
                };

                _context.Contacts.Add(contact);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Mesajınız başarıyla gönderildi. En kısa sürede size dönüş yapacağız.";
                return RedirectToAction(nameof(Contact));
            }

            await SetCommonViewBag();
            return View(model);
        }

        public async Task<IActionResult> ProductDetail(int id)
        {
            await SetCommonViewBag();
            
            var product = await _context.Products
                .Where(p => p.IsActive && p.Id == id)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync();

            if (product == null)
                return NotFound();

            // Benzer ürünleri getir (aynı kategoride)
            var relatedProducts = await _context.Products
                .Where(p => p.IsActive && p.CategoryId == product.CategoryId && p.Id != product.Id)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .OrderBy(p => p.OrderNo)
                .Take(4)
                .ToListAsync();

            ViewBag.RelatedProducts = relatedProducts;
            ViewBag.FavoriteProductIds = await GetFavoriteProductIds();
            
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
