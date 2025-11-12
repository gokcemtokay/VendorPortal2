using VendorPortal.Models.DTOs;
using VendorPortal.Models.Enums;

namespace VendorPortal.Services
{
    public interface IUserManagementService
    {
        // Kullanıcı İşlemleri
        Task<UserListDto?> GetUserByIdAsync(Guid userId);
        Task<List<UserListDto>> GetAllUsersAsync();
        Task<List<UserListDto>> GetFirmaUsersAsync(Guid firmaId);
        Task<(bool Success, string Message, Guid? UserId)> CreateUserAsync(CreateUserDto dto, Guid createdByUserId);
        Task<(bool Success, string Message)> UpdateUserAsync(UpdateUserDto dto, Guid updatedByUserId);
        Task<(bool Success, string Message)> DeleteUserAsync(Guid userId);
        Task<(bool Success, string Message)> ToggleUserActiveAsync(Guid userId);

        // Firma Yetkileri
        Task<List<UserFirmaListDto>> GetUserFirmsAsync(Guid userId);
        Task<(bool Success, string Message)> AddUserToFirmaAsync(Guid userId, Guid firmaId, List<FirmaYetkileri> yetkiler, Guid createdByUserId);
        Task<(bool Success, string Message)> RemoveUserFromFirmaAsync(Guid userId, Guid firmaId);
        Task<(bool Success, string Message)> UpdateUserFirmaYetkileriAsync(Guid userId, Guid firmaId, List<FirmaYetkileri> yetkiler);
        Task<(bool Success, string Message)> SetDefaultFirmaAsync(Guid userId, Guid firmaId);
        Task<Guid?> GetUserDefaultFirmaIdAsync(Guid userId);

        // Yetki Kontrolü
        Task<bool> HasYetkiAsync(Guid userId, Guid firmaId, FirmaYetkileri yetki);
        Task<bool> IsAdminAsync(Guid userId);
        Task<bool> IsFirmaYoneticisiAsync(Guid userId, Guid firmaId);
    }
}
