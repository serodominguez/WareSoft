namespace Domain.Entities
{
    public partial class GoodsIssueDetails
    {
        public int PK_ISSUE { get; set; }
        public int SEQUENTIAL_NUMBER { get; set; }
        public int PK_PRODUCTO { get; set; }
        public int QUANTITY { get; set; }
        public virtual Brands? Brands { get; set; }
        public virtual Categories? Categories { get; set; }
        public virtual Products? Products { get; set; }
    }
}
