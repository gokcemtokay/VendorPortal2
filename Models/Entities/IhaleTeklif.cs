using VendorPortal.Models.Enums;

namespace VendorPortal.Models.Entities
{
    /// <summary>
    /// İhale teklifi entity'si
    /// </summary>
    public class IhaleTeklif
    {
        public Guid Id { get; set; }
        public Guid IhaleId { get; set; }
        public Guid TedarikciFirmaId { get; set; }
        public string TeklifNo { get; set; } = string.Empty;
        public DateTime TeklifTarihi { get; set; } = DateTime.UtcNow;
        public DateTime? GecerlilikTarihi { get; set; }
        public decimal ToplamTeklifTutari { get; set; }
        public TeklifDurumu Durum { get; set; }
        public string? Aciklama { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public Guid? CreatedByUserId { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation Properties
        public virtual Ihale Ihale { get; set; } = null!;
        public virtual Firma TedarikciFirma { get; set; } = null!;
        public virtual ApplicationUser? CreatedByUser { get; set; }
        public virtual ICollection<IhaleTeklifKalem> TeklifKalemleri { get; set; } = new List<IhaleTeklifKalem>();
    }
}
