using ETicaret.Core.Entities;
using ETicaret.Core.Interfaces;
using ETicaret.Core.Models;
using ETicaret.Core.Services;
using ETicaret.Core.Validators;
using FluentValidation;

namespace ETicaret.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<ProductService> _logger;
        private readonly ProductValidator _validator;

        public ProductService(
            IProductRepository productRepository,
            ICacheService cacheService,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
            _logger = logger;
            _validator = new ProductValidator();
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            const string cacheKey = "all_products";
            
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                _logger.LogInformation("Fetching all products from database");
                return await _productRepository.GetAllAsync();
            }, TimeSpan.FromMinutes(30));
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            var cacheKey = $"product_{id}";
            
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                _logger.LogInformation("Fetching product {ProductId} from database", id);
                return await _productRepository.GetProductWithDetailsAsync(id);
            }, TimeSpan.FromMinutes(15));
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            var cacheKey = $"products_category_{categoryId}";
            
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                _logger.LogInformation("Fetching products for category {CategoryId}", categoryId);
                return await _productRepository.GetProductsByCategoryAsync(categoryId);
            }, TimeSpan.FromMinutes(20));
        }

        public async Task<IEnumerable<Product>> GetProductsByBrandAsync(int brandId)
        {
            var cacheKey = $"products_brand_{brandId}";
            
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                _logger.LogInformation("Fetching products for brand {BrandId}", brandId);
                return await _productRepository.GetProductsByBrandAsync(brandId);
            }, TimeSpan.FromMinutes(20));
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Enumerable.Empty<Product>();

            _logger.LogInformation("Searching products with term: {SearchTerm}", searchTerm);
            return await _productRepository.SearchProductsAsync(searchTerm);
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
        {
            const string cacheKey = "featured_products";
            
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                _logger.LogInformation("Fetching featured products");
                return await _productRepository.GetFeaturedProductsAsync();
            }, TimeSpan.FromMinutes(15));
        }

        public async Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            _logger.LogInformation("Fetching products in price range {MinPrice} - {MaxPrice}", minPrice, maxPrice);
            return await _productRepository.GetProductsByPriceRangeAsync(minPrice, maxPrice);
        }

        public async Task<IEnumerable<Product>> GetProductsWithDiscountAsync()
        {
            const string cacheKey = "discounted_products";
            
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                _logger.LogInformation("Fetching products with discount");
                return await _productRepository.GetProductsWithDiscountAsync();
            }, TimeSpan.FromMinutes(10));
        }

        public async Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId, int categoryId, int count = 4)
        {
            var cacheKey = $"related_products_{productId}_{categoryId}_{count}";
            
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                _logger.LogInformation("Fetching related products for product {ProductId}", productId);
                return await _productRepository.GetRelatedProductsAsync(productId, categoryId, count);
            }, TimeSpan.FromMinutes(30));
        }

        public async Task<IEnumerable<Product>> GetTopSellingProductsAsync(int count = 10)
        {
            var cacheKey = $"top_selling_products_{count}";
            
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                _logger.LogInformation("Fetching top {Count} selling products", count);
                return await _productRepository.GetTopSellingProductsAsync(count);
            }, TimeSpan.FromMinutes(20));
        }

        public async Task<IEnumerable<Product>> GetNewestProductsAsync(int count = 10)
        {
            var cacheKey = $"newest_products_{count}";
            
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                _logger.LogInformation("Fetching newest {Count} products", count);
                return await _productRepository.GetNewestProductsAsync(count);
            }, TimeSpan.FromMinutes(15));
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            var validationResult = await _validator.ValidateAsync(product);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException($"Product validation failed: {errors}");
            }

            _logger.LogInformation("Creating new product: {ProductName}", product.Name);
            var createdProduct = await _productRepository.AddAsync(product);
            
            // Cache'i temizle
            await _cacheService.RemoveByPatternAsync("products_");
            await _cacheService.RemoveByPatternAsync("all_products");
            
            return createdProduct;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            var validationResult = await _validator.ValidateAsync(product);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException($"Product validation failed: {errors}");
            }

            _logger.LogInformation("Updating product: {ProductId}", product.Id);
            await _productRepository.UpdateAsync(product);
            
            // Cache'i temizle
            await _cacheService.RemoveByPatternAsync($"product_{product.Id}");
            await _cacheService.RemoveByPatternAsync("products_");
            await _cacheService.RemoveByPatternAsync("all_products");
            
            return product;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product {ProductId} not found for deletion", id);
                return false;
            }

            _logger.LogInformation("Deleting product: {ProductId}", id);
            await _productRepository.DeleteAsync(product);
            
            // Cache'i temizle
            await _cacheService.RemoveByPatternAsync($"product_{id}");
            await _cacheService.RemoveByPatternAsync("products_");
            await _cacheService.RemoveByPatternAsync("all_products");
            
            return true;
        }

        public async Task<bool> IsProductInStockAsync(int productId)
        {
            return await _productRepository.IsProductInStockAsync(productId);
        }

        public async Task<bool> UpdateProductStockAsync(int productId, int quantity)
        {
            if (quantity <= 0)
            {
                _logger.LogWarning("Invalid quantity {Quantity} for product {ProductId}", quantity, productId);
                return false;
            }

            _logger.LogInformation("Updating stock for product {ProductId} by {Quantity}", productId, quantity);
            await _productRepository.UpdateProductStockAsync(productId, quantity);
            
            // Cache'i temizle
            await _cacheService.RemoveByPatternAsync($"product_{productId}");
            
            return true;
        }

        public async Task<decimal> CalculateDiscountedPriceAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                return 0;

            // Mevcut entity'de discount yok, bu yüzden normal fiyatı döndürüyoruz
            return product.Price;
        }

        public async Task<IEnumerable<Product>> GetProductsByTagsAsync(string[] tags)
        {
            if (tags == null || tags.Length == 0)
                return Enumerable.Empty<Product>();

            _logger.LogInformation("Fetching products by tags: {Tags}", string.Join(", ", tags));
            return await _productRepository.GetProductsByTagsAsync(tags);
        }

        public async Task<ProductStatistics> GetProductStatisticsAsync()
        {
            const string cacheKey = "product_statistics";
            
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                _logger.LogInformation("Generating product statistics");
                
                var allProducts = await _productRepository.GetAllAsync();
                var products = allProducts.ToList();

                var statistics = new ProductStatistics
                {
                    TotalProducts = products.Count,
                    ActiveProducts = products.Count(p => p.IsActive),
                    InactiveProducts = products.Count(p => !p.IsActive),
                    OutOfStockProducts = products.Count(p => p.Stock == 0),
                    LowStockProducts = products.Count(p => p.Stock > 0 && p.Stock <= 10),
                    FeaturedProducts = products.Count(p => p.IsHome),
                    ProductsWithDiscount = 0, // Mevcut entity'de discount yok
                    AveragePrice = products.Any() ? products.Average(p => p.Price) : 0,
                    HighestPrice = products.Any() ? products.Max(p => p.Price) : 0,
                    LowestPrice = products.Any() ? products.Min(p => p.Price) : 0,
                    TotalStockValue = products.Sum(p => p.Price * p.Stock)
                };

                return statistics;
            }, TimeSpan.FromMinutes(60));
        }

        public async Task<bool> ValidateProductAsync(Product product)
        {
            var validationResult = await _validator.ValidateAsync(product);
            return validationResult.IsValid;
        }

        public async Task<IEnumerable<Product>> GetRecommendedProductsAsync(int userId, int count = 5)
        {
            // Bu metod gelecekte kullanıcı davranış analizi ile geliştirilebilir
            // Şimdilik en çok satan ürünleri döndürüyoruz
            _logger.LogInformation("Getting recommended products for user {UserId}", userId);
            return await GetTopSellingProductsAsync(count);
        }
    }
}
