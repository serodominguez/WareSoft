namespace Application.Dtos.Response.Users
{
    public class UsersResponseDto
    {
        public int PK_USER { get; set; }
        public string? USER_NAME { get; set; }
        public byte[]? PASSWORD_HASH { get; set; }
        public string? NAMES { get; set; }
        public string? LAST_NAMES { get; set; }
        public string? IDENTIFICATION_NUMBER { get; set; }
        public int PHONE_NUMBER { get; set; }
        public int PK_ROLE { get; set; }
        public int PK_STORE { get; set; }
        public string? AUDIT_CREATE_DATE { get; set; }
        public bool STATE { get; set; }
        public string? STATE_USER { get; set; }

        public string? ROLE_NAME { get; set; }
        public string? STORE_NAME { get; set; }
    }
}
