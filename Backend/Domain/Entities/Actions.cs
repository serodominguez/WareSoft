namespace Domain.Entities
{
    public partial class Actions : BaseEntity
    {
        public string? ACTION_NAME { get; set; }

        public virtual ICollection<Permissions> Permissions { get; set; } = new List<Permissions>();
    }
}
