namespace Domain.Entities
{
    public partial class OutboundTransfer : BaseEntity
    {
        public int PK_OUTBOUND { get; set; }
        public string? CODE { get; set; }
        public DateTime DATE { get; set; }
        public string? ANNOTATIONS { get; set; }
        public int PK_STORE { get; set; }
        public int PK_USER { get; set; }
        public virtual Stores? Stores { get; set; }
        public virtual Users? Users { get; set; }
        public ICollection<OutboundTransferDetails>? details { get; set; } = new List<OutboundTransferDetails>();
    }
}
