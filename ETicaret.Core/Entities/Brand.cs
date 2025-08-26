using System.ComponentModel.DataAnnotations;

namespace ETicaret.Core.Entities
{
    public class Brand : IEntity
    {
        public Brand()
        {
            Name = string.Empty;
            Description = string.Empty;
            Logo = string.Empty;
            Image = string.Empty;
            IsActive = true;
            OrderNo = 1;
            CreateDate = DateTime.UtcNow;
            Products = new List<Product>();
        }

        public int Id { get; set; }
        
        [Display(Name = "MARKA ADI")]
        [Required(ErrorMessage = "Marka adı zorunludur")]
        [StringLength(100, ErrorMessage = "Marka adı en fazla 100 karakter olabilir")]
        public string Name { get; set; }
        
        [Display(Name = "AÇIKLAMA")]
        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string Description { get; set; }
        
        [Display(Name = "LOGO")]
        public string? Logo { get; set; }
        
        [Display(Name = "RESİM")]
        public string Image { get; set; }
        
        [Display(Name = "AKTİF")]
        public bool IsActive { get; set; } = true;
        
        [Display(Name = "SIRA NO")]
        [Range(1, 999, ErrorMessage = "Sıra no 1-999 arasında olmalıdır")]
        public int OrderNo { get; set; } = 1;
        
        [Display(Name = "OLUŞTURMA TARİHİ"), ScaffoldColumn(false)]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        
        [Display(Name = "GÜNCELLEME TARİHİ"), ScaffoldColumn(false)]
        public DateTime? UpdateDate { get; set; }
        
        public IList<Product> Products { get; set; } = new List<Product>();
    }
}
