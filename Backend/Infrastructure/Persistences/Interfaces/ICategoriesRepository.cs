using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Infrastructure.Persistences.Interfaces
{
    public interface ICategoriesRepository
    {
        Task<BaseEntityResponse<Categories>> ListCategories(BaseFiltersRequest filters);
        Task<IEnumerable<Categories>> ListSelectCategories();
        Task<Categories> CategoryById(int categoryId);
        Task<bool> RegisterCategory(Categories category);
        Task<bool> EditCategory(Categories category);
        Task<bool> EnableCategory(int categoryId);
        Task<bool> DisableCategory(int categoryId);
        Task<bool> RemoveCategory(int categoryId);
    }
}
