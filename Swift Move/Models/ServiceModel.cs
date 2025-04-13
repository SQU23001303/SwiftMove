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

        //Service List model so admins can edit easily 
        [Required(ErrorMessage = "Service type is required")]
        [Display(Name = "Service Type")]
        public int ServiceListId { get; set; }
        [ForeignKey("ServiceListId")]
        [ValidateNever]
        public ServiceList ServiceList { get; set; }

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

        [ValidateNever]
        [Display(Name = "Quote Price (£)")]
        [DataType(DataType.Currency)]
        public decimal? QuotePrice { get; set; }

        [BindNever]
        public string? UserId { get; set; }

    }
}