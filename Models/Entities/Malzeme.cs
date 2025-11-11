namespace VendorPortal.Models.Entities
{
    /// <summary>
    /// Malzeme/Hizmet entity'si
    /// </summary>
    public class Malzeme
    {
        public Guid Id { get; set; }
        public Guid FirmaId { get; set; }
        public string Kod { get; set; } = string.Empty;
        public string Ad { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public string? Birim { get; set; }
        public decimal? Fiyat { get; set; }
        public string? ParaBirimi { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public Guid? CreatedByUserId { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation Properties
        public virtual Firma Firma { get; set; } = null!;
        public virtual ApplicationUser? CreatedByUser { get; set; }
        public virtual ICollection<TedarikciMalzemeEslestirme> MusteriEslestirmeleri { get; set; } = new List<TedarikciMalzemeEslestirme>();
        public virtual ICollection<TedarikciMalzemeEslestirme> TedarikciEslestirmeleri { get; set; } = new List<TedarikciMalzemeEslestirme>();
        public virtual ICollection<SiparisKalem> SiparisKalemleri { get; set; } = new List<SiparisKalem>();
        public virtual ICollection<IhaleKalem> IhaleKalemleri { get; set; } = new List<IhaleKalem>();
    }
}
