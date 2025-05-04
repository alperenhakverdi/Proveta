using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using VeterinaryPrescriptionSystem.Web.Models;
using VeterinaryPrescriptionSystem.Web.Models.ViewModels;

namespace VeterinaryPrescriptionSystem.Web.Controllers
{
    public class OwnerDashboardController : Controller
    {
        public IActionResult Index()
        {
            // Dummy (örnek) veri
            var owner = new OwnerUser
            {
                Name = "Test Sahibi",
                Email = "test@example.com",
                Phone = "555-123-4567",
                Address = "Test Mahallesi"
            };

            var patient = new Patient
            {
                Id = 1,
                Name = "Boncuk",
                Age = 3,
                Species = "Köpek",
                Breed = "Golden Retriever",
                OwnerName = owner.Name,
                OwnerPhone = owner.Phone,
                OwnerEmail = owner.Email
            };

            var vaccines = new List<VaccineRecord>
            {
                new VaccineRecord
                {
                    PatientId = 1,
                    VaccineName = "Karma Aşı",
                    Date = DateTime.Now.AddDays(-30),
                    Note = "1 ay önce yapıldı"
                }
            };

            var vm = new OwnerDashboardViewModel
            {
                Owner = owner,
                Patients = new List<Patient> { patient },
                VaccineRecords = vaccines
            };

            return View(vm);
        }
    }
}
