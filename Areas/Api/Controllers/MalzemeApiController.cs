using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VendorPortal.Models.Entities;
using VendorPortal.Services;

namespace VendorPortal.Areas.Api.Controllers
{
    [Area("Api")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MalzemeApiController : ControllerBase
    {
        private readonly IMalzemeService _malzemeService;
        private readonly ILogger<MalzemeApiController> _logger;

        public MalzemeApiController(IMalzemeService malzemeService, ILogger<MalzemeApiController> logger)
        {
            _malzemeService = malzemeService;
            _logger = logger;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out Guid userId) ? userId : Guid.Empty;
        }

        /// <summary>
        /// Malzeme oluştur
        /// POST: api/MalzemeApi/Create
        /// </summary>
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] Malzeme malzeme)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty) return Unauthorized();

                var result = await _malzemeService.CreateMalzemeAsync(malzeme, userId);

                if (result.Success)
                    return Ok(new { message = result.Message, data = result.Data });
                else
                    return BadRequest(new { message = result.Message, errors = result.Errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Malzeme oluşturulurken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// Malzeme detayı
        /// GET: api/MalzemeApi/Get/{id}
        /// </summary>
        [HttpGet("Get/{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var result = await _malzemeService.GetMalzemeByIdAsync(id);

                if (result.Success)
                    return Ok(new { message = result.Message, data = result.Data });
                else
                    return NotFound(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Malzeme getirilirken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// Firma bazlı malzeme listesi
        /// GET: api/MalzemeApi/GetByFirma/{firmaId}
        /// </summary>
        [HttpGet("GetByFirma/{firmaId}")]
        public async Task<IActionResult> GetByFirma(Guid firmaId)
        {
            try
            {
                var result = await _malzemeService.GetMalzemelerByFirmaAsync(firmaId);

                if (result.Success)
                    return Ok(new { message = result.Message, data = result.Data });
                else
                    return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Malzemeler getirilirken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// Tüm malzemeleri getir
        /// GET: api/MalzemeApi/GetAll
        /// </summary>
        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _malzemeService.GetAllMalzemelerAsync();

                if (result.Success)
                    return Ok(new { message = result.Message, data = result.Data });
                else
                    return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Malzemeler listelenirken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// Malzeme güncelle
        /// PUT: api/MalzemeApi/Update
        /// </summary>
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] Malzeme malzeme)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty) return Unauthorized();

                var result = await _malzemeService.UpdateMalzemeAsync(malzeme, userId);

                if (result.Success)
                    return Ok(new { message = result.Message, data = result.Data });
                else
                    return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Malzeme güncellenirken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// Malzeme sil
        /// DELETE: api/MalzemeApi/Delete/{id}
        /// </summary>
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty) return Unauthorized();

                var result = await _malzemeService.DeleteMalzemeAsync(id, userId);

                if (result.Success)
                    return Ok(new { message = result.Message });
                else
                    return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Malzeme silinirken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// Malzeme eşleştirme oluştur
        /// POST: api/MalzemeApi/CreateEslestirme
        /// </summary>
        [HttpPost("CreateEslestirme")]
        public async Task<IActionResult> CreateEslestirme([FromBody] EslestirmeRequest request)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty) return Unauthorized();

                var result = await _malzemeService.CreateEslestirmeAsync(
                    request.MusteriMalzemeId, 
                    request.TedarikciMalzemeId, 
                    userId);

                if (result.Success)
                    return Ok(new { message = result.Message, data = result.Data });
                else
                    return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Eşleştirme oluşturulurken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// Firma bazlı eşleştirmeleri getir
        /// GET: api/MalzemeApi/GetEslestirmeler?musteriId={musteriId}&tedarikciId={tedarikciId}
        /// </summary>
        [HttpGet("GetEslestirmeler")]
        public async Task<IActionResult> GetEslestirmeler([FromQuery] Guid musteriId, [FromQuery] Guid tedarikciId)
        {
            try
            {
                var result = await _malzemeService.GetEslestirmelerAsync(musteriId, tedarikciId);

                if (result.Success)
                    return Ok(new { message = result.Message, data = result.Data });
                else
                    return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Eşleştirmeler getirilirken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }
    }

    public class EslestirmeRequest
    {
        public Guid MusteriMalzemeId { get; set; }
        public Guid TedarikciMalzemeId { get; set; }
    }
}
