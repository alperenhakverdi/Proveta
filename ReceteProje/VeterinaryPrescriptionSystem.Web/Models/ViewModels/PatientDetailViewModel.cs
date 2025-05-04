using System.Collections.Generic;

namespace VeterinaryPrescriptionSystem.Web.Models.ViewModels
{
    public class PatientDetailViewModel
    {
        public Patient Patient { get; set; }
        public PatientSpecial Special { get; set; }
        public List<VaccineRecord> VaccineRecords { get; set; }
        public List<Examination> Examinations { get; set; }
    }
}
