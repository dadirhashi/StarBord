using System.ComponentModel.DataAnnotations;

namespace StarBord.DTOS
{
    public class CreateResponseDto
    {
        [Required]
        public Guid ReviewId { get; set; }

        [Required]
        [MaxLength(5000)]
        public string ResponseText { get; set; }
    }
}