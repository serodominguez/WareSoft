namespace Domain.Entities
{
    public partial class Categories : BaseEntity
    {
        public string? CATEGORY_NAME { get; set; }
        public string? DESCRIPTION { get; set; }
        public virtual ICollection<Products> Products { get; set; } = new List<Products>();
    }
}
