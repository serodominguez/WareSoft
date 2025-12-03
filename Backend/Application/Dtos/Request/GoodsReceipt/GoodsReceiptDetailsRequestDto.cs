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
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }
    }
}
