using Microsoft.EntityFrameworkCore;
using VendorPortal.Data;
using VendorPortal.Models.DTOs;
using VendorPortal.Models.Entities;
using VendorPortal.Models.Enums;

namespace VendorPortal.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            ApplicationDbContext context,
            ILogger<NotificationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<BildirimDto>> GetUserNotificationsAsync(Guid userId, bool sadecOkunmayanlar = false)
        {
            try
            {
                var query = _context.Bildirimler.Where(b => b.UserId == userId);

                if (sadecOkunmayanlar)
                    query = query.Where(b => !b.Okundu);

                return await query
                    .OrderByDescending(b => b.CreatedDate)
                    .Take(50)
                    .Select(b => new BildirimDto
                    {
                        Id = b.Id,
                        Tip = b.Tip,
                        Baslik = b.Baslik,
                        Icerik = b.Icerik,
                        Url = b.Url,
                        Okundu = b.Okundu,
                        CreatedDate = b.CreatedDate
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUserNotificationsAsync error for userId: {UserId}", userId);
                return new List<BildirimDto>();
            }
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            try
            {
                return await _context.Bildirimler
                    .CountAsync(b => b.UserId == userId && !b.Okundu);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUnreadCountAsync error for userId: {UserId}", userId);
                return 0;
            }
        }

        public async Task<(bool Success, string Message)> CreateNotificationAsync(CreateBildirimDto dto)
        {
            try
            {
                var bildirim = new Bildirim
                {
                    Id = Guid.NewGuid(),
                    UserId = dto.UserId,
                    Tip = dto.Tip,
                    Baslik = dto.Baslik,
                    Icerik = dto.Icerik,
                    IlgiliKayitId = dto.IlgiliKayitId,
                    IlgiliKayitTipi = dto.IlgiliKayitTipi,
                    Url = dto.Url,
                    Okundu = false,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Bildirimler.Add(bildirim);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Notification created for user {UserId}: {Baslik}", dto.UserId, dto.Baslik);
                return (true, "Bildirim oluşturuldu.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateNotificationAsync error");
                return (false, "Bildirim oluşturulamadı.");
            }
        }

        public async Task<(bool Success, string Message)> MarkAsReadAsync(Guid bildirimId)
        {
            try
            {
                var bildirim = await _context.Bildirimler.FindAsync(bildirimId);
                if (bildirim == null)
                    return (false, "Bildirim bulunamadı.");

                bildirim.Okundu = true;
                bildirim.OkunmaTarihi = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return (true, "Bildirim okundu olarak işaretlendi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MarkAsReadAsync error for bildirimId: {BildirimId}", bildirimId);
                return (false, "Bildirim güncellenemedi.");
            }
        }

        public async Task<(bool Success, string Message)> MarkAllAsReadAsync(Guid userId)
        {
            try
            {
                var bildirimler = await _context.Bildirimler
                    .Where(b => b.UserId == userId && !b.Okundu)
                    .ToListAsync();

                if (bildirimler.Count == 0)
                    return (true, "Okunmamış bildirim bulunmuyor.");

                foreach (var bildirim in bildirimler)
                {
                    bildirim.Okundu = true;
                    bildirim.OkunmaTarihi = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("{Count} notifications marked as read for user {UserId}", bildirimler.Count, userId);
                return (true, $"{bildirimler.Count} bildirim okundu olarak işaretlendi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MarkAllAsReadAsync error for userId: {UserId}", userId);
                return (false, "Bildirimler güncellenemedi.");
            }
        }

        public async Task<(bool Success, string Message)> DeleteNotificationAsync(Guid bildirimId)
        {
            try
            {
                var bildirim = await _context.Bildirimler.FindAsync(bildirimId);
                if (bildirim == null)
                    return (false, "Bildirim bulunamadı.");

                _context.Bildirimler.Remove(bildirim);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Notification deleted: {BildirimId}", bildirimId);
                return (true, "Bildirim silindi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteNotificationAsync error for bildirimId: {BildirimId}", bildirimId);
                return (false, "Bildirim silinemedi.");
            }
        }
    }
}
