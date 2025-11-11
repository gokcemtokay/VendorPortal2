using System.ComponentModel.DataAnnotations;

namespace VendorPortal.Models.DTOs
{
    /// <summary>
    /// Sipariş başlık DTO
    /// </summary>
    public class SiparisBaslikDto
    {
        public string SiparisNo { get; set; } = string.Empty;
        public DateTime SiparisTarihi { get; set; }
        public string MusteriVergiNo { get; set; } = string.Empty;
        public string TedarikciVergiNo { get; set; } = string.Empty;
        public string ParaBirimi { get; set; } = "TRY";
        public int SiparisTipi { get; set; }
        public string? TeslimatAdresi { get; set; }
        public DateTime KayitTarihi { get; set; }
        public List<SiparisKalemDto> Kalemler { get; set; } = new();
    }

    /// <summary>
    /// Sipariş kalemi DTO
    /// </summary>
    public class SiparisKalemDto
    {
        public int KalemNo { get; set; }
        public string MalzemeNum { get; set; } = string.Empty;
        public decimal Miktar { get; set; }
        public decimal Fiyat { get; set; }
        public decimal IndirimTutari { get; set; }
        public DateTime? IstenenTeslimTarihi { get; set; }
        public string? MusteriNotu { get; set; }
        public string? TedarikciNotu { get; set; }
        public DateTime KayitTarihi { get; set; }
        public SiparisKalemVaryantBilgisiDto? VaryantBilgisi { get; set; }
    }

    /// <summary>
    /// Varyant bilgisi DTO
    /// </summary>
    public class SiparisKalemVaryantBilgisiDto
    {
        [Required(ErrorMessage = "Varyant sınıf kodu zorunludur.")]
        public string VaryantSinifKodu { get; set; } = string.Empty;

        public List<VaryantOzellikDto> VaryantOzellikleri { get; set; } = new();
    }

    /// <summary>
    /// Varyant özellik DTO
    /// </summary>
    public class VaryantOzellikDto
    {
        [Required(ErrorMessage = "Özellik kodu zorunludur.")]
        public string OzellikKodu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Değer zorunludur.")]
        public string Deger { get; set; } = string.Empty;
    }

    /// <summary>
    /// Toplu sipariş import DTO
    /// </summary>
    public class SiparislerDto
    {
        public List<SiparisBaslikDto> Siparisler { get; set; } = new();
    }
}
