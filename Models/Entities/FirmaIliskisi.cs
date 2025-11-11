using VendorPortal.Models.Enums;

namespace VendorPortal.Models.Entities
{
    /// <summary>
    /// Müşteri-Tedarikçi ilişkisi entity'si
    /// </summary>
    public class FirmaIliskisi
    {
        public Guid Id { get; set; }
        public Guid MusteriFirmaId { get; set; }
        public Guid TedarikciFirmaId { get; set; }
        public FirmaDurumu Durum { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public Guid? CreatedByUserId { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation Properties
        public virtual Firma MusteriFirma { get; set; } = null!;
        public virtual Firma TedarikciFirma { get; set; } = null!;
        public virtual ApplicationUser? CreatedByUser { get; set; }
    }
}
