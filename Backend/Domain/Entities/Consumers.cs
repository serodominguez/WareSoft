namespace Domain.Entities
{
    public class Consumers
    {
        public int PK_CONSUMER { get; set; }
        public string? NAMES { get; set; }
        public string? LAST_NAMES { get; set; }
        public string? IDENTIFICATION_NUMBER { get; set; }
        public int PHONE_NUMBER { get; set; }
        public DateTime AUDIT_CREATE_DATE { get; set; }
        public int AUDIT_UPDATE_USER { get; set; }
        public DateTime AUDIT_UPDATE_DATE { get; set; }
        public int AUDIT_DELETE_USER { get; set; }
        public DateTime AUDIT_DELETE_DATE { get; set; }
        public bool STATE { get; set; }
        public virtual ICollection<GoodsIssue> GoodsIssue { get; set; } = new List<GoodsIssue>();
    }
}
