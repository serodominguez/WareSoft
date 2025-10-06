namespace Application.Dtos.Response.Brands
{
    public class BrandsResponseDto
    {
        public int PK_BRAND { get; set; }
        public string? BRAND_NAME { get; set; }
        public string? AUDIT_CREATE_DATE { get; set; }
        public bool STATE { get; set; }
        public string? STATE_BRAND { get; set; }
    }
}
