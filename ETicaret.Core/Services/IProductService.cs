using ETicaret.Core.Entities;
using ETicaret.Core.Models;

namespace ETicaret.Core.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetProductsByBrandAsync(int brandId);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task<IEnumerable<Product>> GetFeaturedProductsAsync();
        Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<IEnumerable<Product>> GetProductsWithDiscountAsync();
        Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId, int categoryId, int count = 4);
        Task<IEnumerable<Product>> GetTopSellingProductsAsync(int count = 10);
        Task<IEnumerable<Product>> GetNewestProductsAsync(int count = 10);
        Task<Product> CreateProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> IsProductInStockAsync(int productId);
        Task<bool> UpdateProductStockAsync(int productId, int quantity);
        Task<decimal> CalculateDiscountedPriceAsync(int productId);
        Task<IEnumerable<Product>> GetProductsByTagsAsync(string[] tags);
        Task<ProductStatistics> GetProductStatisticsAsync();
        Task<bool> ValidateProductAsync(Product product);
        Task<IEnumerable<Product>> GetRecommendedProductsAsync(int userId, int count = 5);
    }
}
