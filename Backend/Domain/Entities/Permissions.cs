namespace Domain.Entities
{
    public partial class Permissions : BaseEntity
    {
        public int PK_PERMISSION { get; set; }
        public int PK_ROLE { get; set; }
        public int PK_MODULE { get; set; }
        public int PK_ACTION { get; set; }

        public virtual Roles? Roles { get; set; }
        public virtual Modules? Modules { get; set; }
        public virtual Actions? Actions { get; set; }
    }
}
