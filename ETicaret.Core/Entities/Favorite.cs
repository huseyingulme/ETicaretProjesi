using System.ComponentModel.DataAnnotations;

namespace ETicaret.Core.Entities
{
    public class Favorite : IEntity
    {
        public int Id { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        
        [Required]
        public bool IsActive { get; set; } = true; // Default değer true olarak ayarlandı
        
        // Constructor'da IsActive değerini garanti altına al
        public Favorite()
        {
            IsActive = true;
            CreateDate = DateTime.UtcNow;
        }
    }
}
