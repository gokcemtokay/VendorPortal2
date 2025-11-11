using VendorPortal.Models.Enums;

namespace VendorPortal.Models.Entities
{
    /// <summary>
    /// İhale teklif kalemi entity'si
    /// </summary>
    public class IhaleTeklifKalem
    {
        public Guid Id { get; set; }
        public Guid IhaleTeklifId { get; set; }
        public Guid IhaleKalemId { get; set; }
        public decimal TeklifEdilenMiktar { get; set; }
        public string Birim { get; set; } = string.Empty;
        public decimal BirimFiyat { get; set; }
        public decimal ToplamFiyat { get; set; }
        public decimal? IndirimOrani { get; set; }
        public int? TeslimSuresiGun { get; set; }
        public string? Notlar { get; set; }
        public TeklifKalemDurumu Durum { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public Guid? CreatedByUserId { get; set; }

        // Navigation Properties
        public virtual IhaleTeklif IhaleTeklif { get; set; } = null!;
        public virtual IhaleKalem IhaleKalem { get; set; } = null!;
        public virtual ApplicationUser? CreatedByUser { get; set; }
    }
}
