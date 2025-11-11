using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VendorPortal.Models;
using VendorPortal.Models.Entities;
using VendorPortal.Services;

namespace VendorPortal.Controllers
{
    [Authorize]
    public class IhaleController : Controller
    {
        private readonly IIhaleService _ihaleService;
        private readonly ILogger<IhaleController> _logger;

        public IhaleController(IIhaleService ihaleService, ILogger<IhaleController> logger)
        {
            _ihaleService = ihaleService;
            _logger = logger;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out Guid userId) ? userId : Guid.Empty;
        }

        // GET: Ihale
        public async Task<IActionResult> Index()
        {
            try
            {
                ServiceResult<List<Ihale>> result;

                if (User.IsInRole("Admin"))
                {
                    result = await _ihaleService.GetAllIhalelerAsync();
                }
                else
                {
                    // Kullanıcının firmasına göre filtrele (bu örnekte basitleştirilmiş)
                    TempData["InfoMessage"] = "İhaleleriniz listeleniyor.";
                    result = await _ihaleService.GetAllIhalelerAsync();
                }

                if (result.Success)
                {
                    return View(result.Data);
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    return View(new List<Ihale>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhaleler listelenirken hata");
                TempData["ErrorMessage"] = "İhaleler listelenirken bir hata oluştu.";
                return View(new List<Ihale>());
            }
        }

        // GET: Ihale/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var result = await _ihaleService.GetIhaleByIdAsync(id);

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
                _logger.LogError(ex, "İhale detayı getirilirken hata");
                TempData["ErrorMessage"] = "İhale detayı getirilirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Ihale/Create
        [Authorize(Roles = "Musteri,Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ihale/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Musteri,Admin")]
        public async Task<IActionResult> Create(Ihale ihale)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(ihale);
                }

                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    return RedirectToAction("Login", "Account");
                }

                var kalemler = new List<IhaleKalem>();
                var result = await _ihaleService.CreateIhaleAsync(ihale, kalemler, userId);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    return View(ihale);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale oluşturulurken hata");
                TempData["ErrorMessage"] = "İhale oluşturulurken bir hata oluştu.";
                return View(ihale);
            }
        }

        // GET: Ihale/Edit/5
        [Authorize(Roles = "Musteri,Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var result = await _ihaleService.GetIhaleByIdAsync(id);

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
                _logger.LogError(ex, "İhale düzenlenirken hata");
                TempData["ErrorMessage"] = "İhale düzenlenirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Ihale/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Musteri,Admin")]
        public async Task<IActionResult> Edit(Guid id, Ihale ihale)
        {
            try
            {
                if (id != ihale.Id)
                {
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    return View(ihale);
                }

                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    return RedirectToAction("Login", "Account");
                }

                var result = await _ihaleService.UpdateIhaleAsync(ihale, userId);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    return View(ihale);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale güncellenirken hata");
                TempData["ErrorMessage"] = "İhale güncellenirken bir hata oluştu.";
                return View(ihale);
            }
        }

        // POST: Ihale/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Musteri,Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    return RedirectToAction("Login", "Account");
                }

                var result = await _ihaleService.DeleteIhaleAsync(id, userId);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale silinirken hata");
                TempData["ErrorMessage"] = "İhale silinirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Ihale/Yayinla/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Musteri,Admin")]
        public async Task<IActionResult> Yayinla(Guid id)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    return RedirectToAction("Login", "Account");
                }

                var result = await _ihaleService.YayinlaIhaleAsync(id, userId);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }

                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İhale yayınlanırken hata");
                TempData["ErrorMessage"] = "İhale yayınlanırken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Ihale/Teklifler/5
        public async Task<IActionResult> Teklifler(Guid id)
        {
            try
            {
                var result = await _ihaleService.GetTekliflerByIhaleAsync(id);

                if (result.Success)
                {
                    ViewBag.IhaleId = id;
                    return View(result.Data);
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    return RedirectToAction(nameof(Details), new { id });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Teklifler getirilirken hata");
                TempData["ErrorMessage"] = "Teklifler getirilirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Ihale/TeklifVer/5
        [Authorize(Roles = "Tedarikci,Admin")]
        public async Task<IActionResult> TeklifVer(Guid id)
        {
            try
            {
                var ihaleResult = await _ihaleService.GetIhaleByIdAsync(id);

                if (ihaleResult.Success && ihaleResult.Data != null)
                {
                    ViewBag.Ihale = ihaleResult.Data;
                    return View();
                }
                else
                {
                    TempData["ErrorMessage"] = ihaleResult.Message;
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Teklif verme sayfası açılırken hata");
                TempData["ErrorMessage"] = "Teklif verme sayfası açılırken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
