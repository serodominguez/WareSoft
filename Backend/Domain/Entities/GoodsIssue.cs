namespace Domain.Entities
{
    public class GoodsIssue
    {
        public int PK_ISSUE { get; set; } 
        public string? CODE { get; set; }
        public DateTime REGISTRATION_DATE { get; set; }
        public string? TYPE { get; set; }
        public string? ANNOTATIONS { get; set; }
        public int PK_CONSUMER { get; set; }
        public int PK_STORE { get; set; }
        public int PK_USER { get; set; }
        public int AUDIT_DELETE_USER { get; set; }
        public DateTime AUDIT_DELETE_DATE { get; set; }
        public bool STATE { get; set; }
        public Consumers? Consumers { get; set; }
        public Stores? Stores { get; set; }
        public Users? Users { get; set; }
        public ICollection<GoodsIssueDetails>? details { get; set; }
    }
}
