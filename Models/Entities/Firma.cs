using VendorPortal.Models.Enums;

namespace VendorPortal.Models.Entities
{
    /// <summary>
    /// Firma entity'si (Müşteri ve Tedarikçi firmaları)
    /// </summary>
    public class Firma
    {
        public Guid Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string? VergiNo { get; set; }
        public string? Adres { get; set; }
        public string? Telefon { get; set; }
        public string? Email { get; set; }
        public string? WebSitesi { get; set; }
        public FirmaTipi FirmaTipi { get; set; }
        public FirmaDurumu Durum { get; set; }
        public DateTime? OnayTarihi { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public Guid? CreatedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? ModifiedByUserId { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation Properties
        public virtual ApplicationUser? CreatedByUser { get; set; }
        public virtual ApplicationUser? ModifiedByUser { get; set; }
        public virtual ICollection<ApplicationUser> Kullanicilar { get; set; } = new List<ApplicationUser>();
        public virtual ICollection<FirmaIliskisi> MusteriOlarakIliskiler { get; set; } = new List<FirmaIliskisi>();
        public virtual ICollection<FirmaIliskisi> TedarikciOlarakIliskiler { get; set; } = new List<FirmaIliskisi>();
        public virtual ICollection<Malzeme> Malzemeler { get; set; } = new List<Malzeme>();
        public virtual ICollection<SiparisBaslik> MusteriOlarakSiparisler { get; set; } = new List<SiparisBaslik>();
        public virtual ICollection<SiparisBaslik> TedarikciOlarakSiparisler { get; set; } = new List<SiparisBaslik>();
        public virtual ICollection<Ihale> Ihaleler { get; set; } = new List<Ihale>();
    }
}
