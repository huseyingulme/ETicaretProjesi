using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ETicaret.Data;

namespace ETicaret.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MainController : Controller
    {
        private readonly DatabaseContext _context;

        public MainController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // İstatistikleri hesapla
            ViewBag.ProductCount = await _context.Products.CountAsync();
            ViewBag.CategoryCount = await _context.Categories.CountAsync();
            ViewBag.BrandCount = await _context.Brands.CountAsync();
            ViewBag.UserCount = await _context.AppUsers.CountAsync();
            ViewBag.SliderCount = await _context.Sliders.CountAsync();
            ViewBag.ContactCount = await _context.Contacts.CountAsync();
            ViewBag.OrderCount = await _context.Orders.Where(o => o.IsActive).CountAsync();

            return View();
        }
    }
}
