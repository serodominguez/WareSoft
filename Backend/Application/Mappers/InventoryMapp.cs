using Application.Dtos.Request.Inventory;
using Application.Dtos.Response.Inventory;
using Domain.Entities;
using Utilities.Extensions;

namespace Application.Mappers
{
    public static class InventoryMapp
    {
        public static InventoryEntity InventoryMapping (InventoryRequestDto dto)
        {
            return new InventoryEntity
            {
                IdProduct = dto.IdProduct,
                Price = dto.Price
            };
        }

        public static InventoryResponseDto InventoryByStoreMapping(InventoryEntity entity)
        {
            return new InventoryResponseDto
            {
                IdStore = entity.IdStore,
                IdProduct = entity.IdProduct,
                Stock = entity.Stock,
                Price = entity.Price,
                Code = entity.Product?.Code,
                Description = entity.Product?.Description.ToTitleCase(),
                Material = entity.Product?.Material.ToTitleCase(),
                Color = entity.Product?.Color.ToTitleCase(),
                UnitMeasure = entity.Product?.UnitMeasure.ToTitleCase(),
                BrandName = entity.Product?.Brand?.BrandName.ToTitleCase(),
                CategoryName = entity.Product?.Category?.CategoryName.ToTitleCase()
            };
        }
    }
}
