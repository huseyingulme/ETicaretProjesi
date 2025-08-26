using ETicaret.Core.Models;
using ETicaret.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ETicaret.Controllers
{
    [Authorize]
    public class AddressController : Controller
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return RedirectToAction("Login", "Account");

            var addresses = await _addressService.GetUserAddressesAsync(userId);
            return View(addresses);
        }

        public IActionResult Create()
        {
            return View(new AddressViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return RedirectToAction("Login", "Account");

                var result = await _addressService.CreateAddressAsync(model, userId);
                if (result)
                {
                    TempData["SuccessMessage"] = "Adres başarıyla eklendi.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Adres eklenirken bir hata oluştu.");
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return RedirectToAction("Login", "Account");

            var address = await _addressService.GetAddressByIdAsync(id, userId);
            if (address == null)
                return NotFound();

            var model = new AddressViewModel
            {
                Id = address.Id,
                Title = address.Title,
                FullName = address.FullName,
                Phone = address.Phone,
                City = address.City,
                District = address.District,
                FullAddress = address.FullAddress
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return RedirectToAction("Login", "Account");

                var result = await _addressService.UpdateAddressAsync(model, userId);
                if (result)
                {
                    TempData["SuccessMessage"] = "Adres başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Adres güncellenirken bir hata oluştu.");
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return RedirectToAction("Login", "Account");

            var result = await _addressService.DeleteAddressAsync(id, userId);
            if (result)
            {
                TempData["SuccessMessage"] = "Adres başarıyla silindi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Adres silinirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
                return userId;
            return 0;
        }
    }
}
