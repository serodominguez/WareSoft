using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Request.GoodsReceipt
{
    public class GoodsReceiptDetailsRequestDto
    {
        [Required]
        public int Item { get; set; }

        [Required]
        public int IdProduct { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Unit price cannot be negative")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Total price cannot be negative")]
        public decimal TotalPrice { get; set; }
    }
}
