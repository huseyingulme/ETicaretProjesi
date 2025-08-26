using ETicaret.Core.Entities;
using ETicaret.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace ETicaret.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductApiController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductApiController> _logger;

        public ProductApiController(IProductService productService, ILogger<ProductApiController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                    return NotFound();

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product {ProductId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(int categoryId)
        {
            try
            {
                var products = await _productService.GetProductsByCategoryAsync(categoryId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products for category {CategoryId}", categoryId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("brand/{brandId}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByBrand(int brandId)
        {
            try
            {
                var products = await _productService.GetProductsByBrandAsync(brandId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products for brand {BrandId}", brandId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Product>>> SearchProducts([FromQuery] string q)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                    return BadRequest("Search query cannot be empty");

                var products = await _productService.SearchProductsAsync(q);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products with query {Query}", q);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("featured")]
        public async Task<ActionResult<IEnumerable<Product>>> GetFeaturedProducts()
        {
            try
            {
                var products = await _productService.GetFeaturedProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting featured products");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("discounted")]
        public async Task<ActionResult<IEnumerable<Product>>> GetDiscountedProducts()
        {
            try
            {
                var products = await _productService.GetProductsWithDiscountAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting discounted products");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("top-selling")]
        public async Task<ActionResult<IEnumerable<Product>>> GetTopSellingProducts([FromQuery] int count = 10)
        {
            try
            {
                var products = await _productService.GetTopSellingProductsAsync(count);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top selling products");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("newest")]
        public async Task<ActionResult<IEnumerable<Product>>> GetNewestProducts([FromQuery] int count = 10)
        {
            try
            {
                var products = await _productService.GetNewestProductsAsync(count);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting newest products");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/related")]
        public async Task<ActionResult<IEnumerable<Product>>> GetRelatedProducts(int id, [FromQuery] int categoryId, [FromQuery] int count = 4)
        {
            try
            {
                var products = await _productService.GetRelatedProductsAsync(id, categoryId, count);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting related products for product {ProductId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("price-range")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
        {
            try
            {
                if (minPrice < 0 || maxPrice < 0 || minPrice > maxPrice)
                    return BadRequest("Invalid price range");

                var products = await _productService.GetProductsByPriceRangeAsync(minPrice, maxPrice);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products by price range");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("statistics")]
        public async Task<ActionResult> GetProductStatistics()
        {
            try
            {
                var statistics = await _productService.GetProductStatisticsAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product statistics");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/stock")]
        public async Task<ActionResult<bool>> CheckProductStock(int id)
        {
            try
            {
                var inStock = await _productService.IsProductInStockAsync(id);
                return Ok(new { productId = id, inStock });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking stock for product {ProductId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/discounted-price")]
        public async Task<ActionResult<decimal>> GetDiscountedPrice(int id)
        {
            try
            {
                var discountedPrice = await _productService.CalculateDiscountedPriceAsync(id);
                return Ok(new { productId = id, discountedPrice });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating discounted price for product {ProductId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdProduct = await _productService.CreateProductAsync(product);
                return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> UpdateProduct(int id, [FromBody] Product product)
        {
            try
            {
                if (id != product.Id)
                    return BadRequest("Product ID mismatch");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updatedProduct = await _productService.UpdateProductAsync(product);
                return Ok(updatedProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
