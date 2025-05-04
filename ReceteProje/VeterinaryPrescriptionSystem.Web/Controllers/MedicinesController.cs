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

namespace VeterinaryPrescriptionSystem.Web.Controllers
{
    [Authorize] // Sadece giriş yapmış kullanıcılar erişebilir
    public class MedicinesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<MedicinesController> _logger;

        public MedicinesController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            ILogger<MedicinesController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Medicines
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("İlaçlar sayfası yükleniyor...");

                var medicines = await _context.Medicines
                    .OrderBy(m => m.Name)
                    .ToListAsync();

                _logger.LogInformation($"Toplam {medicines.Count} ilaç bulundu.");
                return View(medicines);
            }
            catch (Exception ex)
            {
                _logger.LogError($"İlaçlar listelenirken bir hata oluştu: {ex}");
                TempData["Error"] = "İlaçlar listelenirken bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
                return View(new List<Medicine>());
            }
        }

        // GET: Medicines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicine = await _context.Medicines
                .Include(m => m.PrescriptionMedicines)
                    .ThenInclude(pm => pm.Prescription)
                        .ThenInclude(p => p.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medicine == null)
            {
                return NotFound();
            }

            return View(medicine);
        }

        // GET: Medicines/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Medicines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Medicine medicine)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(medicine);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "İlaç başarıyla kaydedildi.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"İlaç kaydedilirken bir hata oluştu: {ex}");
                ModelState.AddModelError("", "İlaç kaydedilirken bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
            }

            return View(medicine);
        }

        // GET: Medicines/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null)
            {
                return NotFound();
            }

            return View(medicine);
        }

        // POST: Medicines/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Medicine medicine)
        {
            if (id != medicine.Id)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var existingMedicine = await _context.Medicines.FindAsync(id);
                    if (existingMedicine == null)
                    {
                        return NotFound();
                    }

                    existingMedicine.Name = medicine.Name;
                    existingMedicine.Description = medicine.Description;
                    existingMedicine.StockQuantity = medicine.StockQuantity;
                    existingMedicine.Unit = medicine.Unit;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "İlaç bilgileri başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicineExists(medicine.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return View(medicine);
        }

        // GET: Medicines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicine = await _context.Medicines
                .Include(m => m.PrescriptionMedicines)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medicine == null)
            {
                return NotFound();
            }

            // İlaç reçetelerde kullanılmışsa silmeye izin verme
            if (medicine.PrescriptionMedicines.Any())
            {
                TempData["Error"] = "Bu ilaç reçetelerde kullanıldığı için silinemez.";
                return RedirectToAction(nameof(Index));
            }

            return View(medicine);
        }

        // POST: Medicines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicine = await _context.Medicines
                .Include(m => m.PrescriptionMedicines)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medicine != null)
            {
                // İlaç reçetelerde kullanılmışsa silmeye izin verme
                if (medicine.PrescriptionMedicines.Any())
                {
                    TempData["Error"] = "Bu ilaç reçetelerde kullanıldığı için silinemez.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Medicines.Remove(medicine);
                await _context.SaveChangesAsync();
                TempData["Success"] = "İlaç başarıyla silindi.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MedicineExists(int id)
        {
            return _context.Medicines.Any(e => e.Id == id);
        }
    }
}
