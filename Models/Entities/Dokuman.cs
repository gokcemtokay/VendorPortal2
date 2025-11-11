using VendorPortal.Models.Enums;

namespace VendorPortal.Models.Entities
{
    /// <summary>
    /// Genel doküman yönetimi entity'si
    /// </summary>
    public class Dokuman
    {
        public Guid Id { get; set; }
        public Guid FirmaId { get; set; }
        public DokumanKategorisi Kategori { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public string DosyaAdi { get; set; } = string.Empty;
        public string DosyaYolu { get; set; } = string.Empty;
        public string? DosyaTipi { get; set; }
        public int BoyutKB { get; set; }
        public string? Versiyon { get; set; }
        public Guid YukleyenUserId { get; set; }
        public DateTime YuklemeTarihi { get; set; } = DateTime.UtcNow;
        public DateTime? GecerlilikBitisTarihi { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation Properties
        public virtual Firma Firma { get; set; } = null!;
        public virtual ApplicationUser YukleyenUser { get; set; } = null!;
    }
}
