using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Request.Module
{
    public class ModuleRequestDto
    {
        [Required]
        [StringLength(25, ErrorMessage = "The module name must be 1 to 25 characters long.", MinimumLength = 1)]
        public string? ModuleName { get; set; }
    }
}
