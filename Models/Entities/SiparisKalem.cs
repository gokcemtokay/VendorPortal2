using VendorPortal.Models.Enums;

namespace VendorPortal.Models.Entities
{
    /// <summary>
    /// Sipariş kalemi entity'si
    /// </summary>
    public class SiparisKalem
    {
        public Guid Id { get; set; }
        public Guid SiparisBaslikId { get; set; }
        public Guid MalzemeId { get; set; }
        public string MalzemeKodu { get; set; } = string.Empty;
        public string MalzemeAdi { get; set; } = string.Empty;
        public decimal Miktar { get; set; }
        public string Birim { get; set; } = string.Empty;
        public decimal BirimFiyat { get; set; }
        public decimal ToplamFiyat { get; set; }
        public string? Aciklama { get; set; }
        public DateTime? IstenenTeslimatTarihi { get; set; }
        public decimal? OnaylananMiktar { get; set; }
        public decimal? OnaylananBirimFiyat { get; set; }
        public SiparisKalemDurumu Durum { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public Guid? CreatedByUserId { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation Properties
        public virtual SiparisBaslik SiparisBaslik { get; set; } = null!;
        public virtual Malzeme Malzeme { get; set; } = null!;
        public virtual ApplicationUser? CreatedByUser { get; set; }
        public virtual ICollection<SiparisOnayGecmisi> OnayGecmisleri { get; set; } = new List<SiparisOnayGecmisi>();
    }
}
