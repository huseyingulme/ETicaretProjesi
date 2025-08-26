using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.Core.Entities
{
    public class Product : IEntity
    {
        public Product()
        {
            Name = string.Empty;
            Description = string.Empty;
            Image = string.Empty;
            ImageUrl = string.Empty;
            ProductCode = string.Empty;
            Price = 0;
            Stock = 0;
            IsActive = true;
            IsHome = false;
            OrderNo = 1;
            CreateDate = DateTime.UtcNow;
        }

        public int Id { get; set; }
        
        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        [StringLength(200, ErrorMessage = "Ürün adı en fazla 200 karakter olabilir.")]
        [Display(Name = "ÜRÜN ADI")]
        public string Name { get; set; }
        
        [StringLength(1000, ErrorMessage = "Ürün açıklaması en fazla 1000 karakter olabilir.")]
        [Display(Name = "ÜRÜN AÇIKLAMASI")]
        public string Description { get; set; }
        
        [Display(Name = "ÜRÜN RESMİ")]
        public string Image { get; set; }
        
        [Display(Name = "ÜRÜN RESMİ URL")]
        public string ImageUrl { get; set; }
        
        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
        [Display(Name = "FİYAT")]
        public decimal Price { get; set; }
        
        [StringLength(50, ErrorMessage = "Ürün kodu en fazla 50 karakter olabilir.")]
        [Display(Name = "ÜRÜN KODU")]
        public string? ProductCode { get; set; }    
        
        [Required(ErrorMessage = "Stok miktarı zorunludur.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stok miktarı 0'dan küçük olamaz.")]
        [Display(Name = "STOK MİKTARI")]
        public int Stock { get; set; }  
        
        [Display(Name = "AKTİF")]
        public bool IsActive { get; set; }
        
        [Display(Name = "ANASAYFADA GÖSTER")]
        public bool IsHome { get; set; }
        
        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        [Display(Name = "KATEGORİ")]
        public int CategoryId { get; set; }   
        
        [Display(Name = "KATEGORİ")]
        public Category? Category { get; set; } 
        
        [Required(ErrorMessage = "Marka seçimi zorunludur.")]
        [Display(Name = "MARKA")]
        public int BrandId { get; set; }     
        
        [Display(Name = "MARKA")]
        public Brand? Brand { get; set; } 
        
        [Range(1, int.MaxValue, ErrorMessage = "Sıra no 1'den küçük olamaz.")]
        [Display(Name = "SIRA NO")]
        public int OrderNo { get; set; }
        
        [Display(Name = "OLUŞTURULMA TARİHİ"), ScaffoldColumn(false)]
        public DateTime CreateDate { get; set; }
    }
}
