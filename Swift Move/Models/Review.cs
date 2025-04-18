using System.ComponentModel.DataAnnotations;

namespace Swift_Move.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Comment cannot be empty or longer than 500 characters")]
        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? UserId { get; set; }

        public string? UserName { get; set; }
    }
}


