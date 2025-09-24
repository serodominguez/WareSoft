namespace Domain.Entities
{
    public class Inventory
    {
        public int PK_STORE { get; set; }
        public int PK_PRODUCT { get; set; }
        public int STOCK { get; set; }
        public int PRICE { get; set; }
        public Products? products { get; set; }
        public Stores? stores { get; set; }

    }
}
