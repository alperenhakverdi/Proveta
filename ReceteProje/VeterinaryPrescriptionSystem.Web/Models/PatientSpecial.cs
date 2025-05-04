using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterinaryPrescriptionSystem.Web.Models
{
    public class PatientSpecial
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public Patient Patient { get; set; }

        [Display(Name = "Kronik Rahatsızlık")]
        public string ChronicDisease { get; set; }

        [Display(Name = "Alerji")]
        public string Allergy { get; set; }

        [Display(Name = "Kullanılan İlaçlar")]
        public string Drugs { get; set; }

        [Display(Name = "Önceki Ameliyatlar")]
        public string PreviousSurgeries { get; set; }
    }
} 