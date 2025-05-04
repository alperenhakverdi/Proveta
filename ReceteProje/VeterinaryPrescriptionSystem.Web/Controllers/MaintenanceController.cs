using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinaryPrescriptionSystem.Web.Data;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace VeterinaryPrescriptionSystem.Web.Controllers
{
    // Bu controller sadece bakım amaçlıdır, işiniz bitince silebilirsiniz.
    [Route("maintenance")]
    public class MaintenanceController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MaintenanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("delete-invalid-patients")]
        public async Task<IActionResult> DeleteInvalidPatients()
        {
            try
            {
                // AspNetUsers tablosundaki tüm Id'leri al
                var validVetIds = await _context.Users.Select(u => u.Id).ToListAsync();
                // Hatalı hastaları bul
                var invalidPatients = await _context.Patients.Where(p => !validVetIds.Contains(p.VeterinarianId)).ToListAsync();
                int count = invalidPatients.Count;
                if (count > 0)
                {
                    var invalidPatientIds = invalidPatients.Select(p => p.Id).ToList();
                    // İlişkili reçeteleri sil
                    var prescriptions = _context.Prescriptions.Where(r => invalidPatientIds.Contains(r.PatientId));
                    _context.Prescriptions.RemoveRange(prescriptions);
                    // İlişkili aşı kayıtlarını sil
                    var vaccines = _context.VaccineRecords.Where(v => invalidPatientIds.Contains(v.PatientId));
                    _context.VaccineRecords.RemoveRange(vaccines);
                    // İlişkili muayene kayıtlarını sil
                    var exams = _context.Examinations.Where(e => invalidPatientIds.Contains(e.PatientId));
                    _context.Examinations.RemoveRange(exams);
                    // Son olarak hastaları sil
                    _context.Patients.RemoveRange(invalidPatients);
                    await _context.SaveChangesAsync();
                }
                return Ok($"Silinen hasta kaydı: {count}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Hata oluştu: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }
        }
    }
} 