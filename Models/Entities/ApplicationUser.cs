using Microsoft.AspNetCore.Identity;

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

        // Navigation Properties
        public virtual Firma? Firma { get; set; }
    }
}
