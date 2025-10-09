using Application.Commons.Bases;
using Application.Dtos.Request.Stores;
using Application.Dtos.Response.Stores;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Application.Interfaces
{
    public interface IStoresApplication
    {
        Task<BaseResponse<BaseEntityResponse<StoresResponseDto>>> ListStores(BaseFiltersRequest filters);
        Task<BaseResponse<IEnumerable<StoresSelectResponseDto>>> ListSelectStores();
        Task<BaseResponse<StoresResponseDto>> StoreById(int storeId);
        Task<BaseResponse<bool>> RegisterStore(StoresRequestDto requestDto);
        Task<BaseResponse<bool>> EditStore(int storeId, StoresRequestDto requestDto);
        Task<BaseResponse<bool>> EnableStore(int storeId);
        Task<BaseResponse<bool>> DisableStore(int storeId);
        Task<BaseResponse<bool>> RemoveStore(int storeId);
    }
}
