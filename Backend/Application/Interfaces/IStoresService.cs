using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Dtos.Request.Stores;
using Application.Dtos.Response.Stores;

namespace Application.Interfaces
{
    public interface IStoresService
    {
        Task<BaseResponse<IEnumerable<StoresResponseDto>>> ListStores(BaseFiltersRequest filters);
        Task<BaseResponse<IEnumerable<StoresSelectResponseDto>>> ListSelectStores();
        Task<BaseResponse<StoresResponseDto>> StoreById(int storeId);
        Task<BaseResponse<bool>> RegisterStore(int authenticatedUserId, StoresRequestDto requestDto);
        Task<BaseResponse<bool>> EditStore(int authenticatedUserId, int storeId, StoresRequestDto requestDto);
        Task<BaseResponse<bool>> EnableStore(int authenticatedUserId, int storeId);
        Task<BaseResponse<bool>> DisableStore(int authenticatedUserId, int storeId);
        Task<BaseResponse<bool>> RemoveStore(int authenticatedUserId, int storeId);
    }
}
