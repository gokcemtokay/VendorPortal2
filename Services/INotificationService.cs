using VendorPortal.Models.DTOs;

namespace VendorPortal.Services
{
    public interface INotificationService
    {
        Task<List<BildirimDto>> GetUserNotificationsAsync(Guid userId, bool sadecOkunmayanlar = false);
        Task<int> GetUnreadCountAsync(Guid userId);
        Task<(bool Success, string Message)> CreateNotificationAsync(CreateBildirimDto dto);
        Task<(bool Success, string Message)> MarkAsReadAsync(Guid bildirimId);
        Task<(bool Success, string Message)> MarkAllAsReadAsync(Guid userId);
        Task<(bool Success, string Message)> DeleteNotificationAsync(Guid bildirimId);
    }
}
