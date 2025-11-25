using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Request.Supplier
{
    public class SupplierRequestDto
    {
        [Required]
        [StringLength(50, ErrorMessage = "The name must be 1 to 50 characters.", MinimumLength = 1)]
        public string? CompanyName { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "The name must be 1 to 30 characters.", MinimumLength = 1)]
        public string? Contact { get; set; }

        public int? PhoneNumber { get; set; }

        [StringLength(50, ErrorMessage = "The email must have a maximum of 50 characters.")]
        public string? Email { get; set; }
    }
}
