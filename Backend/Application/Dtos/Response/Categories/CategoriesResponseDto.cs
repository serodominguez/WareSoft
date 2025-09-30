namespace Application.Dtos.Response.Categories
{
    public class CategoriesResponseDto
    {
        public int PK_CATEGORY { get; set; }
        public string? CATEGORY_NAME { get; set; }
        public string? DESCRIPTION { get; set; }
        public string? AUDIT_CREATE_DATE { get; set; }
        public bool STATE { get; set; }
        public string? STATE_CATEGORY { get; set; }
    }
}
