using System.ComponentModel.DataAnnotations;
using ETicaret.Core.Entities;

namespace ETicaret.Core.Models
{
    public class CheckoutViewModel
    {
        public CheckoutViewModel()
        {
            CartItems = new List<CartItemViewModel>();
            Addresses = new List<AddressViewModel>();
            SelectedAddressId = 0;
            PaymentMethod = PaymentMethod.CreditCard;
            Notes = string.Empty;
        }

        [Display(Name = "Sepet Ürünleri")]
        public List<CartItemViewModel> CartItems { get; set; }
        
        [Display(Name = "Adresler")]
        public List<AddressViewModel> Addresses { get; set; }
        
        [Required(ErrorMessage = "Adres seçimi zorunludur.")]
        [Display(Name = "Teslimat Adresi")]
        public int SelectedAddressId { get; set; }
        
        [Required(ErrorMessage = "Ödeme yöntemi seçimi zorunludur.")]
        [Display(Name = "Ödeme Yöntemi")]
        public PaymentMethod PaymentMethod { get; set; }
        
        [Display(Name = "Sipariş Notları")]
        [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir.")]
        public string? Notes { get; set; }
        
        [Display(Name = "Kargo Ücreti")]
        public decimal ShippingCost { get; set; } = 0;
        
        [Display(Name = "Toplam Tutar")]
        public decimal TotalAmount { get; set; }
        
        [Display(Name = "Genel Toplam")]
        public decimal GrandTotal => TotalAmount + ShippingCost;
    }

    public class CartItemViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity;
    }
}
