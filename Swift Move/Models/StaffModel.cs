using System.ComponentModel.DataAnnotations;

namespace Swift_Move.Models
{
    public class Staff
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; }

        public ICollection<ServiceStaff> ServiceStaff { get; set; }
    }
}