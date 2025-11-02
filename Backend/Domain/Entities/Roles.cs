namespace Domain.Entities
{
    public partial class Roles : BaseEntity
    {
        public string? ROLE_NAME { get; set; }
        public virtual ICollection<Permissions> Permissions { get; set; } = new List<Permissions>();
        public virtual ICollection<Users> Users { get; set; } = new List<Users>();
    }
}
