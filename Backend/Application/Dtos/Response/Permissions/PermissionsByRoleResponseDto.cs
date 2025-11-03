namespace Application.Dtos.Response.Permissions
{
    public class PermissionsByRoleResponseDto
    {
        public int PK_PERMISSION { get; set; }
        public int PK_ROLE { get; set; }
        public int PK_MODULE { get; set; }
        public string? MODULE_NAME { get; set; }
        public int PK_ACTION { get; set; }
        public string? ACTION_NAME { get; set; }
        public bool STATE { get; set; }
    }
}
