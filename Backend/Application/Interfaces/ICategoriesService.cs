using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Dtos.Request.Categories;
using Application.Dtos.Response.Categories;

namespace Application.Interfaces
{
    public interface ICategoriesService
    {
        Task<BaseResponse<IEnumerable<CategoriesResponseDto>>> ListCategories(BaseFiltersRequest filters);
        Task<BaseResponse<IEnumerable<CategoriesSelectResponseDto>>> ListSelectCategories();
        Task<BaseResponse<CategoriesResponseDto>> CategoryById(int categoryId);
        Task<BaseResponse<bool>> RegisterCategory(int authenticatedUserId, CategoriesRequestDto requestDto);
        Task<BaseResponse<bool>> EditCategory(int authenticatedUserId, int categoryId, CategoriesRequestDto requestDto);
        Task<BaseResponse<bool>> EnableCategory(int authenticatedUserId,int categoryId);
        Task<BaseResponse<bool>> DisableCategory(int authenticatedUserId, int categoryId);
        Task<BaseResponse<bool>> RemoveCategory(int authenticatedUserId, int categoryId);
    }
}
