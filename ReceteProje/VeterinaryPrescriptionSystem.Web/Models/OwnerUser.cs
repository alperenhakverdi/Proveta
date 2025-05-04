using System.ComponentModel.DataAnnotations;

namespace VeterinaryPrescriptionSystem.Web.Models
{
    public class OwnerUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [StringLength(200)]
        public string Address { get; set; }
    }
} 