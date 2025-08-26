using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.Core.Entities
{
    public class Slider : IEntity
    {
        public Slider()
        {
            Title = string.Empty;
            Description = string.Empty;
            Image = string.Empty;
            Link = string.Empty;
            IsActive = true;
            OrderNo = 1;
            CreateDate = DateTime.UtcNow;
        }

        public int Id { get; set; }
        
        [Display(Name = "BAŞLIK")]
        [Required(ErrorMessage = "Başlık alanı zorunludur.")]
        public string Title { get; set; } 
        
        [Display(Name = "AÇIKLAMA")]
        public string? Description { get; set; } 
        
        [Display(Name = "RESİM")]
        public string Image { get; set; } 
        
        [Display(Name = "LİNK")]
        public string? Link { get; set; }
        
        [Display(Name = "SIRA NO")]
        public int OrderNo { get; set; }
        
        [Display(Name = "AKTİF")]
        public bool IsActive { get; set; }
        
        [Display(Name = "OLUŞTURULMA TARİHİ"), ScaffoldColumn(false)]
        public DateTime CreateDate { get; set; }
    }
}
