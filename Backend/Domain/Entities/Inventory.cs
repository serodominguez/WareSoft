namespace Domain.Entities
{
    public partial class Inventory
    {
        public int PK_STORE { get; set; }
        public int PK_PRODUCT { get; set; }
        public int STOCK { get; set; }
        public int PRICE { get; set; }
        public virtual Products? products { get; set; }
        public virtual Stores? stores { get; set; }

    }
}
