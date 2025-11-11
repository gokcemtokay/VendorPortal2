using VendorPortal.Models.Enums;

namespace VendorPortal.Models.Entities
{
    /// <summary>
    /// Sipariş başlık entity'si
    /// </summary>
    public class SiparisBaslik
    {
        public Guid Id { get; set; }
        public string ReferansNo { get; set; } = string.Empty;
        public SiparisTuru SiparisTuru { get; set; }
        public string? Aciklama { get; set; }
        public DateTime SiparisTarihi { get; set; }
        public DateTime? HedefTeslimatTarihi { get; set; }
        public Guid MusteriFirmaId { get; set; }
        public Guid TedarikciFirmaId { get; set; }
        public string ParaBirimi { get; set; } = "TRY";
        public decimal ToplamTutar { get; set; }
        public SiparisDurumu Durum { get; set; }
        public string? DokumanlarJson { get; set; }
        public string? TeslimatAdresi { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public Guid? CreatedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? ModifiedByUserId { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation Properties
        public virtual Firma MusteriFirma { get; set; } = null!;
        public virtual Firma TedarikciFirma { get; set; } = null!;
        public virtual ApplicationUser? CreatedByUser { get; set; }
        public virtual ApplicationUser? ModifiedByUser { get; set; }
        public virtual ICollection<SiparisKalem> Kalemler { get; set; } = new List<SiparisKalem>();
        public virtual ICollection<SiparisOnayGecmisi> OnayGecmisleri { get; set; } = new List<SiparisOnayGecmisi>();
    }
}
