using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VeterinaryPrescriptionSystem.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            // Dummy örnek veriler
            ViewBag.TotalPatients = 12;
            ViewBag.TotalMedicines = 8;
            ViewBag.TotalPrescriptions = 21;
            return View();
        }
    }
} 