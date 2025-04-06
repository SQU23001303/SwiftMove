using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;




namespace Swift_Move.Models
{
    public class ServiceModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Service Title is requires")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Property Address is required")]
        public string CollectionAddress { get; set; }

        [Required(ErrorMessage = "Property Address is required")]
        public string DeliveryAddress { get; set; }

        [Required(ErrorMessage = "Service type is required")]
        public ServiceType ServiceType { get; set; }

        [Required(ErrorMessage = "Collection Date is required")]
        public DateTime CollectionDate { get; set; }

        [Required(ErrorMessage = "Delivery Date is required")]
        public DateTime DeliveryDate { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [Phone(ErrorMessage = "Enter a valid phone number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; }

        [ValidateNever]
        public ICollection<ServiceStaff> ServiceStaff { get; set; }
    }
}