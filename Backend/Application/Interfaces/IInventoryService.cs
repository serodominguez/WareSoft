using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Dtos.Response.Inventory;

namespace Application.Interfaces
{
    public interface IInventoryService
    {
        Task<BaseResponse<IEnumerable<StockByStoreResponseDto>>> ListStockByStore(int storeId, BaseFiltersRequest filters);
    }
}
