using ETicaret.Core.Entities;

namespace ETicaret.Core.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetProductsByBrandAsync(int brandId);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task<IEnumerable<Product>> GetFeaturedProductsAsync();
        Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<IEnumerable<Product>> GetProductsWithDiscountAsync();
        Task<Product?> GetProductWithDetailsAsync(int id);
        Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId, int categoryId, int count = 4);
        Task<IEnumerable<Product>> GetTopSellingProductsAsync(int count = 10);
        Task<IEnumerable<Product>> GetNewestProductsAsync(int count = 10);
        Task<decimal> GetAverageRatingAsync(int productId);
        Task<int> GetTotalStockAsync(int productId);
        Task<bool> IsProductInStockAsync(int productId);
        Task UpdateProductStockAsync(int productId, int quantity);
        Task<IEnumerable<Product>> GetProductsByTagsAsync(string[] tags);
    }
}
