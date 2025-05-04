using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VeterinaryPrescriptionSystem.Web.Data;
using VeterinaryPrescriptionSystem.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using VeterinaryPrescriptionSystem.Web.Models.ViewModels;

namespace VeterinaryPrescriptionSystem.Web.Controllers
{
    [Authorize]
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<PatientsController> _logger;

        public PatientsController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            ILogger<PatientsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                    return Challenge();

                var patients = await _context.Patients
                    .Where(p => p.VeterinarianId == userId)
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                return View(patients);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Hastalar listelenirken bir hata oluştu: {ex}");
                TempData["Error"] = "Hastalar listelenirken bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
                return View(new List<Patient>());
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);
            var patient = await _context.Patients
                .Include(p => p.Special)
                .Include(p => p.VaccineRecords)
                .Include(p => p.Examinations)
                .Where(p => p.VeterinarianId == userId && p.Id == id)
                .FirstOrDefaultAsync();

            if (patient == null)
                return NotFound();

            var viewModel = new PatientDetailViewModel
            {
                Patient = patient,
                Special = patient.Special,
                VaccineRecords = patient.VaccineRecords.ToList(),
                Examinations = patient.Examinations.ToList()
            };

            return View(viewModel);
        }

        public IActionResult Create()
        {
            // Yeni bir hasta nesnesi oluştur ve VeterinarianId'yi şimdiden ata
            var userId = _userManager.GetUserId(User);
            var patient = new Patient
            {
                VeterinarianId = userId
            };
            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient patient)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "Kullanıcı oturumu bulunamadı!");
                return View(patient);
            }

            // Bu satır önemli - VeterinarianId için ModelState doğrulamasını kaldırıyoruz
            ModelState.Remove("VeterinarianId");
            ModelState.Remove("Veterinarian"); // Veterinarian nesnesinin doğrulamasını da kaldır

            // Veteriner ID'sini şu anki kullanıcının ID'sine ayarlıyoruz
            patient.VeterinarianId = userId;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(patient);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Hasta başarıyla kaydedildi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Hasta kaydedilirken bir hata oluştu: {ex}");
                    ModelState.AddModelError("", "Hasta kaydedilirken bir hata oluştu: " + ex.Message);
                }
            }
            else
            {
                // ModelState hatalarını loglama
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _logger.LogError($"ModelState hatası: {error.ErrorMessage}");
                    }
                }
            }

            return View(patient);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);
            var patient = await _context.Patients
                .Where(p => p.VeterinarianId == userId && p.Id == id)
                .FirstOrDefaultAsync();

            if (patient == null)
                return NotFound();

            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Patient patient)
        {
            if (id != patient.Id)
                return NotFound();

            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                    return Challenge();

                // Veterinarian ile ilgili kontrolleri kaldır
                ModelState.Remove("VeterinarianId");
                ModelState.Remove("Veterinarian");

                // Veteriner ID'sini ayarla
                patient.VeterinarianId = userId;

                if (ModelState.IsValid)
                {
                    var existingPatient = await _context.Patients
                        .Where(p => p.VeterinarianId == userId && p.Id == id)
                        .FirstOrDefaultAsync();

                    if (existingPatient == null)
                        return NotFound();

                    existingPatient.Name = patient.Name;
                    existingPatient.Age = patient.Age;
                    existingPatient.Species = patient.Species;
                    existingPatient.Breed = patient.Breed;
                    existingPatient.OwnerName = patient.OwnerName;
                    existingPatient.OwnerPhone = patient.OwnerPhone;
                    existingPatient.OwnerEmail = patient.OwnerEmail;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Hasta bilgileri başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientExists(patient.Id))
                    return NotFound();
                else
                    throw;
            }

            return View(patient);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);
            var patient = await _context.Patients
                .Where(p => p.VeterinarianId == userId && p.Id == id)
                .FirstOrDefaultAsync();

            if (patient == null)
                return NotFound();

            return View(patient);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var patient = await _context.Patients
                .Where(p => p.VeterinarianId == userId && p.Id == id)
                .FirstOrDefaultAsync();

            if (patient != null)
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Hasta başarıyla silindi.";
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> FixAllPatientsOwner()
        {
            var userId = _userManager.GetUserId(User);
            var allPatients = await _context.Patients.ToListAsync();
            foreach (var patient in allPatients)
            {
                patient.VeterinarianId = userId;
            }
            await _context.SaveChangesAsync();
            TempData["Success"] = "Tüm hastalar giriş yapan kullanıcıya atandı!";
            return RedirectToAction("Index");
        }

        private bool PatientExists(int id)
        {
            var userId = _userManager.GetUserId(User);
            return _context.Patients.Any(e => e.Id == id && e.VeterinarianId == userId);
        }
    }
}