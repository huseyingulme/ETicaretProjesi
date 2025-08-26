using System.ComponentModel.DataAnnotations;
using ETicaret.Core.Entities;

namespace ETicaret.Core.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Sipariş Numarası")]
        public string OrderNumber { get; set; } = string.Empty;
        
        [Display(Name = "Sipariş Durumu")]
        public OrderStatus OrderStatus { get; set; }
        
        [Display(Name = "Ödeme Yöntemi")]
        public PaymentMethod PaymentMethod { get; set; }
        
        [Display(Name = "Toplam Tutar")]
        public decimal TotalAmount { get; set; }
        
        [Display(Name = "Kargo Ücreti")]
        public decimal ShippingCost { get; set; }
        
        [Display(Name = "Genel Toplam")]
        public decimal GrandTotal => TotalAmount + ShippingCost;
        
        [Display(Name = "Sipariş Tarihi")]
        public DateTime CreateDate { get; set; }
        
        [Display(Name = "Notlar")]
        public string? Notes { get; set; }
        
        [Display(Name = "Adres")]
        public AddressViewModel Address { get; set; } = new AddressViewModel();
        
        [Display(Name = "Sipariş Ürünleri")]
        public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();
    }

    public class OrderItemViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity;
    }

    public class OrderListViewModel
    {
        public List<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int ConfirmedOrders { get; set; }
        public int ShippedOrders { get; set; }
        public int DeliveredOrders { get; set; }
    }
}
