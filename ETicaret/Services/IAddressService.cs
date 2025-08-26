using ETicaret.Core.Entities;
using ETicaret.Core.Models;

namespace ETicaret.Services
{
    public interface IAddressService
    {
        Task<List<Address>> GetUserAddressesAsync(int userId);
        Task<Address> GetAddressByIdAsync(int addressId, int userId);
        Task<bool> CreateAddressAsync(AddressViewModel model, int userId);
        Task<bool> UpdateAddressAsync(AddressViewModel model, int userId);
        Task<bool> DeleteAddressAsync(int addressId, int userId);
        Task<bool> SetDefaultAddressAsync(int addressId, int userId);
    }
}
