using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VendorPortal.Models.Entities;
using VendorPortal.Models.Enums;
using VendorPortal.Services;

namespace VendorPortal.Areas.Api.Controllers
{
    [Area("Api")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FirmaApiController : ControllerBase
    {
        private readonly IFirmaService _firmaService;
        private readonly ILogger<FirmaApiController> _logger;

        public FirmaApiController(IFirmaService firmaService, ILogger<FirmaApiController> logger)
        {
            _firmaService = firmaService;
            _logger = logger;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out Guid userId) ? userId : Guid.Empty;
        }

        /// <summary>
        /// Firma oluştur
        /// POST: api/FirmaApi/Create
        /// </summary>
        [HttpPost("Create")]
        [Authorize(Roles = "Admin,Musteri")]
        public async Task<IActionResult> Create([FromBody] Firma firma)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty) return Unauthorized();

                var result = await _firmaService.CreateFirmaAsync(firma, userId);

                if (result.Success)
                    return Ok(new { message = result.Message, data = result.Data });
                else
                    return BadRequest(new { message = result.Message, errors = result.Errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Firma oluşturulurken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// Tüm firmaları getir
        /// GET: api/FirmaApi/GetAll
        /// </summary>
        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _firmaService.GetAllFirmalarAsync();

                if (result.Success)
                    return Ok(new { message = result.Message, data = result.Data });
                else
                    return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Firmalar listelenirken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// Firma detayı getir
        /// GET: api/FirmaApi/Get/{id}
        /// </summary>
        [HttpGet("Get/{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var result = await _firmaService.GetFirmaByIdAsync(id);

                if (result.Success)
                    return Ok(new { message = result.Message, data = result.Data });
                else
                    return NotFound(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Firma getirilirken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// Firma güncelle
        /// PUT: api/FirmaApi/Update
        /// </summary>
        [HttpPut("Update")]
        [Authorize(Roles = "Admin,Musteri,Tedarikci")]
        public async Task<IActionResult> Update([FromBody] Firma firma)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty) return Unauthorized();

                var result = await _firmaService.UpdateFirmaAsync(firma, userId);

                if (result.Success)
                    return Ok(new { message = result.Message, data = result.Data });
                else
                    return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Firma güncellenirken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// Firma sil
        /// DELETE: api/FirmaApi/Delete/{id}
        /// </summary>
        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty) return Unauthorized();

                var result = await _firmaService.DeleteFirmaAsync(id, userId);

                if (result.Success)
                    return Ok(new { message = result.Message });
                else
                    return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Firma silinirken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// Firmayı onayla
        /// POST: api/FirmaApi/Onayla/{id}
        /// </summary>
        [HttpPost("Onayla/{id}")]
        [Authorize(Roles = "Admin,Musteri")]
        public async Task<IActionResult> Onayla(Guid id)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty) return Unauthorized();

                var result = await _firmaService.OnaylaFirmaAsync(id, userId);

                if (result.Success)
                    return Ok(new { message = result.Message });
                else
                    return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Firma onaylanırken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// Tip bazlı firma listesi
        /// GET: api/FirmaApi/GetByTip/{tip}
        /// </summary>
        [HttpGet("GetByTip/{tip}")]
        public async Task<IActionResult> GetByTip(FirmaTipi tip)
        {
            try
            {
                var result = await _firmaService.GetFirmalarByTipAsync(tip);

                if (result.Success)
                    return Ok(new { message = result.Message, data = result.Data });
                else
                    return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Firmalar getirilirken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// Müşterinin tedarikçilerini getir
        /// GET: api/FirmaApi/GetTedarikciler/{musteriId}
        /// </summary>
        [HttpGet("GetTedarikciler/{musteriId}")]
        public async Task<IActionResult> GetTedarikciler(Guid musteriId)
        {
            try
            {
                var result = await _firmaService.GetMusterininTedarikcileriAsync(musteriId);

                if (result.Success)
                    return Ok(new { message = result.Message, data = result.Data });
                else
                    return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tedarikçiler getirilirken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }
    }
}
