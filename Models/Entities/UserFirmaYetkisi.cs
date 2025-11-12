using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendorPortal.Models.Entities
{
    /// <summary>
    /// Kullanıcı-Firma yetkilendirme ilişkisi
    /// Bir kullanıcı birden fazla firmada yetkili olabilir
    /// </summary>
    public class UserFirmaYetkisi
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid FirmaId { get; set; }

        /// <summary>
        /// Kullanıcının bu firmadaki yetkileri (birden fazla yetki olabilir, JSON formatında saklanır)
        /// Örnek: "1,2,7" -> FirmaYoneticisi, SatinAlma, KullaniciYonetimi
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Yetkiler { get; set; } = string.Empty;

        /// <summary>
        /// Bu firma kullanıcı için varsayılan (aktif) firma mı?
        /// </summary>
        public bool IsDefault { get; set; } = false;

        /// <summary>
        /// Yetki aktif mi?
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Yetki başlangıç tarihi
        /// </summary>
        public DateTime BaslangicTarihi { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Yetki bitiş tarihi (null ise süresiz)
        /// </summary>
        public DateTime? BitisTarihi { get; set; }

        /// <summary>
        /// Oluşturulma tarihi
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Yetkiyi oluşturan kullanıcı
        /// </summary>
        public Guid? CreatedByUserId { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        [ForeignKey("FirmaId")]
        public virtual Firma Firma { get; set; } = null!;

        [ForeignKey("CreatedByUserId")]
        public virtual ApplicationUser? CreatedByUser { get; set; }
    }
}
