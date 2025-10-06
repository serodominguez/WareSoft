using Application.Commons.Bases;
using Application.Dtos.Request.Brands;
using Application.Dtos.Response.Brands;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Application.Interfaces
{
    public interface IBrandsApplication
    {
        Task<BaseResponse<BaseEntityResponse<BrandsResponseDto>>> ListBrands(BaseFiltersRequest filters);
        Task<BaseResponse<IEnumerable<BrandsSelectResponseDto>>> ListSelectBrands();
        Task<BaseResponse<BrandsResponseDto>> BrandById(int brandId);
        Task<BaseResponse<bool>> RegisterBrand(BrandsRequestDto requestDto);
        Task<BaseResponse<bool>> EditBrand(int brandId, BrandsRequestDto requestDto);
        Task<BaseResponse<bool>> EnableBrand(int brandId);
        Task<BaseResponse<bool>> DisableBrand(int brandId);
        Task<BaseResponse<bool>> RemoveBrand(int brandId);
    }
}
