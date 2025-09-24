namespace Domain.Entities
{
    public class Stores
    {
        public int PK_SOTRE { get; set; }
        public string? STORE_NAME { get; set; }
        public string? MANAGER { get; set; }
        public string? ADDRESS { get; set; }
        public int PHONE_NUMBER { get; set; }
        public string? CITY { get; set; }
        public string? EMAIL { get; set; }
        public string? TYPE { get; set; }
        public int AUDIT_CREATE_USER { get; set; }
        public DateTime AUDIT_CREATE_DATE { get; set; }
        public int AUDIT_UPDATE_USER { get; set; }
        public DateTime AUDIT_UPDATE_DATE { get; set; }
        public int AUDIT_DELETE_USER { get; set; }
        public DateTime AUDIT_DELETE_DATE { get; set; }
        public bool STATE { get; set; }
        public virtual ICollection<Users> Users { get; set; } = new List<Users>();
        public virtual ICollection<Inventory> Inventory { get; set; } = new List<Inventory>();
        public virtual ICollection<GoodsIssue> GoodsIssue { get; set; } = new List<GoodsIssue>();
        public virtual ICollection<GoodsReceipt> GoodsReceipt { get; set; } = new List<GoodsReceipt>();
        public virtual ICollection<InboundTransfer> InboundTransfer { get; set; } = new List<InboundTransfer>();
        public virtual ICollection<OutboundTransfer> OutboundTransfer { get; set; } = new List<OutboundTransfer>();
    }
}
