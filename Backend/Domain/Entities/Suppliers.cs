namespace Domain.Entities
{
    public partial class Suppliers : BaseEntity
    {
        public int PK_SUPPLIER { get; set; }
        public string? COMPANY_NAME { get; set; }
        public string? CONTACT { get; set; }
        public int PHONE_NUMBER { get; set; }
        public string? EMAIL { get; set; }
        public virtual ICollection<GoodsReceipt> GoodsReceipt { get; set; } = new List<GoodsReceipt>();
    }
}
