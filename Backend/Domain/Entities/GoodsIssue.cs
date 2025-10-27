namespace Domain.Entities
{
    public partial class GoodsIssue : BaseEntity
    {
        public int PK_ISSUE { get; set; } 
        public string? CODE { get; set; }
        public DateTime REGISTRATION_DATE { get; set; }
        public string? TYPE { get; set; }
        public string? ANNOTATIONS { get; set; }
        public int PK_CONSUMER { get; set; }
        public int PK_STORE { get; set; }
        public int PK_USER { get; set; }
        public virtual Consumers? Consumers { get; set; }
        public virtual Stores? Stores { get; set; }
        public virtual Users? Users { get; set; }
        public virtual ICollection<GoodsIssueDetails>? details { get; set; } = new List<GoodsIssueDetails>();
    }
}
