using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Request.Product
{
    public class ProductRequestDto
    {
        [StringLength(25, ErrorMessage = "The code must have a maximum of 25 characters.")]
        public string? Code { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The description must be 1 to 50 characters.", MinimumLength = 1)]
        public string? Description { get; set; }

        [StringLength(25, ErrorMessage = "The material must have a maximum of 25 characters.")]
        public string? Material { get; set; }

        [StringLength(20, ErrorMessage = "The color must have a maximum of 20 characters.")]
        public string? Color { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "The unit of measure must be 1 to 15 characters.", MinimumLength = 1)]
        public string? UnitMeasure { get; set; }

        [Required]
        public int IdBrand { get; set; }

        [Required]
        public int IdCategory { get; set; }
    }
}
