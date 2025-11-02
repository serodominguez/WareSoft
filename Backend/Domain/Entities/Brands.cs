namespace Domain.Entities
{
    public partial class Brands : BaseEntity
    {
        public string? BRAND_NAME { get; set; }
        public virtual ICollection<Products> Products { get; set; } = new List<Products>();
    }
}
