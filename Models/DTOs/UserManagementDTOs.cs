using VendorPortal.Models.Enums;

namespace VendorPortal.Models.DTOs
{
    /// <summary>
    /// Kullanıcı oluşturma DTO
    /// </summary>
    public class CreateUserDto
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public List<UserFirmaYetkisiDto> FirmaYetkileri { get; set; } = new();
    }

    /// <summary>
    /// Kullanıcı güncelleme DTO
    /// </summary>
    public class UpdateUserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<UserFirmaYetkisiDto> FirmaYetkileri { get; set; } = new();
    }

    /// <summary>
    /// Kullanıcı-Firma yetki DTO
    /// </summary>
    public class UserFirmaYetkisiDto
    {
        public Guid? Id { get; set; }
        public Guid FirmaId { get; set; }
        public string FirmaAd { get; set; } = string.Empty;
        public List<int> Yetkiler { get; set; } = new();
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Kullanıcı listesi için DTO
    /// </summary>
    public class UserListDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public List<UserFirmaListDto> Firmalar { get; set; } = new();
    }

    /// <summary>
    /// Kullanıcının firmalarını listeleme için
    /// </summary>
    public class UserFirmaListDto
    {
        public Guid FirmaId { get; set; }
        public string FirmaAd { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public List<string> Yetkiler { get; set; } = new();
    }

    /// <summary>
    /// Aktif firma değiştirme DTO
    /// </summary>
    public class ChangeFirmaDto
    {
        public Guid UserId { get; set; }
        public Guid YeniFirmaId { get; set; }
    }

    /// <summary>
    /// Bildirim DTO
    /// </summary>
    public class BildirimDto
    {
        public Guid Id { get; set; }
        public BildirimTipi Tip { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string? Icerik { get; set; }
        public string? Url { get; set; }
        public bool Okundu { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    /// <summary>
    /// Bildirim oluşturma DTO
    /// </summary>
    public class CreateBildirimDto
    {
        public Guid UserId { get; set; }
        public BildirimTipi Tip { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string? Icerik { get; set; }
        public Guid? IlgiliKayitId { get; set; }
        public string? IlgiliKayitTipi { get; set; }
        public string? Url { get; set; }
    }
}
