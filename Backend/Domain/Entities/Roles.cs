namespace Domain.Entities
{
    public class Roles
    {
        public int PK_ROLE { get; set; }
        public string? ROLE_NAME { get; set; }
        public bool STATE { get; set; }
        public virtual ICollection<Users> Users { get; set; } = new List<Users>();
    }
}
