namespace Domain.Entities
{
    public partial class Consumers : BaseEntity
    {
        public int PK_CONSUMER { get; set; }
        public string? NAMES { get; set; }
        public string? LAST_NAMES { get; set; }
        public string? IDENTIFICATION_NUMBER { get; set; }
        public int PHONE_NUMBER { get; set; }
        public virtual ICollection<GoodsIssue> GoodsIssue { get; set; } = new List<GoodsIssue>();
    }
}
