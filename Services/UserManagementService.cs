using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VendorPortal.Data;
using VendorPortal.Models.DTOs;
using VendorPortal.Models.Entities;
using VendorPortal.Models.Enums;

namespace VendorPortal.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserManagementService> _logger;

        public UserManagementService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<UserManagementService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        #region Kullanıcı İşlemleri

        public async Task<UserListDto?> GetUserByIdAsync(Guid userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.FirmaYetkileri)
                        .ThenInclude(fy => fy.Firma)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                    return null;

                return new UserListDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FullName = $"{user.FirstName} {user.LastName}".Trim(),
                    PhoneNumber = user.PhoneNumber ?? "",
                    IsActive = user.IsActive,
                    LastLoginDate = user.LastLoginDate,
                    Firmalar = user.FirmaYetkileri
                        .Where(fy => fy.IsActive)
                        .Select(fy => new UserFirmaListDto
                        {
                            FirmaId = fy.FirmaId,
                            FirmaAd = fy.Firma.Ad,
                            IsDefault = fy.IsDefault,
                            Yetkiler = fy.Yetkiler.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(y => ((FirmaYetkileri)int.Parse(y)).ToString())
                                .ToList()
                        })
                        .ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUserByIdAsync error for userId: {UserId}", userId);
                return null;
            }
        }

        public async Task<List<UserListDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _context.Users
                    .Include(u => u.FirmaYetkileri)
                        .ThenInclude(fy => fy.Firma)
                    .Where(u => u.IsActive)
                    .ToListAsync();

                return users.Select(user => new UserListDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FullName = $"{user.FirstName} {user.LastName}".Trim(),
                    PhoneNumber = user.PhoneNumber ?? "",
                    IsActive = user.IsActive,
                    LastLoginDate = user.LastLoginDate,
                    Firmalar = user.FirmaYetkileri
                        .Where(fy => fy.IsActive)
                        .Select(fy => new UserFirmaListDto
                        {
                            FirmaId = fy.FirmaId,
                            FirmaAd = fy.Firma.Ad,
                            IsDefault = fy.IsDefault,
                            Yetkiler = fy.Yetkiler.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(y => ((FirmaYetkileri)int.Parse(y)).ToString())
                                .ToList()
                        })
                        .ToList()
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllUsersAsync error");
                return new List<UserListDto>();
            }
        }

        public async Task<List<UserListDto>> GetFirmaUsersAsync(Guid firmaId)
        {
            try
            {
                var userIds = await _context.UserFirmaYetkileri
                    .Where(ufy => ufy.FirmaId == firmaId && ufy.IsActive)
                    .Select(ufy => ufy.UserId)
                    .Distinct()
                    .ToListAsync();

                var users = await _context.Users
                    .Include(u => u.FirmaYetkileri)
                        .ThenInclude(fy => fy.Firma)
                    .Where(u => userIds.Contains(u.Id))
                    .ToListAsync();

                return users.Select(user => new UserListDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FullName = $"{user.FirstName} {user.LastName}".Trim(),
                    PhoneNumber = user.PhoneNumber ?? "",
                    IsActive = user.IsActive,
                    LastLoginDate = user.LastLoginDate,
                    Firmalar = user.FirmaYetkileri
                        .Where(fy => fy.FirmaId == firmaId && fy.IsActive)
                        .Select(fy => new UserFirmaListDto
                        {
                            FirmaId = fy.FirmaId,
                            FirmaAd = fy.Firma.Ad,
                            IsDefault = fy.IsDefault,
                            Yetkiler = fy.Yetkiler.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(y => ((FirmaYetkileri)int.Parse(y)).ToString())
                                .ToList()
                        })
                        .ToList()
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFirmaUsersAsync error for firmaId: {FirmaId}", firmaId);
                return new List<UserListDto>();
            }
        }

        public async Task<(bool Success, string Message, Guid? UserId)> CreateUserAsync(CreateUserDto dto, Guid createdByUserId)
        {
            try
            {
                // Email kontrolü
                var existingUser = await _userManager.FindByEmailAsync(dto.Email);
                if (existingUser != null)
                    return (false, "Bu email adresi zaten kullanılıyor.", null);

                // Kullanıcı oluştur
                var user = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    PhoneNumber = dto.PhoneNumber,
                    IsActive = true,
                    EmailConfirmed = true,
                    CreatedDate = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return (false, errors, null);
                }

                // Firma yetkilerini ekle
                foreach (var firmaYetki in dto.FirmaYetkileri)
                {
                    var yetki = new UserFirmaYetkisi
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        FirmaId = firmaYetki.FirmaId,
                        Yetkiler = string.Join(",", firmaYetki.Yetkiler),
                        IsDefault = firmaYetki.IsDefault,
                        IsActive = true,
                        BaslangicTarihi = DateTime.UtcNow,
                        CreatedDate = DateTime.UtcNow,
                        CreatedByUserId = createdByUserId
                    };
                    _context.UserFirmaYetkileri.Add(yetki);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("User created: {Email} by {CreatedBy}", dto.Email, createdByUserId);
                return (true, "Kullanıcı başarıyla oluşturuldu.", user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateUserAsync error");
                return (false, "Kullanıcı oluşturulurken hata oluştu.", null);
            }
        }

        public async Task<(bool Success, string Message)> UpdateUserAsync(UpdateUserDto dto, Guid updatedByUserId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(dto.Id.ToString());
                if (user == null)
                    return (false, "Kullanıcı bulunamadı.");

                // Kullanıcı bilgilerini güncelle
                user.FirstName = dto.FirstName;
                user.LastName = dto.LastName;
                user.PhoneNumber = dto.PhoneNumber;
                user.IsActive = dto.IsActive;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return (false, errors);
                }

                // Mevcut yetkileri kaldır
                var mevcutYetkiler = await _context.UserFirmaYetkileri
                    .Where(ufy => ufy.UserId == dto.Id)
                    .ToListAsync();
                _context.UserFirmaYetkileri.RemoveRange(mevcutYetkiler);

                // Yeni yetkileri ekle
                foreach (var firmaYetki in dto.FirmaYetkileri)
                {
                    var yetki = new UserFirmaYetkisi
                    {
                        Id = firmaYetki.Id ?? Guid.NewGuid(),
                        UserId = dto.Id,
                        FirmaId = firmaYetki.FirmaId,
                        Yetkiler = string.Join(",", firmaYetki.Yetkiler),
                        IsDefault = firmaYetki.IsDefault,
                        IsActive = firmaYetki.IsActive,
                        BaslangicTarihi = DateTime.UtcNow,
                        CreatedDate = DateTime.UtcNow,
                        CreatedByUserId = updatedByUserId
                    };
                    _context.UserFirmaYetkileri.Add(yetki);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("User updated: {UserId} by {UpdatedBy}", dto.Id, updatedByUserId);
                return (true, "Kullanıcı başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateUserAsync error");
                return (false, "Kullanıcı güncellenirken hata oluştu.");
            }
        }

        public async Task<(bool Success, string Message)> DeleteUserAsync(Guid userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    return (false, "Kullanıcı bulunamadı.");

                // Soft delete
                user.IsActive = false;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return (false, errors);
                }

                _logger.LogInformation("User deleted (soft): {UserId}", userId);
                return (true, "Kullanıcı başarıyla silindi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteUserAsync error");
                return (false, "Kullanıcı silinirken hata oluştu.");
            }
        }

        public async Task<(bool Success, string Message)> ToggleUserActiveAsync(Guid userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    return (false, "Kullanıcı bulunamadı.");

                user.IsActive = !user.IsActive;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return (false, errors);
                }

                var status = user.IsActive ? "aktif" : "pasif";
                _logger.LogInformation("User toggled: {UserId} to {Status}", userId, status);
                return (true, $"Kullanıcı {status} duruma getirildi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ToggleUserActiveAsync error");
                return (false, "Kullanıcı durumu değiştirilirken hata oluştu.");
            }
        }

        #endregion

        #region Firma Yetkileri

        public async Task<List<UserFirmaListDto>> GetUserFirmsAsync(Guid userId)
        {
            try
            {
                return await _context.UserFirmaYetkileri
                    .Include(ufy => ufy.Firma)
                    .Where(ufy => ufy.UserId == userId && ufy.IsActive)
                    .Select(ufy => new UserFirmaListDto
                    {
                        FirmaId = ufy.FirmaId,
                        FirmaAd = ufy.Firma.Ad,
                        IsDefault = ufy.IsDefault,
                        Yetkiler = ufy.Yetkiler.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(y => ((FirmaYetkileri)int.Parse(y)).ToString())
                            .ToList()
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUserFirmsAsync error for userId: {UserId}", userId);
                return new List<UserFirmaListDto>();
            }
        }

        public async Task<(bool Success, string Message)> AddUserToFirmaAsync(Guid userId, Guid firmaId, List<FirmaYetkileri> yetkiler, Guid createdByUserId)
        {
            try
            {
                // Kullanıcı zaten bu firmada var mı?
                var mevcutYetki = await _context.UserFirmaYetkileri
                    .FirstOrDefaultAsync(ufy => ufy.UserId == userId && ufy.FirmaId == firmaId && ufy.IsActive);

                if (mevcutYetki != null)
                    return (false, "Kullanıcı zaten bu firmada yetkili.");

                var yetki = new UserFirmaYetkisi
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    FirmaId = firmaId,
                    Yetkiler = string.Join(",", yetkiler.Select(y => (int)y)),
                    IsDefault = false,
                    IsActive = true,
                    BaslangicTarihi = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow,
                    CreatedByUserId = createdByUserId
                };

                _context.UserFirmaYetkileri.Add(yetki);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} added to firma {FirmaId}", userId, firmaId);
                return (true, "Kullanıcı firmaya eklendi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddUserToFirmaAsync error");
                return (false, "Kullanıcı firmaya eklenirken hata oluştu.");
            }
        }

        public async Task<(bool Success, string Message)> RemoveUserFromFirmaAsync(Guid userId, Guid firmaId)
        {
            try
            {
                var yetki = await _context.UserFirmaYetkileri
                    .FirstOrDefaultAsync(ufy => ufy.UserId == userId && ufy.FirmaId == firmaId);

                if (yetki == null)
                    return (false, "Kullanıcının bu firmada yetkisi bulunamadı.");

                // Soft delete
                yetki.IsActive = false;
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} removed from firma {FirmaId}", userId, firmaId);
                return (true, "Kullanıcı firmadan çıkarıldı.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RemoveUserFromFirmaAsync error");
                return (false, "Kullanıcı firmadan çıkarılırken hata oluştu.");
            }
        }

        public async Task<(bool Success, string Message)> UpdateUserFirmaYetkileriAsync(Guid userId, Guid firmaId, List<FirmaYetkileri> yetkiler)
        {
            try
            {
                var yetki = await _context.UserFirmaYetkileri
                    .FirstOrDefaultAsync(ufy => ufy.UserId == userId && ufy.FirmaId == firmaId && ufy.IsActive);

                if (yetki == null)
                    return (false, "Kullanıcının bu firmada yetkisi bulunamadı.");

                yetki.Yetkiler = string.Join(",", yetkiler.Select(y => (int)y));
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} permissions updated for firma {FirmaId}", userId, firmaId);
                return (true, "Yetkiler güncellendi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateUserFirmaYetkileriAsync error");
                return (false, "Yetkiler güncellenirken hata oluştu.");
            }
        }

        public async Task<(bool Success, string Message)> SetDefaultFirmaAsync(Guid userId, Guid firmaId)
        {
            try
            {
                // Tüm firmaların default'unu kaldır
                var tumYetkiler = await _context.UserFirmaYetkileri
                    .Where(ufy => ufy.UserId == userId && ufy.IsActive)
                    .ToListAsync();

                foreach (var yetki in tumYetkiler)
                {
                    yetki.IsDefault = false;
                }

                // Seçilen firmayı default yap
                var secilenYetki = tumYetkiler.FirstOrDefault(ufy => ufy.FirmaId == firmaId);
                if (secilenYetki == null)
                    return (false, "Kullanıcının bu firmada yetkisi bulunamadı.");

                secilenYetki.IsDefault = true;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Default firma set for user {UserId}: {FirmaId}", userId, firmaId);
                return (true, "Varsayılan firma değiştirildi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetDefaultFirmaAsync error");
                return (false, "Varsayılan firma değiştirilirken hata oluştu.");
            }
        }

        public async Task<Guid?> GetUserDefaultFirmaIdAsync(Guid userId)
        {
            try
            {
                var defaultYetki = await _context.UserFirmaYetkileri
                    .FirstOrDefaultAsync(ufy => ufy.UserId == userId && ufy.IsDefault && ufy.IsActive);

                if (defaultYetki != null)
                    return defaultYetki.FirmaId;

                // Default firma yoksa ilk firmayı döndür
                var ilkYetki = await _context.UserFirmaYetkileri
                    .Where(ufy => ufy.UserId == userId && ufy.IsActive)
                    .OrderBy(ufy => ufy.CreatedDate)
                    .FirstOrDefaultAsync();

                return ilkYetki?.FirmaId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUserDefaultFirmaIdAsync error");
                return null;
            }
        }

        #endregion

        #region Yetki Kontrolü

        public async Task<bool> HasYetkiAsync(Guid userId, Guid firmaId, FirmaYetkileri yetki)
        {
            try
            {
                // Admin her zaman yetkili
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
                    return true;

                var userYetki = await _context.UserFirmaYetkileri
                    .FirstOrDefaultAsync(ufy =>
                        ufy.UserId == userId &&
                        ufy.FirmaId == firmaId &&
                        ufy.IsActive);

                if (userYetki == null)
                    return false;

                var yetkiler = userYetki.Yetkiler
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(y => (FirmaYetkileri)int.Parse(y))
                    .ToList();

                // FirmaYoneticisi tüm yetkilere sahip
                return yetkiler.Contains(FirmaYetkileri.FirmaYoneticisi) || yetkiler.Contains(yetki);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HasYetkiAsync error");
                return false;
            }
        }

        public async Task<bool> IsAdminAsync(Guid userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    return false;

                return await _userManager.IsInRoleAsync(user, "Admin");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IsAdminAsync error");
                return false;
            }
        }

        public async Task<bool> IsFirmaYoneticisiAsync(Guid userId, Guid firmaId)
        {
            try
            {
                var userYetki = await _context.UserFirmaYetkileri
                    .FirstOrDefaultAsync(ufy =>
                        ufy.UserId == userId &&
                        ufy.FirmaId == firmaId &&
                        ufy.IsActive);

                if (userYetki == null)
                    return false;

                var yetkiler = userYetki.Yetkiler
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(y => (FirmaYetkileri)int.Parse(y))
                    .ToList();

                return yetkiler.Contains(FirmaYetkileri.FirmaYoneticisi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IsFirmaYoneticisiAsync error");
                return false;
            }
        }

        #endregion
    }
}
