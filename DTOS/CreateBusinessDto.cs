using System.ComponentModel.DataAnnotations;

namespace StarBord.DTOS
{
    public class CreateBusinessDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }


        [Required]
        [MaxLength(500)]
        public string Address { get; set; }
    }
}
