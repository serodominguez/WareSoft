namespace Domain.Entities
{
    public partial class GoodsReceiptDetails
    {
        public int PK_RECEIPT { get; set; }
        public int SEQUENTIAL_NUMBER { get; set; }
        public int PK_PRODUCTO { get; set; }
        public int QUANTITY { get; set; }
        public virtual Brands? Brands { get; set; }
        public virtual Categories? Categories { get; set; }
        public virtual Products? Products { get; set; }
    }
}
