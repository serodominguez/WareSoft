namespace Application.Dtos.Response.Inventory
{
    public class InventoryResponseDto
    {
        public int IdStore { get; set; }
        public int IdProduct { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? Material { get; set; }
        public string? Color { get; set; }
        public string? UnitMeasure { get; set; }
        public string? BrandName { get; set; }
        public string? CategoryName { get; set; }
    }
}
