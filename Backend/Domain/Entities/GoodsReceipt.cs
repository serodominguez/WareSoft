namespace Domain.Entities
{
    public class GoodsReceipt
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
        public int AUDIT_DELETE_USER { get; set; }
        public DateTime AUDIT_DELETE_DATE { get; set; }
        public bool STATE { get; set; }
        public Stores? Stores { get; set; }
        public Suppliers? Suppliers { get; set; }
        public Users? Users { get; set; }
        public ICollection<GoodsReceiptDetails>? details { get; set; }
    }
}
