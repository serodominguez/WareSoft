using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Dtos.Request.Brands;
using Application.Dtos.Response.Brands;

namespace Application.Interfaces
{
    public interface IBrandsService
    {
        Task<BaseResponse<IEnumerable<BrandsResponseDto>>> ListBrands(BaseFiltersRequest filters);
        Task<BaseResponse<IEnumerable<BrandsSelectResponseDto>>> ListSelectBrands();
        Task<BaseResponse<BrandsResponseDto>> BrandById(int brandId);
        Task<BaseResponse<bool>> RegisterBrand(int authenticatedUserId, BrandsRequestDto requestDto);
        Task<BaseResponse<bool>> EditBrand(int authenticatedUserId, int brandId, BrandsRequestDto requestDto);
        Task<BaseResponse<bool>> EnableBrand(int authenticatedUserId, int brandId);
        Task<BaseResponse<bool>> DisableBrand(int authenticatedUserId, int brandId);
        Task<BaseResponse<bool>> RemoveBrand(int authenticatedUserId, int brandId);
    }
}
