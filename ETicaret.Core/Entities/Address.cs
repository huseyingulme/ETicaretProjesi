using System.ComponentModel.DataAnnotations;
using System;

namespace ETicaret.Core.Entities
{
    public class Address : IEntity
    {
        public Address()
        {
            Title = string.Empty;
            FullName = string.Empty;
            Phone = string.Empty;
            City = string.Empty;
            District = string.Empty;
            FullAddress = string.Empty;
            IsActive = true;
            CreateDate = DateTime.UtcNow;
        }

        public int Id { get; set; }
        
        [Display(Name = "ADRES BAŞLIĞI")]
        [Required(ErrorMessage = "Adres başlığı zorunludur")]
        public string Title { get; set; }
        
        [Display(Name = "AD SOYAD")]
        [Required(ErrorMessage = "Ad soyad zorunludur")]
        public string FullName { get; set; }
        
        [Display(Name = "TELEFON")]
        [Required(ErrorMessage = "Telefon zorunludur")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        public string Phone { get; set; }
        
        [Display(Name = "İL")]
        [Required(ErrorMessage = "İl zorunludur")]
        public string City { get; set; }
        
        [Display(Name = "İLÇE")]
        [Required(ErrorMessage = "İlçe zorunludur")]
        public string District { get; set; }
        
        [Display(Name = "AÇIK ADRES")]
        [Required(ErrorMessage = "Açık adres zorunludur")]
        public string FullAddress { get; set; }
        
        [Display(Name = "AKTİF")]
        public bool IsActive { get; set; }
        
        [Display(Name = "OLUŞTURULMA TARİHİ")]
        public DateTime CreateDate { get; set; }
        
        [Display(Name = "GÜNCELLEME TARİHİ")]
        public DateTime? UpdateDate { get; set; }
        

        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
