using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Request.Brand
{
    public class BrandRequestDto
    {
        [Required]
        [StringLength(25, ErrorMessage = "The brand name must be 1 to 25 characters.", MinimumLength = 1)]
        public string? BrandName { get; set; }
    }
}
