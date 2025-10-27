namespace Domain.Entities
{
    public partial class InboundTransfer : BaseEntity
    {
        public int PK_INBOUND { get; set; }
        public string? CODE { get; set; }
        public DateTime DATE { get; set; }
        public string? ANNOTATIONS { get; set; }
        public int PK_STORE { get; set; }
        public int PK_USER { get; set; }
        public virtual Stores? Stores { get; set; }
        public virtual Users? Users { get; set; }
        public ICollection<InboundTransferDetails>? details { get; set; } = new List<InboundTransferDetails>();
    }
}
