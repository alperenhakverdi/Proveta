using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VeterinaryPrescriptionSystem.Web.Data;
using VeterinaryPrescriptionSystem.Web.Models;
using System.Threading.Tasks;
using System.Linq;

namespace VeterinaryPrescriptionSystem.Web.Controllers
{
    [Authorize]
    public class VaccineRecordsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public VaccineRecordsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: VaccineRecords/Create
        public async Task<IActionResult> Create(int patientId)
        {
            var userId = _userManager.GetUserId(User);
            var patient = await _context.Patients
                .Where(p => p.Id == patientId && p.VeterinarianId == userId)
                .FirstOrDefaultAsync();

            if (patient == null)
            {
                return NotFound();
            }

            var vaccineRecord = new VaccineRecord
            {
                PatientId = patientId
            };

            return View(vaccineRecord);
        }

        // POST: VaccineRecords/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VaccineRecord vaccineRecord)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var patient = await _context.Patients
                    .Where(p => p.Id == vaccineRecord.PatientId && p.VeterinarianId == userId)
                    .FirstOrDefaultAsync();

                if (patient == null)
                {
                    return NotFound();
                }

                _context.Add(vaccineRecord);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Aşı kaydı başarıyla oluşturuldu.";
                return RedirectToAction("Details", "Patients", new { id = vaccineRecord.PatientId });
            }
            return View(vaccineRecord);
        }

        // GET: VaccineRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var vaccineRecord = await _context.VaccineRecords
                .Include(v => v.Patient)
                .Where(v => v.Id == id && v.Patient.VeterinarianId == userId)
                .FirstOrDefaultAsync();

            if (vaccineRecord == null)
            {
                return NotFound();
            }

            return View(vaccineRecord);
        }

        // POST: VaccineRecords/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VaccineRecord vaccineRecord)
        {
            if (id != vaccineRecord.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = _userManager.GetUserId(User);
                    var existingRecord = await _context.VaccineRecords
                        .Include(v => v.Patient)
                        .Where(v => v.Id == id && v.Patient.VeterinarianId == userId)
                        .FirstOrDefaultAsync();

                    if (existingRecord == null)
                    {
                        return NotFound();
                    }

                    existingRecord.Name = vaccineRecord.Name;
                    existingRecord.Date = vaccineRecord.Date;
                    existingRecord.Veterinarian = vaccineRecord.Veterinarian;
                    existingRecord.Note = vaccineRecord.Note;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Aşı kaydı başarıyla güncellendi.";
                    return RedirectToAction("Details", "Patients", new { id = existingRecord.PatientId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VaccineRecordExists(vaccineRecord.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(vaccineRecord);
        }

        // GET: VaccineRecords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var vaccineRecord = await _context.VaccineRecords
                .Include(v => v.Patient)
                .Where(v => v.Id == id && v.Patient.VeterinarianId == userId)
                .FirstOrDefaultAsync();

            if (vaccineRecord == null)
            {
                return NotFound();
            }

            return View(vaccineRecord);
        }

        // POST: VaccineRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var vaccineRecord = await _context.VaccineRecords
                .Include(v => v.Patient)
                .Where(v => v.Id == id && v.Patient.VeterinarianId == userId)
                .FirstOrDefaultAsync();

            if (vaccineRecord != null)
            {
                var patientId = vaccineRecord.PatientId;
                _context.VaccineRecords.Remove(vaccineRecord);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Aşı kaydı başarıyla silindi.";
                return RedirectToAction("Details", "Patients", new { id = patientId });
            }

            return NotFound();
        }

        private bool VaccineRecordExists(int id)
        {
            return _context.VaccineRecords.Any(e => e.Id == id);
        }
    }
} 