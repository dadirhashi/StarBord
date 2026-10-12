using System.ComponentModel.DataAnnotations;

namespace StarBord.DTOS
{
    public class CreateReviewDto
    {
        [Required]
        public Guid BusinessId { get; set; }

        [Required]
        [MaxLength(5000)]
        public string ReviewText { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [MaxLength(50)]
        public string Platform { get; set; }

        [Required]
        public DateTime ReviewDate { get; set; }

        [MaxLength(200)]
        public string? ExternalReviewId { get; set; }
    }
}
