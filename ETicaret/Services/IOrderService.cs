using ETicaret.Core.Entities;
using ETicaret.Core.Models;

namespace ETicaret.Services
{
    public interface IOrderService
    {
        Task<CheckoutViewModel> GetCheckoutViewModelAsync(int userId);
        Task<Order> CreateOrderAsync(CheckoutViewModel model, int userId);
        Task<OrderViewModel> GetOrderByIdAsync(int orderId, int userId);
        Task<List<OrderViewModel>> GetUserOrdersAsync(int userId);
        Task<OrderListViewModel> GetUserOrderListAsync(int userId);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task<string> GenerateOrderNumberAsync();
        Task<bool> CancelOrderAsync(int orderId, int userId);
        Task<decimal> CalculateShippingCostAsync(decimal totalAmount);
        Task<bool> ValidateOrderAsync(CheckoutViewModel model, int userId);
        
        // Admin için ek metodlar
        Task<List<OrderViewModel>> GetAllOrdersAsync();
        Task<OrderViewModel> GetOrderByIdForAdminAsync(int orderId);
    }
}
