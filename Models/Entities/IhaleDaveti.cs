namespace VendorPortal.Models.Entities
{
    /// <summary>
    /// İhale daveti entity'si
    /// </summary>
    public class IhaleDaveti
    {
        public Guid Id { get; set; }
        public Guid IhaleId { get; set; }
        public Guid TedarikciFirmaId { get; set; }
        public DateTime DavetTarihi { get; set; } = DateTime.UtcNow;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public Guid? CreatedByUserId { get; set; }

        // Navigation Properties
        public virtual Ihale Ihale { get; set; } = null!;
        public virtual Firma TedarikciFirma { get; set; } = null!;
        public virtual ApplicationUser? CreatedByUser { get; set; }
    }
}
