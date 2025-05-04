using VeterinaryPrescriptionSystem.Web.Models;
using System.Collections.Generic;

namespace VeterinaryPrescriptionSystem.Web.Models.ViewModels
{
    public class OwnerDashboardViewModel
    {
        public OwnerUser Owner { get; set; }
        public List<Patient> Patients { get; set; } = new List<Patient>();
        public List<VaccineRecord> VaccineRecords { get; set; } = new List<VaccineRecord>();
    }
} 