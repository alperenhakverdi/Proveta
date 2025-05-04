using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterinaryPrescriptionSystem.Web.Models
{
    public class VaccineRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string VaccineName { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Veterinarian { get; set; }

        [Required]
        public string BatchNumber { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }

        [Required]
        public string Manufacturer { get; set; }

        [Required]
        public string Note { get; set; }

        [Required]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public Patient Patient { get; set; }
    }
} 