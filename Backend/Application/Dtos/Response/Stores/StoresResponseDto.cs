namespace Application.Dtos.Response.Stores
{
    public class StoresResponseDto
    {
        public int PK_STORE { get; set; }
        public string? STORE_NAME { get; set; }
        public string? MANAGER { get; set; }
        public string? ADDRESS { get; set; }
        public int? PHONE_NUMBER { get; set; }
        public string? CITY { get; set; }
        public string? EMAIL { get; set; }
        public string? TYPE { get; set; }
        public string? AUDIT_CREATE_DATE { get; set; }
        public bool STATE { get; set; }
        public string? STATE_STORE { get; set; }
    }
}
