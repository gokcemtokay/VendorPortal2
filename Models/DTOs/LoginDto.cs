using System.ComponentModel.DataAnnotations;

namespace VendorPortal.Models.DTOs
{
    /// <summary>
    /// Login için DTO
    /// </summary>
    public class LoginDto
    {
        [Required(ErrorMessage = "Email gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre gereklidir")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Register için DTO
    /// </summary>
    public class RegisterDto
    {
        [Required(ErrorMessage = "Email gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre gereklidir")]
        [StringLength(100, ErrorMessage = "Şifre en az {2} karakter olmalıdır", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ad gereklidir")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad gereklidir")]
        public string LastName { get; set; } = string.Empty;
    }
}