using Application.Commons.Bases;
using Application.Dtos.Request.Categories;
using Application.Dtos.Response.Categories;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Application.Interfaces
{
    public interface ICategoriesApplication
    {
        Task<BaseResponse<BaseEntityResponse<CategoriesResponseDto>>> ListCategories(BaseFiltersRequest filters);
        Task<BaseResponse<IEnumerable<CategoriesSelectResponseDto>>> ListSelectCategories();
        Task<BaseResponse<CategoriesResponseDto>> CategoryById(int categoryId);
        Task<BaseResponse<bool>> RegisterCategory(CategoriesRequestDto requestDto);
        Task<BaseResponse<bool>> EditCategory(int categoryId, CategoriesRequestDto requestDto);
        Task<BaseResponse<bool>> RemoveCategory(int categoryId);
    }
}
