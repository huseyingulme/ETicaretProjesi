using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ETicaret.Core.Entities
{
    public class Order : IEntity
    {
        public Order()
        {
            OrderNumber = string.Empty;
            OrderStatus = OrderStatus.Pending;
            PaymentMethod = PaymentMethod.CreditCard;
            CreateDate = DateTime.UtcNow;
            IsActive = true;
            OrderItems = new List<OrderItem>();
        }

        public int Id { get; set; }
        
        [Display(Name = "SİPARİŞ NUMARASI")]
        public string OrderNumber { get; set; }
        
        [Display(Name = "KULLANICI ID")]
        public int AppUserId { get; set; }
        
        [Display(Name = "ADRES ID")]
        public int AddressId { get; set; }
        
        [Display(Name = "SİPARİŞ DURUMU")]
        public OrderStatus OrderStatus { get; set; }
        
        [Display(Name = "ÖDEME YÖNTEMİ")]
        public PaymentMethod PaymentMethod { get; set; }
        
        [Display(Name = "TOPLAM TUTAR")]
        public decimal TotalAmount { get; set; }
        
        [Display(Name = "KARGO ÜCRETİ")]
        public decimal ShippingCost { get; set; }
        
        [Display(Name = "NOTLAR")]
        public string? Notes { get; set; }
        
        [Display(Name = "AKTİF")]
        public bool IsActive { get; set; }
        
        [Display(Name = "OLUŞTURULMA TARİHİ")]
        public DateTime CreateDate { get; set; }
        
        [Display(Name = "GÜNCELLEME TARİHİ")]
        public DateTime? UpdateDate { get; set; }
        

        public AppUser AppUser { get; set; } = null!;
        public Address Address { get; set; } = null!;
        public List<OrderItem> OrderItems { get; set; }
        

        public int TotalItems => OrderItems?.Count ?? 0;
        public decimal GrandTotal => TotalAmount + ShippingCost;
    }
}
