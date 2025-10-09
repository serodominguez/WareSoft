using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IStoresRepository
    {
        Task<BaseEntityResponse<Stores>> ListStores(BaseFiltersRequest filters);
        Task<IEnumerable<Stores>> ListSelectStores();
        Task<Stores> StoreById(int storeId);
        Task<bool> RegisterStore(Stores store);
        Task<bool> EditStore(Stores store);
        Task<bool> EnableStore(int storeId);
        Task<bool> DisableStore(int storeId);
        Task<bool> RemoveStore(int storeId);
    }
}
