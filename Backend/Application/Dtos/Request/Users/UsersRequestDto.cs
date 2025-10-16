namespace Application.Dtos.Request.Users
{
    public class UsersRequestDto
    {
        public string? USER_NAME { get; set; }
        public string? NAMES { get; set; }
        public string? PASSWORD { get; set; }
        public string? LAST_NAMES { get; set; }
        public string? IDENTIFICATION_NUMBER { get; set; }
        public int PHONE_NUMBER { get; set; }
        public int PK_ROLE { get; set; }
        public int PK_STORE { get; set; }
        public bool? UPDATE_PASSWORD { get; set; }
    }
}
