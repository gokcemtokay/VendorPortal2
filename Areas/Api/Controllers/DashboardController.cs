using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VendorPortal.Models.Enums;

namespace VendorPortal.Areas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardApiController : ControllerBase
    {
        //[HttpGet("GetStats")]
        //public async Task<ActionResult<DashboardStatsDto>> GetStats()
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    var stats = new DashboardStatsDto
        //    {
        //        TotalTeklif = await _context.IhaleTeklifler
        //            .Where(t => t.TedarikciId == userId)
        //            .CountAsync(),

        //        AcikOran = await CalculateAcikOran(),

        //        OnayBekleyen = await _context.SiparisKalemler
        //            .Where(k => k.Durum == SiparisDurumu.OnayBekliyor)
        //            .CountAsync(),

        //        // ... diğer istatistikler
        //    };

        //    return Ok(stats);
        //}
    }
}
