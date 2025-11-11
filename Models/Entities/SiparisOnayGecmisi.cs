using VendorPortal.Models.Enums;

namespace VendorPortal.Models.Entities
{
    /// <summary>
    /// Sipariş onay/revize geçmişi entity'si
    /// </summary>
    public class SiparisOnayGecmisi
    {
        public Guid Id { get; set; }
        public Guid? SiparisBaslikId { get; set; }
        public Guid? SiparisKalemId { get; set; }
        public IslemTipi IslemTipi { get; set; }
        public string? Aciklama { get; set; }
        public int? EskiDurum { get; set; }
        public int? YeniDurum { get; set; }
        public Guid IslemYapanUserId { get; set; }
        public DateTime IslemTarihi { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual SiparisBaslik? SiparisBaslik { get; set; }
        public virtual SiparisKalem? SiparisKalem { get; set; }
        public virtual ApplicationUser IslemYapanUser { get; set; } = null!;
    }
}
