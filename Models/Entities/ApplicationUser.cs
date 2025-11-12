using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendorPortal.Models.Entities
{
    /// <summary>
    /// Uygulama kullanıcısı (Identity'den türetilmiş)
    /// </summary>
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Guid? FirmaId { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // ⭐ YENİ: Navigation Properties
        public virtual Firma? Firma { get; set; }

        // ⭐ YENİ: Multi-firma ilişkisi
        public virtual ICollection<UserFirmaYetkisi> FirmaYetkileri { get; set; } = new List<UserFirmaYetkisi>();

        // ⭐ YENİ: Bildirimler
        public virtual ICollection<Bildirim> Bildirimler { get; set; } = new List<Bildirim>();

        // ⭐ YENİ: Helper property
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
