namespace Domain.Entities
{
    public class GoodsReceiptEntity
    {
        public int IdReceipt { get; set; }
        public string? Code { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime CreateDate { get; set; }
        public string? Type { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Number { get; set; }
        public string? Annotations { get; set; }
        public int IdSupplier { get; set; }
        public int IdStore { get; set; }
        public int IdUser { get; set; }
        public int? AuditDeleteUser { get; set; }
        public DateTime? AuditDeleteDate { get; set; }
        public bool Status { get; set; }
        public virtual StoreEntity Store { get; set; } = null!;
        public virtual SupplierEntity Supplier { get; set; } = null!;
        public virtual UserEntity User { get; set; } = null!;
        public ICollection<GoodsReceiptDetailsEntity>? GoodsReceiptDetails { get; set; } = new List<GoodsReceiptDetailsEntity>();
    }
}
