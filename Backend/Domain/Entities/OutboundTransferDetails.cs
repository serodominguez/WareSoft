namespace Domain.Entities
{
    public class OutboundTransferDetails
    {
        public int PK_OUTBOUND { get; set; }
        public int SEQUENTIAL_NUMBER { get; set; }
        public int PK_PRODUCT { get; set; }
        public int QUANTITY { get; set; }
        public Brands? Brands { get; set; }
        public Categories? Categories { get; set; }
        public Products? Products { get; set; }
    }
}
