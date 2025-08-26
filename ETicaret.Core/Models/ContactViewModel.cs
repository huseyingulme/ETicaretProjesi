using System.ComponentModel.DataAnnotations;

namespace ETicaret.Core.Models
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Ad alanı zorunludur")]
        [Display(Name = "Ad Soyad")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta alanı zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Konu alanı zorunludur")]
        [Display(Name = "Konu")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mesaj alanı zorunludur")]
        [Display(Name = "Mesaj")]
        [StringLength(1000, ErrorMessage = "Mesaj en fazla 1000 karakter olabilir")]
        public string Message { get; set; } = string.Empty;
    }
}
