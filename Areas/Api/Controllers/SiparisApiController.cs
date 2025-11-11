using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VendorPortal.Models.DTOs;
using VendorPortal.Services;
using VendorPortal.Workers;

namespace VendorPortal.Areas.Api.Controllers
{
    [Area("Api")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SiparisApiController : ControllerBase
    {
        private readonly ISiparisService _siparisService;
        private readonly GenericImportWorker _importWorker;
        private readonly ILogger<SiparisApiController> _logger;

        public SiparisApiController(
            ISiparisService siparisService,
            GenericImportWorker importWorker,
            ILogger<SiparisApiController> logger)
        {
            _siparisService = siparisService;
            _importWorker = importWorker;
            _logger = logger;
        }

        /// <summary>
        /// Toplu sipariş import (JSON)
        /// POST: api/SiparisApi/PostSiparisler
        /// </summary>
        [HttpPost("PostSiparisler")]
        public async Task<IActionResult> PostSiparisler([FromBody] SiparislerDto request)
        {
            try
            {
                if (request?.Siparisler == null || !request.Siparisler.Any())
                {
                    return BadRequest(new
                    {
                        message = "Sipariş listesi boş olamaz.",
                        error = "Geçerli sipariş verisi gönderilmedi."
                    });
                }

                // Kullanıcı ID'sini al
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(new { message = "Geçersiz kullanıcı." });
                }

                _logger.LogInformation($"Toplu sipariş import isteği alındı. Sipariş sayısı: {request.Siparisler.Count}");

                // Import işlemini başlat
                var result = await _siparisService.BulkCreateSiparislerAsync(request.Siparisler, userId);

                if (result.Success)
                {
                    return Ok(new
                    {
                        message = result.Message,
                        data = new
                        {
                            totalCount = request.Siparisler.Count,
                            successCount = result.Data?.Count ?? 0,
                            failedCount = request.Siparisler.Count - (result.Data?.Count ?? 0)
                        }
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        message = result.Message,
                        errors = result.Errors
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PostSiparisler metodunda hata oluştu");
                return StatusCode(500, new
                {
                    message = "Sunucu hatası oluştu.",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Tek sipariş oluşturma
        /// POST: api/SiparisApi/CreateSiparis
        /// </summary>
        [HttpPost("CreateSiparis")]
        public async Task<IActionResult> CreateSiparis([FromBody] SiparisBaslikDto request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(new { message = "Geçersiz kullanıcı." });
                }

                var result = await _siparisService.CreateSiparisAsync(request, userId);

                if (result.Success)
                {
                    return Ok(new { message = result.Message, data = result.Data });
                }
                else
                {
                    return BadRequest(new { message = result.Message, errors = result.Errors });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateSiparis metodunda hata oluştu");
                return StatusCode(500, new { message = "Sunucu hatası oluştu.", error = ex.Message });
            }
        }

        /// <summary>
        /// Sipariş detayı getir
        /// GET: api/SiparisApi/GetSiparis/{id}
        /// </summary>
        [HttpGet("GetSiparis/{id}")]
        public async Task<IActionResult> GetSiparis(Guid id)
        {
            try
            {
                var result = await _siparisService.GetSiparisByIdAsync(id);

                if (result.Success)
                {
                    return Ok(new { message = result.Message, data = result.Data });
                }
                else
                {
                    return NotFound(new { message = result.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetSiparis metodunda hata oluştu");
                return StatusCode(500, new { message = "Sunucu hatası oluştu.", error = ex.Message });
            }
        }

        /// <summary>
        /// Firmaya ait siparişleri getir
        /// GET: api/SiparisApi/GetSiparislerByFirma/{firmaId}?isMusteriView=true
        /// </summary>
        [HttpGet("GetSiparislerByFirma/{firmaId}")]
        public async Task<IActionResult> GetSiparislerByFirma(Guid firmaId, [FromQuery] bool isMusteriView = true)
        {
            try
            {
                var result = await _siparisService.GetSiparislerByFirmaAsync(firmaId, isMusteriView);

                if (result.Success)
                {
                    return Ok(new { message = result.Message, data = result.Data });
                }
                else
                {
                    return BadRequest(new { message = result.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetSiparislerByFirma metodunda hata oluştu");
                return StatusCode(500, new { message = "Sunucu hatası oluştu.", error = ex.Message });
            }
        }
    }
}
