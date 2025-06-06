using System.ComponentModel.DataAnnotations;

namespace Swift_Move.Models
{
    public class ServiceList
    {
        //Service List is on the admin dashboard to be able to edit what services the company has to offer
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Staff Required")]
        public int StaffRequired { get; set; }

        [Required]
        [Display(Name = "Price per Job (�)")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
    }
}
