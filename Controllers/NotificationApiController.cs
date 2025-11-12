using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VendorPortal.Models.DTOs;
using VendorPortal.Services;

namespace VendorPortal.Areas.Api.Controllers
{
    [Area("Api")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationApiController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationApiController> _logger;

        public NotificationApiController(
            INotificationService notificationService,
            ILogger<NotificationApiController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        /// <summary>
        /// Kullanıcının bildirimlerini getir
        /// </summary>
        [HttpGet("GetMyNotifications")]
        public async Task<IActionResult> GetMyNotifications([FromQuery] bool sadecOkunmayanlar = false)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var notifications = await _notificationService.GetUserNotificationsAsync(userId, sadecOkunmayanlar);

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetMyNotifications error");
                return StatusCode(500, new { message = "Bildirimler yüklenirken hata oluştu" });
            }
        }

        /// <summary>
        /// Okunmamış bildirim sayısını getir
        /// </summary>
        [HttpGet("GetUnreadCount")]
        public async Task<IActionResult> GetUnreadCount()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var count = await _notificationService.GetUnreadCountAsync(userId);

                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUnreadCount error");
                return StatusCode(500, new { message = "Bildirim sayısı alınırken hata oluştu" });
            }
        }

        /// <summary>
        /// Bildirimi okundu olarak işaretle
        /// </summary>
        [HttpPost("MarkAsRead/{id}")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            try
            {
                var result = await _notificationService.MarkAsReadAsync(id);

                if (result.Success)
                    return Ok(new { message = result.Message });

                return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MarkAsRead error");
                return StatusCode(500, new { message = "Bildirim güncellenirken hata oluştu" });
            }
        }

        /// <summary>
        /// Tüm bildirimleri okundu olarak işaretle
        /// </summary>
        [HttpPost("MarkAllAsRead")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _notificationService.MarkAllAsReadAsync(userId);

                if (result.Success)
                    return Ok(new { message = result.Message });

                return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MarkAllAsRead error");
                return StatusCode(500, new { message = "Bildirimler güncellenirken hata oluştu" });
            }
        }

        /// <summary>
        /// Bildirimi sil
        /// </summary>
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _notificationService.DeleteNotificationAsync(id);

                if (result.Success)
                    return Ok(new { message = result.Message });

                return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete notification error");
                return StatusCode(500, new { message = "Bildirim silinirken hata oluştu" });
            }
        }

        /// <summary>
        /// Yeni bildirim oluştur (admin veya sistem için)
        /// </summary>
        [HttpPost("Create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateBildirimDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _notificationService.CreateNotificationAsync(dto);

                if (result.Success)
                    return Ok(new { message = result.Message });

                return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create notification error");
                return StatusCode(500, new { message = "Bildirim oluşturulurken hata oluştu" });
            }
        }
    }
}
