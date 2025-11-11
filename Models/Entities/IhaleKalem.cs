namespace VendorPortal.Models.Entities
{
    /// <summary>
    /// İhale kalemi entity'si
    /// </summary>
    public class IhaleKalem
    {
        public Guid Id { get; set; }
        public Guid IhaleId { get; set; }
        public Guid MalzemeId { get; set; }
        public string MalzemeKodu { get; set; } = string.Empty;
        public string MalzemeAdi { get; set; } = string.Empty;
        public decimal IstenenMiktar { get; set; }
        public string Birim { get; set; } = string.Empty;
        public string? Spesifikasyon { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public Guid? CreatedByUserId { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation Properties
        public virtual Ihale Ihale { get; set; } = null!;
        public virtual Malzeme Malzeme { get; set; } = null!;
        public virtual ApplicationUser? CreatedByUser { get; set; }
        public virtual ICollection<IhaleTeklifKalem> TeklifKalemleri { get; set; } = new List<IhaleTeklifKalem>();
    }
}
