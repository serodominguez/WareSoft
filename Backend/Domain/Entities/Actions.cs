namespace Domain.Entities
{
    public partial class Actions : BaseEntity
    {
        public int PK_ACTION { get; set; }
        public string? ACTION_NAME { get; set; }
        public virtual ICollection<Permissions> Permissions { get; set; } = new List<Permissions>();
    }
}
