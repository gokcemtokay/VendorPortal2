using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VendorPortal.Data;

namespace VendorPortal.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.Title = "Vendor Portal - Dashboard";
            
            // Dashboard istatistikleri
            var totalFirmalar = _context.Firmalar.Count(f => !f.IsDeleted);
            var totalSiparisler = _context.SiparisBasliklar.Count(s => !s.IsDeleted);
            var totalIhaleler = _context.Ihaleler.Count(i => !i.IsDeleted);
            
            ViewBag.TotalFirmalar = totalFirmalar;
            ViewBag.TotalSiparisler = totalSiparisler;
            ViewBag.TotalIhaleler = totalIhaleler;
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }
    }
}
