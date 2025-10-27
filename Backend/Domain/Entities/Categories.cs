namespace Domain.Entities
{
    public partial class Categories : BaseEntity
    {
        public int PK_CATEGORY { get; set; }
        public string? CATEGORY_NAME { get; set; }
        public string? DESCRIPTION { get; set; }
        public virtual ICollection<Products> Products { get; set; } = new List<Products>();
    }
}
