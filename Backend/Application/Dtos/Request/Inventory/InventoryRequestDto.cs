using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Request.Inventory
{
    public class InventoryRequestDto
    {
        [Required]
        public int IdProduct { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
    }
}
