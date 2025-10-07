namespace Application.Dtos.Response.Roles
{
    public class RolesResponseDto
    {
        public int PK_ROLE { get; set; }
        public string? ROLE_NAME { get; set; }
        public string? AUDIT_CREATE_DATE { get; set; }
        public bool STATE { get; set; }
        public string? STATE_ROLE { get; set; }
    }
}
