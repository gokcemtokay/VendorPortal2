using VendorPortal.Models.Enums;

namespace VendorPortal.Models.Entities
{
    /// <summary>
    /// Mail bildirimi entity'si
    /// </summary>
    public class MailBildirimi
    {
        public Guid Id { get; set; }
        public string AliciEmail { get; set; } = string.Empty;
        public string Konu { get; set; } = string.Empty;
        public string Icerik { get; set; } = string.Empty;
        public DateTime GonderimTarihi { get; set; } = DateTime.UtcNow;
        public MailDurumu Durum { get; set; }
        public MailReferansTipi? ReferansTipi { get; set; }
        public Guid? ReferansId { get; set; }
        public string? HataMesaji { get; set; }
    }
}
