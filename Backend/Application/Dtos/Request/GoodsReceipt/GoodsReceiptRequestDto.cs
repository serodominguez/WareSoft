using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Request.GoodsReceipt
{
    public class GoodsReceiptRequestDto
    {
        [Required]
        [StringLength(15, ErrorMessage = "The type must be 1 to 25 characters.", MinimumLength = 1)]
        public string? Type { get; set; }

        [Required]
        public DateTime DocumentDate { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "The type must be 1 to 25 characters.", MinimumLength = 1)]
        public string? DocumentType { get; set; }

        public string? DocumentNumber { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [StringLength(80, ErrorMessage = "The annotations must have a maximum of 80 characters.")]
        public string? Annotations { get; set; }

        [Required]
        public int IdSupplier { get; set; }

        [Required]
        public int IdStore { get; set; }

        public ICollection<GoodsReceiptDetailsRequestDto> GoodsReceiptDetails { get; set; } = null!;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if (Type == "ADQUISICIÓN" && string.IsNullOrWhiteSpace(DocumentNumber))
            {
                yield return new ValidationResult("Document number is required for acquisition type.",
                    new[] { nameof(DocumentNumber) }
                );
            }
        }
    }
}
