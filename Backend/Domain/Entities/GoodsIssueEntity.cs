namespace Domain.Entities
{
    public class GoodsIssueEntity
    {
        public int IdIssue { get; set; }
        public string? Code { get; set; }
        public DateTime CreateDate { get; set; }
        public string? Type { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Annotations { get; set; }
        public int IdConsumer { get; set; }
        public int IdStore { get; set; }
        public int IdUser { get; set; }
        public int? AuditDeleteUser { get; set; }
        public DateTime? AuditDeleteDate { get; set; }
        public bool Status { get; set; }
        public virtual ConsumerEntity Consumer { get; set; } = null!;
        public virtual StoreEntity Store { get; set; } = null!;
        public virtual UserEntity User { get; set; } = null!;
        public virtual ICollection<GoodsIssueDetailsEntity>? GoodsIssueDetails { get; set; } = new List<GoodsIssueDetailsEntity>();
    }
}
