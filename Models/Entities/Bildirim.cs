using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VendorPortal.Models.Enums;

namespace VendorPortal.Models.Entities
{
    /// <summary>
    /// Kullanıcı bildirimleri
    /// </summary>
    public class Bildirim
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Bildirimi alan kullanıcı
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Bildirim tipi
        /// </summary>
        [Required]
        public BildirimTipi Tip { get; set; }

        /// <summary>
        /// Bildirim başlığı
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Baslik { get; set; } = string.Empty;

        /// <summary>
        /// Bildirim içeriği
        /// </summary>
        [MaxLength(1000)]
        public string? Icerik { get; set; }

        /// <summary>
        /// İlgili kayıt ID'si (Sipariş, İhale, vb.)
        /// </summary>
        public Guid? IlgiliKayitId { get; set; }

        /// <summary>
        /// İlgili kayıt tipi (Siparis, Ihale, Firma, vb.)
        /// </summary>
        [MaxLength(50)]
        public string? IlgiliKayitTipi { get; set; }

        /// <summary>
        /// Yönlendirilecek URL
        /// </summary>
        [MaxLength(500)]
        public string? Url { get; set; }

        /// <summary>
        /// Okundu mu?
        /// </summary>
        public bool Okundu { get; set; } = false;

        /// <summary>
        /// Okunma tarihi
        /// </summary>
        public DateTime? OkunmaTarihi { get; set; }

        /// <summary>
        /// Oluşturulma tarihi
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
