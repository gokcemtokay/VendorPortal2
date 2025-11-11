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
    public class IhaleApiController : ControllerBase
    {
        private readonly IIhaleService _ihaleService;
        private readonly ILogger<IhaleApiController> _logger;

        public IhaleApiController(IIhaleService ihaleService, ILogger<IhaleApiController> logger)
        {
            _ihaleService = ihaleService;
            _logger = logger;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out Guid userId) ? userId : Guid.Empty;
        }

        /// <summary>
        /// İhale oluştur
        /// POST: api/IhaleApi/Create
        /// </summary>
        [HttpPost("Create")]
        [Authorize(Roles = "Musteri,Admin")]
        public async Task<IActionResult> Create([FromBody] IhaleCreateRequest request)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty) return Unauthorized();

                var result = await _ihaleService.CreateIhaleAsync(request.Ihale, request.Kalemler, userId);

                if (result.Success)
                    return Ok(new { message = result.Message, data = result.Data });
                else
                    return BadRequest(new { message = result.Message, errors = result.Errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale oluşturulurken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// İhale detayı
        /// GET: api/IhaleApi/Get/{id}
        /// </summary>
        [HttpGet("Get/{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var result = await _ihaleService.GetIhaleByIdAsync(id);

                if (result.Success)
                    return Ok(new { message = result.Message, data = result.Data });
                else
                    return NotFound(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale getirilirken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// Tüm ihaleleri getir
        /// GET: api/IhaleApi/GetAll
        /// </summary>
        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _ihaleService.GetAllIhalelerAsync();

                if (result.Success)
                    return Ok(new { message = result.Message, data = result.Data });
                else
                    return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhaleler listelenirken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// Firma bazlı ihaleleri getir
        /// GET: api/IhaleApi/GetByFirma/{firmaId}
        /// </summary>
        [HttpGet("GetByFirma/{firmaId}")]
        public async Task<IActionResult> GetByFirma(Guid firmaId)
        {
            try
            {
                var result = await _ihaleService.GetIhalelerByFirmaAsync(firmaId);

                if (result.Success)
                    return Ok(new { message = result.Message, data = result.Data });
                else
                    return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhaleler getirilirken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// İhale güncelle
        /// PUT: api/IhaleApi/Update
        /// </summary>
        [HttpPut("Update")]
        [Authorize(Roles = "Musteri,Admin")]
        public async Task<IActionResult> Update([FromBody] Ihale ihale)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty) return Unauthorized();

                var result = await _ihaleService.UpdateIhaleAsync(ihale, userId);

                if (result.Success)
                    return Ok(new { message = result.Message, data = result.Data });
                else
                    return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale güncellenirken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// İhale sil
        /// DELETE: api/IhaleApi/Delete/{id}
        /// </summary>
        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Musteri,Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty) return Unauthorized();

                var result = await _ihaleService.DeleteIhaleAsync(id, userId);

                if (result.Success)
                    return Ok(new { message = result.Message });
                else
                    return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale silinirken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// İhale yayınla
        /// POST: api/IhaleApi/Yayinla/{id}
        /// </summary>
        [HttpPost("Yayinla/{id}")]
        [Authorize(Roles = "Musteri,Admin")]
        public async Task<IActionResult> Yayinla(Guid id)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty) return Unauthorized();

                var result = await _ihaleService.YayinlaIhaleAsync(id, userId);

                if (result.Success)
                    return Ok(new { message = result.Message });
                else
                    return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale yayınlanırken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// İhale iptal et
        /// POST: api/IhaleApi/IptalEt/{id}
        /// </summary>
        [HttpPost("IptalEt/{id}")]
        [Authorize(Roles = "Musteri,Admin")]
        public async Task<IActionResult> IptalEt(Guid id, [FromBody] IptalRequest request)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty) return Unauthorized();

                var result = await _ihaleService.IptalEtIhaleAsync(id, request.IptalNedeni, userId);

                if (result.Success)
                    return Ok(new { message = result.Message });
                else
                    return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale iptal edilirken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// İhaleye tedarikçi davet et
        /// POST: api/IhaleApi/DavetEt
        /// </summary>
        [HttpPost("DavetEt")]
        [Authorize(Roles = "Musteri,Admin")]
        public async Task<IActionResult> DavetEt([FromBody] DavetRequest request)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty) return Unauthorized();

                var result = await _ihaleService.CreateDavetAsync(
                    request.IhaleId, 
                    request.TedarikciFirmaId, 
                    userId);

                if (result.Success)
                    return Ok(new { message = result.Message, data = result.Data });
                else
                    return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Davet oluşturulurken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// İhaleye teklif ver
        /// POST: api/IhaleApi/TeklifVer
        /// </summary>
        [HttpPost("TeklifVer")]
        [Authorize(Roles = "Tedarikci,Admin")]
        public async Task<IActionResult> TeklifVer([FromBody] TeklifRequest request)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty) return Unauthorized();

                var result = await _ihaleService.CreateTeklifAsync(
                    request.Teklif, 
                    request.Kalemler, 
                    userId);

                if (result.Success)
                    return Ok(new { message = result.Message, data = result.Data });
                else
                    return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Teklif oluşturulurken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// İhale tekliflerini getir
        /// GET: api/IhaleApi/GetTeklifler/{ihaleId}
        /// </summary>
        [HttpGet("GetTeklifler/{ihaleId}")]
        public async Task<IActionResult> GetTeklifler(Guid ihaleId)
        {
            try
            {
                var result = await _ihaleService.GetTekliflerByIhaleAsync(ihaleId);

                if (result.Success)
                    return Ok(new { message = result.Message, data = result.Data });
                else
                    return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Teklifler getirilirken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }

        /// <summary>
        /// Teklif onayla
        /// POST: api/IhaleApi/OnaylaTeklif/{teklifId}
        /// </summary>
        [HttpPost("OnaylaTeklif/{teklifId}")]
        [Authorize(Roles = "Musteri,Admin")]
        public async Task<IActionResult> OnaylaTeklif(Guid teklifId)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty) return Unauthorized();

                var result = await _ihaleService.OnaylaTeklifAsync(teklifId, userId);

                if (result.Success)
                    return Ok(new { message = result.Message });
                else
                    return BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Teklif onaylanırken hata");
                return StatusCode(500, new { message = "Sunucu hatası", error = ex.Message });
            }
        }
    }

    public class IhaleCreateRequest
    {
        public Ihale Ihale { get; set; } = null!;
        public List<IhaleKalem> Kalemler { get; set; } = new();
    }

    public class DavetRequest
    {
        public Guid IhaleId { get; set; }
        public Guid TedarikciFirmaId { get; set; }
    }

    public class TeklifRequest
    {
        public IhaleTeklif Teklif { get; set; } = null!;
        public List<IhaleTeklifKalem> Kalemler { get; set; } = new();
    }

    public class IptalRequest
    {
        public string IptalNedeni { get; set; } = string.Empty;
    }
}
