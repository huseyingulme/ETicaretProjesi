using ETicaret.Core.Entities;
using ETicaret.Core.Models;
using ETicaret.Data;
using Microsoft.EntityFrameworkCore;

namespace ETicaret.Services
{
    public class AddressService : IAddressService
    {
        private readonly DatabaseContext _context;

        public AddressService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<Address>> GetUserAddressesAsync(int userId)
        {
            return await _context.Addresses
                .Where(a => a.AppUserId == userId && a.IsActive)
                .OrderByDescending(a => a.CreateDate)
                .ToListAsync();
        }

        public async Task<Address> GetAddressByIdAsync(int addressId, int userId)
        {
            return await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.AppUserId == userId && a.IsActive);
        }

        public async Task<bool> CreateAddressAsync(AddressViewModel model, int userId)
        {
            try
            {
                var address = new Address
                {
                    Title = model.Title,
                    FullName = model.FullName,
                    Phone = model.Phone,
                    City = model.City,
                    District = model.District,
                    FullAddress = model.FullAddress,
                    AppUserId = userId,
                    IsActive = true,
                    CreateDate = DateTime.UtcNow
                };

                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAddressAsync(AddressViewModel model, int userId)
        {
            try
            {
                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.Id == model.Id && a.AppUserId == userId && a.IsActive);

                if (address == null)
                    return false;

                address.Title = model.Title;
                address.FullName = model.FullName;
                address.Phone = model.Phone;
                address.City = model.City;
                address.District = model.District;
                address.FullAddress = model.FullAddress;
                address.UpdateDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAddressAsync(int addressId, int userId)
        {
            try
            {
                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.Id == addressId && a.AppUserId == userId && a.IsActive);

                if (address == null)
                    return false;

                address.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SetDefaultAddressAsync(int addressId, int userId)
        {
            try
            {
                // Tüm adresleri varsayılan olmaktan çıkar
                var allAddresses = await _context.Addresses
                    .Where(a => a.AppUserId == userId && a.IsActive)
                    .ToListAsync();

                foreach (var addr in allAddresses)
                {
                    // Burada IsDefault property'si eklenebilir
                    // Şimdilik sadece CreateDate'e göre sıralama yapılıyor
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
