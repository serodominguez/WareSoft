namespace Domain.Entities
{
    public class Users
    {
        public int PK_USER { get; set; }
        public string? USER_NAME { get; set; }
        public byte[]? PASSWORD_HASH { get; set; }
        public byte[]? PASSWORD_SALT { get; set; }
        public string? NAMES { get; set; }
        public string? LAST_NAMES { get; set; }
        public string? IDENTIFICATION_NUMBER { get; set; }
        public int PHONE_NUMBER { get; set; }
        public int PK_ROLE { get; set; }
        public int PK_STORE { get; set; }
        public int AUDIT_CREATE_USER { get; set; }
        public DateTime AUDIT_CREATE_DATE { get; set; }
        public int AUDIT_UPDATE_USER { get; set; }
        public DateTime AUDIT_UPDATE_DATE { get; set; }
        public int AUDIT_DELETE_USER { get; set; }
        public DateTime AUDIT_DELETE_DATE { get; set; }
        public bool STATE { get; set; }
        public Roles? Roles { get; set; }
        public Stores? Stores { get; set; }
        public virtual ICollection<GoodsIssue> GoodsIssue { get; set; } = new List<GoodsIssue>();
        public virtual ICollection<GoodsReceipt> GoodsReceipt { get; set; } = new List<GoodsReceipt>();
        public virtual ICollection<InboundTransfer> InboundTransfer { get; set; } = new List<InboundTransfer>();
        public virtual ICollection<OutboundTransfer> OutboundTransfer { get; set; } = new List<OutboundTransfer>();
    }
}
