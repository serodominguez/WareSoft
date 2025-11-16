namespace Domain.Entities
{
    public class OutboundTransferEntity
    {
        public int IdOutbound { get; set; }
        public string? TransferCode { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Annotations { get; set; }
        public int IdStoreOrigin { get; set; }
        public int IdStoreDestination { get; set; }
        public int? AuditCreateUser { get; set; }
        public DateTime? AuditCreateDate { get; set; }
        public int? AuditUpdateUser { get; set; }
        public DateTime? AuditUpdateDate { get; set; }
        public int? AuditDeleteUser { get; set; }
        public DateTime? AuditDeleteDate { get; set; }
        public string? Status { get; set; }
        public virtual StoreEntity StoreOrigin { get; set; } = null!;
        public virtual StoreEntity StoreDestination { get; set; } = null!;
        public InboundTransferEntity? InboundTransfer { get; set; }
        public ICollection<OutboundTransferDetailsEntity> OutboundTransferDetails { get; set; } = new List<OutboundTransferDetailsEntity>();
    }
}
