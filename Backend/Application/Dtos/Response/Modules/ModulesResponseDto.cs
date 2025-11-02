namespace Application.Dtos.Response.Modules
{
    public class ModulesResponseDto
    {
        public int PK_MODULE { get; set; }
        public string? MODULE_NAME { get; set; }
        public string? AUDIT_CREATE_DATE { get; set; }
        public bool STATE { get; set; }
        public string? STATE_MODULE { get; set; }
    }
}
