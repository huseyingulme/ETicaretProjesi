using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.Core.Entities
{
    public class AppUser : IEntity
    {
        public AppUser()
        {
            Name = string.Empty;
            SurName = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            UserName = string.Empty;
            IsActive = true;
            IsAdmin = false;
            IsDeletable = true;
            CreateDate = DateTime.UtcNow;
            UserGuid = Guid.NewGuid();
        }

        public int Id { get; set; }
        [Display(Name = "AD")]
        public string Name { get; set; }
        [Display(Name = "SOYAD")]
        public string SurName { get; set; } 
        [Display(Name = "E-POSTA")]
        public string Email { get; set; }
        [Display(Name = "TELEFON")]
        public string? Phone { get; set; }
        [Display(Name = "ŞİFRE")]
        public string Password { get; set; }
        [Display(Name = "KULLANICI ADI")]
        public string? UserName { get; set; } 
        [Display(Name = "AKTİF")]
        public bool IsActive { get; set; }
        [Display(Name = "YÖNETİCİ")]
        public bool IsAdmin { get; set; }
        [Display(Name = "SİLİNEBİLİR")]
        public bool IsDeletable { get; set; }
        [Display(Name = "OLUŞTURULMA TARİHİ") , ScaffoldColumn(false)]
        public DateTime CreateDate { get; set; }
        [Display(Name = "GÜNCELLEME TARİHİ") , ScaffoldColumn(false)]
        public Guid? UserGuid { get; set; }
        

        public ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}
