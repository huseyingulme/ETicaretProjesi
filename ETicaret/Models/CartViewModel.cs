using System.ComponentModel.DataAnnotations;

namespace ETicaret.Models
{
    public class CartViewModel
    {
        public int Id { get; set; }
        public string SessionId { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public List<CartItemViewModel> Items { get; set; } = new();
        public int TotalItems => Items?.Count ?? 0;
        public decimal TotalPrice => Items?.Sum(x => x.TotalPrice) ?? 0;
        public bool IsEmpty => !Items.Any();
    }

    public class CartItemViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice => Price * Quantity;
        public int Stock { get; set; }
        public bool IsAvailable => Stock > 0;
    }

    public class AddToCartViewModel
    {
        [Required(ErrorMessage = "Ürün ID zorunludur.")]
        public int ProductId { get; set; }
        
        [Required(ErrorMessage = "Miktar zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Miktar 1'den küçük olamaz.")]
        public int Quantity { get; set; } = 1;
        
        public string? ReturnUrl { get; set; }
    }

    public class UpdateCartItemViewModel
    {
        [Required(ErrorMessage = "Sepet öğesi ID zorunludur.")]
        public int CartItemId { get; set; }
        
        [Required(ErrorMessage = "Miktar zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Miktar 1'den küçük olamaz.")]
        public int Quantity { get; set; }
    }

    public class CartSummaryViewModel
    {
        public int TotalItems { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsEmpty { get; set; }
    }
}
