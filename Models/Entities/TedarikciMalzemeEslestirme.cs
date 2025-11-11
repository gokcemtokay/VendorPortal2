namespace VendorPortal.Models.Entities
{
    /// <summary>
    /// Müşteri ve Tedarikçi malzemelerinin eşleştirilmesi
    /// </summary>
    public class TedarikciMalzemeEslestirme
    {
        public Guid Id { get; set; }
        public Guid MusteriFirmaId { get; set; }
        public Guid MusteriMalzemeId { get; set; }
        public Guid TedarikciFirmaId { get; set; }
        public Guid TedarikciMalzemeId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public Guid? CreatedByUserId { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation Properties
        public virtual Firma MusteriFirma { get; set; } = null!;
        public virtual Malzeme MusteriMalzeme { get; set; } = null!;
        public virtual Firma TedarikciFirma { get; set; } = null!;
        public virtual Malzeme TedarikciMalzeme { get; set; } = null!;
        public virtual ApplicationUser? CreatedByUser { get; set; }
    }
}
