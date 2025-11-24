using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Request.Customer
{
    public class CustomerRequestDto
    {
        [Required]
        [StringLength(30, ErrorMessage = "The name must be 1 to 30 characters.", MinimumLength = 1)]
        public string? Names { get; set; }
        
        [Required]
        [StringLength(50, ErrorMessage = "The last names must be 1 to 50 characters.", MinimumLength = 1)]
        public string? LastNames { get; set; }
        
        [StringLength(10, ErrorMessage = "The identification number must have a maximum of 10 characters.")]
        public string? IdentificationNumber { get; set; }

        public int? PhoneNumber { get; set; }

    }
}
