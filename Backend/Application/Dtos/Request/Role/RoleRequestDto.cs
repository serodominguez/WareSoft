using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Request.Role
{
    public class RoleRequestDto
    {
        [Required]
        [StringLength(20, ErrorMessage = "The role name must be 1 to 20 characters long.", MinimumLength = 1)]
        public string? RoleName { get; set; }
    }
}
