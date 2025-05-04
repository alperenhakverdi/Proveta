using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterinaryPrescriptionSystem.Web.Models
{
    public class Examination
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public Patient Patient { get; set; }

        [Required]
        [Display(Name = "Tarih")]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Şikayet")]
        public string Complaint { get; set; }

        [Required]
        [Display(Name = "Tanı")]
        public string Diagnosis { get; set; }

        [Required]
        [Display(Name = "Tedavi")]
        public string Treatment { get; set; }

        [Display(Name = "Reçete Linki")]
        public string PrescriptionLink { get; set; }
    }
} 