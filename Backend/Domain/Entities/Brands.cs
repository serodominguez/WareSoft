namespace Domain.Entities
{
    public class Brands : BaseEntity
    {
        public int PK_BRAND { get; set; }
        public string? BRAND_NAME { get; set; }
        public virtual ICollection<Products> Products { get; set; } = new List<Products>();
    }
}
