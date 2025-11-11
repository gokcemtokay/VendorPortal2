using VendorPortal.Models.Enums;

namespace VendorPortal.Models.Entities
{
    /// <summary>
    /// İhale entity'si
    /// </summary>
    public class Ihale
    {
        public Guid Id { get; set; }
        public string IhaleAdi { get; set; } = string.Empty;
        public string ReferansNo { get; set; } = string.Empty;
        public IhaleTuru IhaleTuru { get; set; }
        public string? Aciklama { get; set; }
        public DateTime TeklifBaslangicTarihi { get; set; }
        public DateTime TeklifBitisTarihi { get; set; }
        public int GecerlilikSuresiGun { get; set; }
        public string ParaBirimi { get; set; } = "TRY";
        public IhaleDurumu Durum { get; set; }
        public bool HerkeseAcikMi { get; set; }
        public bool TumTedarikcilereAcikMi { get; set; }
        public Guid MusteriFirmaId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public Guid? CreatedByUserId { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation Properties
        public virtual Firma MusteriFirma { get; set; } = null!;
        public virtual ApplicationUser? CreatedByUser { get; set; }
        public virtual ICollection<IhaleKalem> Kalemler { get; set; } = new List<IhaleKalem>();
        public virtual ICollection<IhaleDaveti> Davetler { get; set; } = new List<IhaleDaveti>();
        public virtual ICollection<IhaleTeklif> Teklifler { get; set; } = new List<IhaleTeklif>();
        public virtual ICollection<IhaleDokuman> Dokumanlar { get; set; } = new List<IhaleDokuman>();
    }
}
