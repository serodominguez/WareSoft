using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IBrandsRepository
    {
        Task<BaseEntityResponse<Brands>> ListBrands(BaseFiltersRequest filters);
        Task<IEnumerable<Brands>> ListSelectBrands();
        Task<Brands> BrandById(int brandId);
        Task<bool> RegisterBrand(Brands brand);
        Task<bool> EditBrand(Brands brand);
        Task<bool> EnableBrand(int brandId);
        Task<bool> DisableBrand(int brandId);
        Task<bool> RemoveBrand(int brandId);
    }
}
