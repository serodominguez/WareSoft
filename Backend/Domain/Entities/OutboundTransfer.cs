namespace Domain.Entities
{
    public class OutboundTransfer
    {
        public int PK_OUTBOUND { get; set; }
        public string? CODE { get; set; }
        public DateTime DATE { get; set; }
        public string? ANNOTATIONS { get; set; }
        public int PK_STORE { get; set; }
        public int PK_USER { get; set; }
        public int AUDIT_DELETE_USER { get; set; }
        public int AUDIT_DELETE_DATE { get; set; }
        public bool STATE { get; set; }
        public Stores? Stores { get; set; }
        public Users? Users { get; set; }
        public ICollection<OutboundTransferDetails>? details { get; set; }
    }
}
