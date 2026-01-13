using Application.Dtos.Response.Inventory;
using Domain.Entities;

namespace Application.Mappers
{
    public static class InventoryMapp
    {
        public static StockByStoreResponseDto StockByStoreMapping(InventoryEntity entity)
        {
            return new StockByStoreResponseDto
            {
                IdStore = entity.IdStore,
                IdProduct = entity.IdProduct,
                Stock = entity.Stock,
                Price = entity.Price,
                Code = entity.Product?.Code,
                Description = entity.Product?.Description,
                Material = entity.Product?.Material,
                Color = entity.Product?.Color,
                BrandName = entity.Product?.Brand?.BrandName,
                CategoryName = entity.Product?.Category?.CategoryName
            };
        }
    }
}
