namespace Domain.Entities
{
    public class GoodsIssueDetails
    {
        public int PK_ISSUE { get; set; }
        public int SEQUENTIAL_NUMBER { get; set; }
        public int PK_PRODUCTO { get; set; }
        public int QUANTITY { get; set; }
        public Brands? Brands { get; set; }
        public Categories? Categories { get; set; }
        public Products? Products { get; set; }
    }
}
