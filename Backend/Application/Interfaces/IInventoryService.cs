using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Dtos.Request.Inventory;
using Application.Dtos.Response.Inventory;

namespace Application.Interfaces
{
    public interface IInventoryService
    {
        Task<BaseResponse<IEnumerable<InventoryResponseDto>>> ListInventoryByStore(int authenticatedStoreId, BaseFiltersRequest filters);
        Task<BaseResponse<bool>> UpdatePriceByProduct(int authenticatedUserId, int authenticatedStoreId, InventoryRequestDto requestDto);
    }
}
