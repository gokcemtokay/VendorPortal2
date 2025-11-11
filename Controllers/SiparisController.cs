using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VendorPortal.Services;

namespace VendorPortal.Controllers
{
    [Authorize]
    public class SiparisController : Controller
    {
        private readonly ISiparisService _siparisService;
        private readonly ILogger<SiparisController> _logger;

        public SiparisController(ISiparisService siparisService, ILogger<SiparisController> logger)
        {
            _siparisService = siparisService;
            _logger = logger;
        }

        // GET: Siparis
        public async Task<IActionResult> Index()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Kullanıcının firmasına göre siparişleri getir
                // Bu örnekte tüm siparişleri getiriyoruz, gerçek uygulamada firma bazlı filtreleme yapılmalı
                var result = await _siparisService.GetSiparislerByFirmaAsync(userId, true);

                if (result.Success)
                {
                    return View(result.Data);
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    return View(new List<VendorPortal.Models.Entities.SiparisBaslik>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Siparişler listelenirken hata oluştu");
                TempData["ErrorMessage"] = "Siparişler listelenirken bir hata oluştu.";
                return View(new List<VendorPortal.Models.Entities.SiparisBaslik>());
            }
        }

        // GET: Siparis/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var result = await _siparisService.GetSiparisByIdAsync(id);

                if (result.Success && result.Data != null)
                {
                    return View(result.Data);
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sipariş detayı getirilirken hata oluştu");
                TempData["ErrorMessage"] = "Sipariş detayı getirilirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Siparis/Create
        public IActionResult Create()
        {
            return View();
        }

        // GET: Siparis/SatinAlma (Müşteri için satın alma siparişleri)
        [Authorize(Roles = "Musteri,Admin")]
        public IActionResult SatinAlma()
        {
            ViewBag.Title = "Satın Alma Siparişleri";
            return View("Index");
        }

        // GET: Siparis/Satis (Satış siparişleri)
        [Authorize(Roles = "Tedarikci,Admin")]
        public IActionResult Satis()
        {
            ViewBag.Title = "Satış Siparişleri";
            return View("Index");
        }
    }
}
