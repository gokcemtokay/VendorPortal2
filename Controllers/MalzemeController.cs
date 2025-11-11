using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using VendorPortal.Models;
using VendorPortal.Models.Entities;
using VendorPortal.Services;

namespace VendorPortal.Controllers
{
    [Authorize]
    public class MalzemeController : Controller
    {
        private readonly IMalzemeService _malzemeService;
        private readonly IFirmaService _firmaService;
        private readonly ILogger<MalzemeController> _logger;

        public MalzemeController(
            IMalzemeService malzemeService,
            IFirmaService firmaService,
            ILogger<MalzemeController> logger)
        {
            _malzemeService = malzemeService;
            _firmaService = firmaService;
            _logger = logger;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out Guid userId) ? userId : Guid.Empty;
        }

        // GET: Malzeme
        public async Task<IActionResult> Index(Guid? firmaId)
        {
            try
            {
                ServiceResult<List<Malzeme>> result;

                if (firmaId.HasValue)
                {
                    result = await _malzemeService.GetMalzemelerByFirmaAsync(firmaId.Value);
                }
                else if (User.IsInRole("Admin"))
                {
                    result = await _malzemeService.GetAllMalzemelerAsync();
                }
                else
                {
                    // Normal kullanıcılar sadece kendi firmalarının malzemelerini görür
                    TempData["InfoMessage"] = "Firma seçiniz.";
                    return View(new List<Malzeme>());
                }

                if (result.Success)
                {
                    return View(result.Data);
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    return View(new List<Malzeme>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Malzemeler listelenirken hata");
                TempData["ErrorMessage"] = "Malzemeler listelenirken bir hata oluştu.";
                return View(new List<Malzeme>());
            }
        }

        // GET: Malzeme/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var result = await _malzemeService.GetMalzemeByIdAsync(id);

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
                _logger.LogError(ex, "Malzeme detayı getirilirken hata");
                TempData["ErrorMessage"] = "Malzeme detayı getirilirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Malzeme/Create
        public async Task<IActionResult> Create()
        {
            await PopulateFirmaDropdown();
            return View();
        }

        // POST: Malzeme/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Malzeme malzeme)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await PopulateFirmaDropdown();
                    return View(malzeme);
                }

                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    return RedirectToAction("Login", "Account");
                }

                var result = await _malzemeService.CreateMalzemeAsync(malzeme, userId);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index), new { firmaId = malzeme.FirmaId });
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    await PopulateFirmaDropdown();
                    return View(malzeme);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Malzeme oluşturulurken hata");
                TempData["ErrorMessage"] = "Malzeme oluşturulurken bir hata oluştu.";
                await PopulateFirmaDropdown();
                return View(malzeme);
            }
        }

        // GET: Malzeme/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var result = await _malzemeService.GetMalzemeByIdAsync(id);

                if (result.Success && result.Data != null)
                {
                    await PopulateFirmaDropdown();
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
                _logger.LogError(ex, "Malzeme düzenlenirken hata");
                TempData["ErrorMessage"] = "Malzeme düzenlenirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Malzeme/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Malzeme malzeme)
        {
            try
            {
                if (id != malzeme.Id)
                {
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    await PopulateFirmaDropdown();
                    return View(malzeme);
                }

                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    return RedirectToAction("Login", "Account");
                }

                var result = await _malzemeService.UpdateMalzemeAsync(malzeme, userId);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index), new { firmaId = malzeme.FirmaId });
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    await PopulateFirmaDropdown();
                    return View(malzeme);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Malzeme güncellenirken hata");
                TempData["ErrorMessage"] = "Malzeme güncellenirken bir hata oluştu.";
                await PopulateFirmaDropdown();
                return View(malzeme);
            }
        }

        // POST: Malzeme/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    return RedirectToAction("Login", "Account");
                }

                var result = await _malzemeService.DeleteMalzemeAsync(id, userId);

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
                _logger.LogError(ex, "Malzeme silinirken hata");
                TempData["ErrorMessage"] = "Malzeme silinirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task PopulateFirmaDropdown()
        {
            var firmaResult = await _firmaService.GetAllFirmalarAsync();
            if (firmaResult.Success && firmaResult.Data != null)
            {
                ViewBag.Firmalar = new SelectList(firmaResult.Data, "Id", "Ad");
            }
        }
    }
}
