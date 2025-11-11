using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VendorPortal.Models.Entities;
using VendorPortal.Services;

namespace VendorPortal.Controllers
{
    [Authorize]
    public class FirmaController : Controller
    {
        private readonly IFirmaService _firmaService;
        private readonly ILogger<FirmaController> _logger;

        public FirmaController(IFirmaService firmaService, ILogger<FirmaController> logger)
        {
            _firmaService = firmaService;
            _logger = logger;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out Guid userId) ? userId : Guid.Empty;
        }

        // GET: Firma
        public async Task<IActionResult> Index()
        {
            try
            {
                var result = await _firmaService.GetAllFirmalarAsync();

                if (result.Success)
                {
                    return View(result.Data);
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    return View(new List<Firma>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Firmalar listelenirken hata");
                TempData["ErrorMessage"] = "Firmalar listelenirken bir hata oluştu.";
                return View(new List<Firma>());
            }
        }

        // GET: Firma/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var result = await _firmaService.GetFirmaByIdAsync(id);

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
                _logger.LogError(ex, "Firma detayı getirilirken hata");
                TempData["ErrorMessage"] = "Firma detayı getirilirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Firma/Create
        [Authorize(Roles = "Admin,Musteri")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Firma/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Musteri")]
        public async Task<IActionResult> Create(Firma firma)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(firma);
                }

                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    return RedirectToAction("Login", "Account");
                }

                var result = await _firmaService.CreateFirmaAsync(firma, userId);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    return View(firma);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Firma oluşturulurken hata");
                TempData["ErrorMessage"] = "Firma oluşturulurken bir hata oluştu.";
                return View(firma);
            }
        }

        // GET: Firma/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var result = await _firmaService.GetFirmaByIdAsync(id);

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
                _logger.LogError(ex, "Firma düzenlenirken hata");
                TempData["ErrorMessage"] = "Firma düzenlenirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Firma/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Firma firma)
        {
            try
            {
                if (id != firma.Id)
                {
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    return View(firma);
                }

                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    return RedirectToAction("Login", "Account");
                }

                var result = await _firmaService.UpdateFirmaAsync(firma, userId);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    return View(firma);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Firma güncellenirken hata");
                TempData["ErrorMessage"] = "Firma güncellenirken bir hata oluştu.";
                return View(firma);
            }
        }

        // POST: Firma/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    return RedirectToAction("Login", "Account");
                }

                var result = await _firmaService.DeleteFirmaAsync(id, userId);

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
                _logger.LogError(ex, "Firma silinirken hata");
                TempData["ErrorMessage"] = "Firma silinirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Firma/Onayla/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Musteri")]
        public async Task<IActionResult> Onayla(Guid id)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    return RedirectToAction("Login", "Account");
                }

                var result = await _firmaService.OnaylaFirmaAsync(id, userId);

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
                _logger.LogError(ex, "Firma onaylanırken hata");
                TempData["ErrorMessage"] = "Firma onaylanırken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Firma/Musteriler
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Musteriler()
        {
            try
            {
                var result = await _firmaService.GetFirmalarByTipAsync(Models.Enums.FirmaTipi.Musteri);

                if (result.Success)
                {
                    ViewBag.Title = "Müşteri Firmalar";
                    return View("Index", result.Data);
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    return View("Index", new List<Firma>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Müşteriler listelenirken hata");
                TempData["ErrorMessage"] = "Müşteriler listelenirken bir hata oluştu.";
                return View("Index", new List<Firma>());
            }
        }

        // GET: Firma/Tedarikciler
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Tedarikciler()
        {
            try
            {
                var result = await _firmaService.GetFirmalarByTipAsync(Models.Enums.FirmaTipi.Tedarikci);

                if (result.Success)
                {
                    ViewBag.Title = "Tedarikçi Firmalar";
                    return View("Index", result.Data);
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    return View("Index", new List<Firma>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tedarikçiler listelenirken hata");
                TempData["ErrorMessage"] = "Tedarikçiler listelenirken bir hata oluştu.";
                return View("Index", new List<Firma>());
            }
        }
    }
}
