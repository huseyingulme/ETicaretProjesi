using ETicaret.Core.Entities;
using ETicaret.Models;

namespace ETicaret.Services
{
    public interface ICartService
    {
        Task<Cart> GetOrCreateCartAsync(string sessionId, int? userId = null);
        Task<CartItem> AddToCartAsync(int productId, int quantity, string sessionId, int? userId = null);
        Task<bool> UpdateCartItemAsync(int cartItemId, int quantity);
        Task<bool> RemoveFromCartAsync(int cartItemId);
        Task<bool> ClearCartAsync(string sessionId, int? userId = null);
        Task<CartViewModel> GetCartViewModelAsync(string sessionId, int? userId = null);
        Task<CartSummaryViewModel> GetCartSummaryAsync(string sessionId, int? userId = null);
        Task<bool> TransferCartToUserAsync(string sessionId, int userId);
        Task<int> GetCartItemCountAsync(string sessionId, int? userId = null);
        
        // Order için gerekli method'lar
        Task<List<CartItem>> GetUserCartItemsAsync(int userId);
        Task<bool> ClearUserCartAsync(int userId);
    }
}
