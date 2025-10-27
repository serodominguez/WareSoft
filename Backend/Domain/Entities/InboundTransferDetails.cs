namespace Domain.Entities
{
    public partial class InboundTransferDetails
    {
        public int PK_INBOUND { get; set; }
        public int SEQUENTIAL_NUMBER { get; set; }
        public int PK_PRODUCT { get; set; }
        public int QUANTITY { get; set; }
        public virtual Brands? Brands { get; set; }
        public virtual Categories? Categories { get; set; }
        public virtual Products? Products { get; set; }
    }
}
