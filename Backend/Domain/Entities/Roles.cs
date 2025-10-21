namespace Domain.Entities
{
    public class Roles : BaseEntity
    {
        public int PK_ROLE { get; set; }
        public string? ROLE_NAME { get; set; }
        public virtual ICollection<Users> Users { get; set; } = new List<Users>();
    }
}
