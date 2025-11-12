using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VendorPortal.Models.DTOs;
using VendorPortal.Models.Entities;
using VendorPortal.Services;

namespace VendorPortal.Areas.Api.Controllers
{
    [Area("Api")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserManagementApiController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserManagementApiController> _logger;

        public UserManagementApiController(
            IUserManagementService userManagementService,
            UserManager<ApplicationUser> userManager,
            ILogger<UserManagementApiController> logger)
        {
            _userManagementService = userManagementService;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Tüm kullanıcıları getir (sadece Admin)
        /// </summary>
        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userManagementService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllUsers error");
                return StatusCode(500, new { message = "Kullanıcılar yüklenirken hata oluştu" });
            }
        }

        /// <summary>
        /// Firmaya ait kullanıcıları getir
        /// </summary>
        [HttpGet("GetFirmaUsers/{firmaId}")]
        public async Task<IActionResult> GetFirmaUsers(Guid firmaId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                // Yetki kontrolü: Admin veya firma yöneticisi
                var isAdmin = await _userManagementService.IsAdminAsync(userId);
                var isFirmaYoneticisi = await _userManagementService.IsFirmaYoneticisiAsync(userId, firmaId);

                if (!isAdmin && !isFirmaYoneticisi)
                    return Forbid();

                var users = await _userManagementService.GetFirmaUsersAsync(firmaId);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFirmaUsers error");
                return StatusCode(500, new { message = "Kullanıcılar yüklenirken hata oluştu" });
            }
        }

        /// <summary>
        /// Kullanıcı bilgilerini getir
        /// </summary>
        [HttpGet("GetUser/{userId}")]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                // Sadece kendi bilgilerini veya admin tüm kullanıcıları görebilir
                var isAdmin = await _userManagementService.IsAdminAsync(currentUserId);
                if (!isAdmin && currentUserId != userId)
                    return Forbid();

                var user = await _userManagementService.GetUserByIdAsync(userId);
                if (user == null)
                    return NotFound(new { message = "Kullanıcı bulunamadı" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUser error");
                return StatusCode(500, new { message = "Kullanıcı bilgileri yüklenirken hata oluştu" });
            }
        }

        /// <summary>
        /// Yeni kullanıcı oluştur
        /// </summary>
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                // Yetki kontrolü: Admin veya KullaniciYonetimi yetkisi
                var isAdmin = await _userManagementService.IsAdminAsync(userId);
                if (!isAdmin)
                {
                    // En az bir firmada KullaniciYonetimi yetkisi olmalı
                    bool hasYetki = false;
                    foreach (var firmaYetki in dto.FirmaYetkileri)
                    {
                        if (await _userManagementService.HasYetkiAsync(userId, firmaYetki.FirmaId, Models.Enums.FirmaYetkileri.KullaniciYonetimi))
                        {
                            hasYetki = true;
                            break;
                        }
                    }

                    if (!hasYetki)
                        return Forbid();
                }

                var result = await _userManagementService.CreateUserAsync(dto, userId);

                if (result.Success)
                    return Ok(new { message = result.Message, userId = result.UserId });

                return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateUser error");
                return StatusCode(500, new { message = "Kullanıcı oluşturulurken hata oluştu" });
            }
        }

        /// <summary>
        /// Kullanıcı bilgilerini güncelle
        /// </summary>
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                // Yetki kontrolü
                var isAdmin = await _userManagementService.IsAdminAsync(userId);
                if (!isAdmin)
                {
                    // Kendi bilgilerini güncelliyor veya KullaniciYonetimi yetkisi var mı?
                    if (userId != dto.Id)
                    {
                        bool hasYetki = false;
                        foreach (var firmaYetki in dto.FirmaYetkileri)
                        {
                            if (await _userManagementService.HasYetkiAsync(userId, firmaYetki.FirmaId, Models.Enums.FirmaYetkileri.KullaniciYonetimi))
                            {
                                hasYetki = true;
                                break;
                            }
                        }

                        if (!hasYetki)
                            return Forbid();
                    }
                }

                var result = await _userManagementService.UpdateUserAsync(dto, userId);

                if (result.Success)
                    return Ok(new { message = result.Message });

                return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateUser error");
                return StatusCode(500, new { message = "Kullanıcı güncellenirken hata oluştu" });
            }
        }

        /// <summary>
        /// Kullanıcıyı sil (soft delete)
        /// </summary>
        [HttpDelete("DeleteUser/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            try
            {
                var result = await _userManagementService.DeleteUserAsync(userId);

                if (result.Success)
                    return Ok(new { message = result.Message });

                return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteUser error");
                return StatusCode(500, new { message = "Kullanıcı silinirken hata oluştu" });
            }
        }

        /// <summary>
        /// Kullanıcı durumunu aktif/pasif yap
        /// </summary>
        [HttpPost("ToggleActive/{userId}")]
        public async Task<IActionResult> ToggleActive(Guid userId)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                // Yetki kontrolü: Admin veya KullaniciYonetimi yetkisi
                var isAdmin = await _userManagementService.IsAdminAsync(currentUserId);
                if (!isAdmin)
                {
                    var userFirmalar = await _userManagementService.GetUserFirmsAsync(userId);
                    bool hasYetki = false;

                    foreach (var firma in userFirmalar)
                    {
                        if (await _userManagementService.HasYetkiAsync(currentUserId, firma.FirmaId, Models.Enums.FirmaYetkileri.KullaniciYonetimi))
                        {
                            hasYetki = true;
                            break;
                        }
                    }

                    if (!hasYetki)
                        return Forbid();
                }

                var result = await _userManagementService.ToggleUserActiveAsync(userId);

                if (result.Success)
                    return Ok(new { message = result.Message });

                return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ToggleActive error");
                return StatusCode(500, new { message = "Kullanıcı durumu değiştirilirken hata oluştu" });
            }
        }

        /// <summary>
        /// Kullanıcının firmalarını getir
        /// </summary>
        [HttpGet("GetUserFirms/{userId}")]
        public async Task<IActionResult> GetUserFirms(Guid userId)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                // Sadece kendi firmalarını veya admin tüm kullanıcıların firmalarını görebilir
                var isAdmin = await _userManagementService.IsAdminAsync(currentUserId);
                if (!isAdmin && currentUserId != userId)
                    return Forbid();

                var firmalar = await _userManagementService.GetUserFirmsAsync(userId);
                return Ok(firmalar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUserFirms error");
                return StatusCode(500, new { message = "Firma bilgileri yüklenirken hata oluştu" });
            }
        }

        /// <summary>
        /// Varsayılan firmayı değiştir
        /// </summary>
        [HttpPost("SetDefaultFirma")]
        public async Task<IActionResult> SetDefaultFirma([FromBody] ChangeFirmaDto dto)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                // Sadece kendi default firmasını değiştirebilir
                if (currentUserId != dto.UserId)
                    return Forbid();

                var result = await _userManagementService.SetDefaultFirmaAsync(dto.UserId, dto.YeniFirmaId);

                if (result.Success)
                    return Ok(new { message = result.Message });

                return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetDefaultFirma error");
                return StatusCode(500, new { message = "Varsayılan firma değiştirilirken hata oluştu" });
            }
        }

        /// <summary>
        /// Mevcut kullanıcı bilgilerini getir (auth context için)
        /// </summary>
        [HttpGet("GetCurrentUserInfo")]
        public async Task<IActionResult> GetCurrentUserInfo()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var user = await _userManagementService.GetUserByIdAsync(userId);
                var defaultFirmaId = await _userManagementService.GetUserDefaultFirmaIdAsync(userId);

                return Ok(new { user, defaultFirmaId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCurrentUserInfo error");
                return StatusCode(500, new { message = "Kullanıcı bilgileri yüklenirken hata oluştu" });
            }
        }

        /// <summary>
        /// Kullanıcıyı firmaya ekle
        /// </summary>
        [HttpPost("AddUserToFirma")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUserToFirma([FromBody] AddUserToFirmaDto dto)
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var result = await _userManagementService.AddUserToFirmaAsync(
                    dto.UserId,
                    dto.FirmaId,
                    dto.Yetkiler,
                    currentUserId
                );

                if (result.Success)
                    return Ok(new { message = result.Message });

                return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddUserToFirma error");
                return StatusCode(500, new { message = "Kullanıcı firmaya eklenirken hata oluştu" });
            }
        }

        /// <summary>
        /// Kullanıcıyı firmadan çıkar
        /// </summary>
        [HttpPost("RemoveUserFromFirma")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveUserFromFirma([FromBody] RemoveUserFromFirmaDto dto)
        {
            try
            {
                var result = await _userManagementService.RemoveUserFromFirmaAsync(dto.UserId, dto.FirmaId);

                if (result.Success)
                    return Ok(new { message = result.Message });

                return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RemoveUserFromFirma error");
                return StatusCode(500, new { message = "Kullanıcı firmadan çıkarılırken hata oluştu" });
            }
        }
    }

    // Ek DTO'lar (controller için)
    public class AddUserToFirmaDto
    {
        public Guid UserId { get; set; }
        public Guid FirmaId { get; set; }
        public List<VendorPortal.Models.Enums.FirmaYetkileri> Yetkiler { get; set; } = new();
    }

    public class RemoveUserFromFirmaDto
    {
        public Guid UserId { get; set; }
        public Guid FirmaId { get; set; }
    }
}
