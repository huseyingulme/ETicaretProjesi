using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.Core.Entities
{
    public class Contact : IEntity
    {
        public Contact()
        {
            Name = string.Empty;
            Surname = string.Empty;
            Subject = string.Empty;
            Message = string.Empty;
            CreateDate = DateTime.UtcNow;
        }

        public int Id { get; set; }
        [Display(Name = "AD")]
        public string Name { get; set; } 
        [Display(Name = "SOYAD")]
        public string Surname { get; set; }
        [Display(Name = "E-POSTA")]
        public string? Email { get; set; }
        [Display(Name = "TELEFON")]
        public string? Phone { get; set; }
        [Display(Name = "KONU")]
        public string Subject { get; set; }
        [Display(Name = "MESAJ")]
        public string Message { get; set; }
        [Display(Name = "OLUŞTURULMA TARİHİ"), ScaffoldColumn(false)]
        public DateTime CreateDate { get; set; }

    }
}
