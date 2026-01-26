namespace Domain.Entities
{
    public class InboundTransferEntity
    {
        public int IdInbound { get; set; }
        public string? TransferCode { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Annotations { get; set; }
        public int IdStoreOrigin { get; set; }
        public int IdStoreDestination { get; set; }
        public int IdOutbound { get; set; }
        public int? AuditCreateUser { get; set; }
        public DateTime? AuditCreateDate { get; set; }
        public int? AuditUpdateUser { get; set; }
        public DateTime? AuditUpdateDate { get; set; }
        public int? AuditDeleteUser { get; set; }
        public DateTime? AuditDeleteDate { get; set; }
        public string? Status { get; set; }
        public virtual StoreEntity StoreOrigin { get; set; } = null!;
        public virtual StoreEntity StoreDestination { get; set; } = null!;
        public virtual OutboundTransferEntity OutboundTransfer { get; set; } = null!;
        public ICollection<InboundTransferDetailsEntity>? InboundTransferDetails { get; set; } = new List<InboundTransferDetailsEntity>();
    }
}
