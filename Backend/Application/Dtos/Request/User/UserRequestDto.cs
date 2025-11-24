using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Request.User
{
    public class UserRequestDto
    {
        [Required]
        [StringLength(20, ErrorMessage = "The user name must be 1 to 20 characters long.", MinimumLength = 1)]
        public string? UserName { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "The names must be 1 to 30 characters long.", MinimumLength = 1)]
        public string? Names { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The lastnames must be 1 to 50 characters long.", MinimumLength = 1)]
        public string? LastNames { get; set; }

        [StringLength(10, ErrorMessage = "The identification number must have a maximum of 10 characters.")]
        public string? IdentificationNumber { get; set; }

        public int? PhoneNumber { get; set; }

        [Required]
        public int IdRole { get; set; }

        [Required]
        public int IdStore { get; set; }

        [Required]
        public bool? UpdatePassword { get; set; }
    }
}
