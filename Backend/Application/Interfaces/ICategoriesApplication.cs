using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Dtos.Request.Categories;
using Application.Dtos.Response.Categories;

namespace Application.Interfaces
{
    public interface ICategoriesApplication
    {
        Task<BaseResponse<IEnumerable<CategoriesResponseDto>>> ListCategories(BaseFiltersRequest filters);
        Task<BaseResponse<IEnumerable<CategoriesSelectResponseDto>>> ListSelectCategories();
        Task<BaseResponse<CategoriesResponseDto>> CategoryById(int categoryId);
        Task<BaseResponse<bool>> RegisterCategory(CategoriesRequestDto requestDto);
        Task<BaseResponse<bool>> EditCategory(int categoryId, CategoriesRequestDto requestDto);
        Task<BaseResponse<bool>> EnableCategory(int categoryId);
        Task<BaseResponse<bool>> DisableCategory(int categoryId);
        Task<BaseResponse<bool>> RemoveCategory(int categoryId);
    }
}
