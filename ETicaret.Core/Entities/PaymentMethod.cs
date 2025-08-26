using System.ComponentModel.DataAnnotations;

namespace ETicaret.Core.Entities
{
    public enum PaymentMethod
    {
        [Display(Name = "Kredi Kartı")]
        CreditCard = 0,
        
        [Display(Name = "Banka Kartı")]
        DebitCard = 1,
        
        [Display(Name = "Kapıda Ödeme")]
        CashOnDelivery = 2,
        
        [Display(Name = "Havale/EFT")]
        BankTransfer = 3,
        
        [Display(Name = "Dijital Cüzdan")]
        DigitalWallet = 4
    }
}
