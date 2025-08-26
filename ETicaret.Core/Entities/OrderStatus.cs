using System.ComponentModel.DataAnnotations;

namespace ETicaret.Core.Entities
{
    public enum OrderStatus
    {
        [Display(Name = "Beklemede")]
        Pending = 0,
        
        [Display(Name = "Onaylandı")]
        Confirmed = 1,
        
        [Display(Name = "Hazırlanıyor")]
        Preparing = 2,
        
        [Display(Name = "Kargoya Verildi")]
        Shipped = 3,
        
        [Display(Name = "Teslim Edildi")]
        Delivered = 4,
        
        [Display(Name = "İptal Edildi")]
        Cancelled = 5,
        
        [Display(Name = "İade Edildi")]
        Returned = 6
    }
}
