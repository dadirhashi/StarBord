using System.ComponentModel.DataAnnotations;

namespace StarBord.DTOS
{
    public class CreatePlatformTokenDto
    {
        [Required]
        public Guid BusinessId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Platform { get; set; }

        [Required]
        public string AccessToken { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }
    }
}
