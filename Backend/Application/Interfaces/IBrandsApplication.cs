using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Dtos.Request.Brands;
using Application.Dtos.Response.Brands;

namespace Application.Interfaces
{
    public interface IBrandsApplication
    {
        Task<BaseResponse<IEnumerable<BrandsResponseDto>>> ListBrands(BaseFiltersRequest filters);
        Task<BaseResponse<IEnumerable<BrandsSelectResponseDto>>> ListSelectBrands();
        Task<BaseResponse<BrandsResponseDto>> BrandById(int brandId);
        Task<BaseResponse<bool>> RegisterBrand(BrandsRequestDto requestDto);
        Task<BaseResponse<bool>> EditBrand(int brandId, BrandsRequestDto requestDto);
        Task<BaseResponse<bool>> EnableBrand(int brandId);
        Task<BaseResponse<bool>> DisableBrand(int brandId);
        Task<BaseResponse<bool>> RemoveBrand(int brandId);
    }
}
