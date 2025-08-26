using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.Core.Entities
{
    public class Category : IEntity
    {
        public Category()
        {
            Name = string.Empty;
            Description = string.Empty;
            IsActive = true;
            IsTopMenu = false;
            OrderNo = 1;
            CreateDate = DateTime.UtcNow;
            Products = new List<Product>();
        }

        public int Id { get; set; }
        [Display(Name = "KATEGORİ ADI")]
        public string Name { get; set; }
        [Display(Name = "KATEGORİ AÇIKLAMASI")]
        public string Description { get; set; }
        [Display(Name = "AKTİF")]
        public bool IsActive { get; set; }
        [Display(Name = "ÜST MENÜ")]
        public bool IsTopMenu { get; set; }
        [Display(Name = "ÜST KATEGORİ")]
        public int? ParentId { get; set; }
        [Display(Name = "SIRA NO")]
        public int OrderNo { get; set; }
        [Display(Name = "OLUŞTURULMA TARİHİ"), ScaffoldColumn(false)]
        public DateTime CreateDate { get; set; }
        public IList<Product> Products { get; set; } 
    }
}
