using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Request.Product
{
    public class ProductRequestDto
    {
        [StringLength(25, ErrorMessage = "The code must be 1 to 25 characters long.", MinimumLength = 1)]
        public string? Code { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The description must be 1 to 50 characters long.", MinimumLength = 1)]
        public string? Description { get; set; }

        [StringLength(25, ErrorMessage = "The material must be 1 to 25 characters long.", MinimumLength = 1)]
        public string? Material { get; set; }

        [StringLength(20, ErrorMessage = "The color must be 1 to 20 characters long.", MinimumLength = 1)]
        public string? Color { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "The unit of measure must be 1 to 15 characters long.", MinimumLength = 1)]
        public string? UnitMeasure { get; set; }

        [Required]
        public int IdBrand { get; set; }

        [Required]
        public int IdCategory { get; set; }
    }
}
