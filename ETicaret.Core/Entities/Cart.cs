using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace ETicaret.Core.Entities
{
    public class Cart : IEntity
    {
        public Cart()
        {
            UserId = 0;
            SessionId = string.Empty;
            CreateDate = DateTime.UtcNow;
            IsActive = true;
            CartItems = new List<CartItem>();
        }

        public int Id { get; set; }
        
        [Display(Name = "KULLANICI ID")]
        public int? UserId { get; set; }
        
        [Display(Name = "OTURUM ID")]
        public string SessionId { get; set; }
        
        [Display(Name = "AKTİF")]
        public bool IsActive { get; set; }
        
        [Display(Name = "OLUŞTURULMA TARİHİ")]
        public DateTime CreateDate { get; set; }
        
        [Display(Name = "GÜNCELLEME TARİHİ")]
        public DateTime? UpdateDate { get; set; }
        

        public AppUser? User { get; set; }
        public List<CartItem> CartItems { get; set; }
        

        public int TotalItems => CartItems?.Count ?? 0;
        public decimal TotalPrice => CartItems?.Sum(x => x.TotalPrice) ?? 0;
    }
}
