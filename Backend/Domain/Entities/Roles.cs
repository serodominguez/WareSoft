namespace Domain.Entities
{
    public class Roles
    {
        public int PK_ROLE { get; set; }
        public string? ROLE_NAME { get; set; }
        public int? AUDIT_CREATE_USER { get; set; }
        public DateTime? AUDIT_CREATE_DATE { get; set; }
        public int? AUDIT_UPDATE_USER { get; set; }
        public DateTime? AUDIT_UPDATE_DATE { get; set; }
        public int? AUDIT_DELETE_USER { get; set; }
        public DateTime? AUDIT_DELETE_DATE { get; set; }
        public bool STATE { get; set; }
        public virtual ICollection<Users> Users { get; set; } = new List<Users>();
    }
}
