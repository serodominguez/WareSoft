namespace Domain.Entities
{
    public class StoreEntity : BaseEntity
    {
        public string? StoreName { get; set; }
        public string? Manager { get; set; }
        public string? Address { get; set; }
        public int? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? Email { get; set; }
        public string? Type { get; set; }
        public virtual ICollection<UserEntity> User { get; set; } = new List<UserEntity>();
        public virtual ICollection<InventoryEntity> Inventory { get; set; } = new List<InventoryEntity>();
        public virtual ICollection<GoodsIssueEntity> GoodsIssue { get; set; } = new List<GoodsIssueEntity>();
        public virtual ICollection<GoodsReceiptEntity> GoodsReceipt { get; set; } = new List<GoodsReceiptEntity>();
        public ICollection<InboundTransferEntity> InboudTransfersAsOrigin { get; set; } = new List<InboundTransferEntity>();
        public ICollection<InboundTransferEntity> InboudTransfersAsDestination { get; set; } = new List<InboundTransferEntity>();
        public ICollection<OutboundTransferEntity> OutboundTransfersAsOrigin { get; set; } = new List<OutboundTransferEntity>();
        public ICollection<OutboundTransferEntity> OutboundTransfersAsDestination { get; set; } = new List<OutboundTransferEntity>();
    }
}
