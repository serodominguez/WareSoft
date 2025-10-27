namespace Domain.Entities
{
    public partial class GoodsReceipt : BaseEntity
    {
        public int PK_RECEIPT { get; set; }
        public DateTime DATE_PURCHASE { get; set; }
        public DateTime DATE_REGISTRATION { get; set; }
        public string? RECEIPT_TYPE { get; set; }
        public string? NUMBER { get; set; }
        public string? ANNOTATIONS { get; set; }
        public int PK_SUPPLIER { get; set; }
        public int PK_STORE { get; set; }
        public int PK_USER { get; set; }
        public virtual Stores? Stores { get; set; }
        public virtual Suppliers? Suppliers { get; set; }
        public virtual Users? Users { get; set; }
        public ICollection<GoodsReceiptDetails>? details { get; set; } = new List<GoodsReceiptDetails>();
    }
}
