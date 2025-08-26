using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ETicaret.Services;
using ETicaret.Core.Entities;
using ETicaret.Core.Models;
using ETicaret.Data;
using Microsoft.EntityFrameworkCore;

namespace ETicaret.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly DatabaseContext _context;

        public OrdersController(IOrderService orderService, DatabaseContext context)
        {
            _orderService = orderService;
            _context = context;
        }

        // GET: Admin/Orders
        public async Task<IActionResult> Index()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Siparişler yüklenirken bir hata oluştu.";
                return View(new List<OrderViewModel>());
            }
        }

        // GET: Admin/Orders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdForAdminAsync(id);
                if (order == null)
                {
                    return NotFound();
                }

                return View(order);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Sipariş detayları yüklenirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Admin/Orders/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdForAdminAsync(id);
                if (order == null)
                {
                    return NotFound();
                }

                return View(order);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Sipariş düzenleme sayfası yüklenirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Admin/Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderViewModel model)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    return NotFound();
                }

                // Sipariş bilgilerini güncelle
                order.OrderStatus = model.OrderStatus;
                order.PaymentMethod = model.PaymentMethod;
                order.Notes = model.Notes;
                order.UpdateDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Sipariş başarıyla güncellendi.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Sipariş güncellenirken bir hata oluştu.";
                return View(model);
            }
        }

        // GET: Admin/Orders/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdForAdminAsync(id);
                if (order == null)
                {
                    return NotFound();
                }

                return View(order);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Sipariş silme sayfası yüklenirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    return NotFound();
                }

                order.IsActive = false;
                order.UpdateDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Sipariş başarıyla silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Sipariş silinirken bir hata oluştu.";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // POST: Admin/Orders/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            try
            {
                var success = await _orderService.UpdateOrderStatusAsync(id, status);
                if (success)
                {
                    TempData["SuccessMessage"] = "Sipariş durumu başarıyla güncellendi.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Sipariş durumu güncellenirken bir hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Sipariş durumu güncellenirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Admin/Orders/Statistics
        public async Task<IActionResult> Statistics()
        {
            try
            {
                var orders = await _context.Orders
                    .Where(o => o.IsActive)
                    .ToListAsync();

                var statistics = new
                {
                    TotalOrders = orders.Count,
                    PendingOrders = orders.Count(o => o.OrderStatus == OrderStatus.Pending),
                    ConfirmedOrders = orders.Count(o => o.OrderStatus == OrderStatus.Confirmed),
                    PreparingOrders = orders.Count(o => o.OrderStatus == OrderStatus.Preparing),
                    ShippedOrders = orders.Count(o => o.OrderStatus == OrderStatus.Shipped),
                    DeliveredOrders = orders.Count(o => o.OrderStatus == OrderStatus.Delivered),
                    CancelledOrders = orders.Count(o => o.OrderStatus == OrderStatus.Cancelled),
                    TotalRevenue = orders.Where(o => o.OrderStatus == OrderStatus.Delivered).Sum(o => o.TotalAmount),
                    AverageOrderValue = orders.Any() ? orders.Average(o => o.TotalAmount) : 0
                };

                return View(statistics);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "İstatistikler yüklenirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
