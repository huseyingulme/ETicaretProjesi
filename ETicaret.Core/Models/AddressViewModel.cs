using System.ComponentModel.DataAnnotations;

namespace ETicaret.Core.Models
{
    public class AddressViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "ADRES BAŞLIĞI")]
        [Required(ErrorMessage = "Adres başlığı zorunludur")]
        public string Title { get; set; } = string.Empty;
        
        [Display(Name = "AD SOYAD")]
        [Required(ErrorMessage = "Ad soyad zorunludur")]
        public string FullName { get; set; } = string.Empty;
        
        [Display(Name = "TELEFON")]
        [Required(ErrorMessage = "Telefon zorunludur")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        public string Phone { get; set; } = string.Empty;
        
        [Display(Name = "İL")]
        [Required(ErrorMessage = "İl zorunludur")]
        public string City { get; set; } = string.Empty;
        
        [Display(Name = "İLÇE")]
        [Required(ErrorMessage = "İlçe zorunludur")]
        public string District { get; set; } = string.Empty;
        
        [Display(Name = "AÇIK ADRES")]
        [Required(ErrorMessage = "Açık adres zorunludur")]
        public string FullAddress { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
