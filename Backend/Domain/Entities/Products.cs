namespace Domain.Entities
{
    public class Products
    {
        public int PK_PRODUCT { get; set; }
        public string? CODE { get; set; }
        public string? DESCRIPTION { get; set; }
        public string? MATERIAL { get; set; }
        public string? COLOR { get; set; }
        public string? MEASUREMENT { get; set; }
        public int PK_BRAND { get; set; }
        public int PK_CATEGORY { get; set; }
        public int AUDIT_CREATE_USER { get; set; }
        public DateTime AUDIT_CREATE_DATE { get; set; }
        public int AUDIT_UPDATE_USER { get; set; }
        public DateTime AUDIT_UPDATE_DATE { get; set; }
        public int AUDIT_DELETE_USER { get; set; }
        public DateTime AUDIT_DELETE_DATE { get; set; }
        public bool STATE { get; set; }
        public virtual Brands? Brands { get; set; }
        public virtual Categories? Categories { get; set; }
        public virtual ICollection<Inventory> Inventory { get; set; } = new List<Inventory>();
        public virtual ICollection<GoodsIssueDetails> GoodsIssueDetails { get; set; } = new List<GoodsIssueDetails>();
        public virtual ICollection<GoodsReceiptDetails> GoodsReceiptDetails { get; set; } = new List<GoodsReceiptDetails>();
        public virtual ICollection<InboundTransferDetails> InboundTransferDetails { get; set; } = new List<InboundTransferDetails>();
        public virtual ICollection<OutboundTransferDetails> OutboundTransferDetails { get; set; } = new List<OutboundTransferDetails>();
    }
}
