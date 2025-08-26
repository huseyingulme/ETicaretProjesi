using ETicaret.Core.Entities;
using ETicaret.Core.Interfaces;
using ETicaret.Data;
using Microsoft.EntityFrameworkCore;

namespace ETicaret.Data.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(DatabaseContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByBrandAsync(int brandId)
        {
            return await _dbSet
                .Where(p => p.BrandId == brandId && p.IsActive)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            return await _dbSet
                .Where(p => p.IsActive && 
                           (p.Name.Contains(searchTerm) || 
                            p.Description.Contains(searchTerm) ||
                            (p.ProductCode != null && p.ProductCode.Contains(searchTerm))))
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
        {
            return await _dbSet
                .Where(p => p.IsActive && p.IsHome)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .OrderByDescending(p => p.CreateDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _dbSet
                .Where(p => p.IsActive && p.Price >= minPrice && p.Price <= maxPrice)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .OrderBy(p => p.Price)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsWithDiscountAsync()
        {
            // Mevcut entity'de discount yok, bu yüzden boş liste döndürüyoruz
            return await Task.FromResult(Enumerable.Empty<Product>());
        }

        public async Task<Product?> GetProductWithDetailsAsync(int id)
        {
            return await _dbSet
                .Where(p => p.Id == id && p.IsActive)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId, int categoryId, int count = 4)
        {
            return await _dbSet
                .Where(p => p.CategoryId == categoryId && p.Id != productId && p.IsActive)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .OrderBy(x => Guid.NewGuid())
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetTopSellingProductsAsync(int count = 10)
        {
            return await _dbSet
                .Where(p => p.IsActive)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .OrderByDescending(p => p.OrderNo)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetNewestProductsAsync(int count = 10)
        {
            return await _dbSet
                .Where(p => p.IsActive)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .OrderByDescending(p => p.CreateDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<decimal> GetAverageRatingAsync(int productId)
        {
            // Bu metod gelecekte rating sistemi eklendiğinde kullanılabilir
            // Şimdilik varsayılan değer döndürüyoruz
            return await Task.FromResult(4.5m);
        }

        public async Task<int> GetTotalStockAsync(int productId)
        {
            var product = await _dbSet.FindAsync(productId);
            return product?.Stock ?? 0;
        }

        public async Task<bool> IsProductInStockAsync(int productId)
        {
            var product = await _dbSet.FindAsync(productId);
            return product != null && product.Stock > 0;
        }

        public async Task UpdateProductStockAsync(int productId, int quantity)
        {
            var product = await _dbSet.FindAsync(productId);
            if (product != null)
            {
                product.Stock -= quantity;
                // Mevcut entity'de UpdateDate yok, bu yüzden sadece stock güncelliyoruz
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetProductsByTagsAsync(string[] tags)
        {
            // Mevcut entity'de tags yok, bu yüzden boş liste döndürüyoruz
            return await Task.FromResult(Enumerable.Empty<Product>());
        }
    }
}
