namespace Domain.Entities
{
    public partial class Modules : BaseEntity
    {
        public int PK_MODULE { get; set; }
        public string? MODULE_NAME { get; set; }

        public virtual ICollection<Permissions> Permissions { get; set; } = new List<Permissions>();
    }
}
