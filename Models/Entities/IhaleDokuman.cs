namespace VendorPortal.Models.Entities
{
    /// <summary>
    /// İhale dokümanı entity'si
    /// </summary>
    public class IhaleDokuman
    {
        public Guid Id { get; set; }
        public Guid? IhaleId { get; set; }
        public Guid? IhaleTeklifId { get; set; }
        public string DosyaAdi { get; set; } = string.Empty;
        public string DosyaYolu { get; set; } = string.Empty;
        public string? DosyaTipi { get; set; }
        public int BoyutKB { get; set; }
        public Guid YukleyenUserId { get; set; }
        public DateTime YuklemeTarihi { get; set; } = DateTime.UtcNow;
        public string? Aciklama { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation Properties
        public virtual Ihale? Ihale { get; set; }
        public virtual IhaleTeklif? IhaleTeklif { get; set; }
        public virtual ApplicationUser YukleyenUser { get; set; } = null!;
    }
}
