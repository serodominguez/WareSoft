using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Request.Permission
{
    public class PermissionRequestDto
    {
        [Required]
        public int IdPermission { get; set; }

        [Required]
        public bool Status { get; set; }
    }
}
