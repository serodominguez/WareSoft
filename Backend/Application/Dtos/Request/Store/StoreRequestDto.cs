using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Request.Store
{
    public class StoreRequestDto
    {
        [Required]
        [StringLength(50, ErrorMessage = "The store name must be 1 to 50 characters.", MinimumLength = 1)]
        public string? StoreName { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "The manager must be 1 to 30 characters.", MinimumLength = 1)]
        public string? Manager { get; set; }

        [Required]
        [StringLength(60, ErrorMessage = "The address must be 1 to 60 characters.", MinimumLength = 1)]
        public string? Address { get; set; }

        public int? PhoneNumber { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "The city must be 1 to 15 characters.", MinimumLength = 1)]
        public string? City { get; set; }

        [StringLength(50, ErrorMessage = "The email must have a maximum of 50 characters.")]
        public string? Email { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "The type must be 1 to 15 characters.", MinimumLength = 1)]
        public string? Type { get; set; }
    }
}
