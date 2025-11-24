using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Request.Category
{
    public class CategoryRequestDto
    {
        [Required]
        [StringLength(25, ErrorMessage = "The category name must be 1 to 25 characters.", MinimumLength = 1)]
        public string? CategoryName { get; set; }

        [StringLength(50, ErrorMessage = "The description must have a maximum of 50 characters.")]
        public string? Description { get; set; }
    }
}
