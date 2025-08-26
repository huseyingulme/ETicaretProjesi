using System;
using System.ComponentModel.DataAnnotations;

namespace ETicaret.Core.Entities
{
    public class CartItem : IEntity
    {
        public CartItem()
        {
            CartId = 0;
            ProductId = 0;
            Quantity = 1;
            Price = 0;
            CreateDate = DateTime.UtcNow;
            IsActive = true;
        }

        public int Id { get; set; }
        
        [Required(ErrorMessage = "Sepet ID zorunludur.")]
        [Display(Name = "SEPET ID")]
        public int CartId { get; set; }
        
        [Required(ErrorMessage = "Ürün ID zorunludur.")]
        [Display(Name = "ÜRÜN ID")]
        public int ProductId { get; set; }
        
        [Required(ErrorMessage = "Miktar zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Miktar 1'den küçük olamaz.")]
        [Display(Name = "MİKTAR")]
        public int Quantity { get; set; }
        
        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
        [Display(Name = "FİYAT")]
        public decimal Price { get; set; }
        
        [Display(Name = "AKTİF")]
        public bool IsActive { get; set; }
        
        [Display(Name = "OLUŞTURULMA TARİHİ")]
        public DateTime CreateDate { get; set; }
        
        [Display(Name = "GÜNCELLEME TARİHİ")]
        public DateTime? UpdateDate { get; set; }
        

        public Cart Cart { get; set; } = null!;
        public Product Product { get; set; } = null!;
        

        public decimal TotalPrice => Price * Quantity;
    }
}
