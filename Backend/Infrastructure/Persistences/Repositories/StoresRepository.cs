using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Static;

namespace Infrastructure.Persistences.Repositories
{
    public class StoresRepository : GenericRepository<Stores>, IStoresRepository
    {
        private readonly DbContextSystem _context;

        public StoresRepository(DbContextSystem context)
        {
            _context = context;
        }

        public async Task<BaseEntityResponse<Stores>> ListStores(BaseFiltersRequest filters)
        {
            var response = new BaseEntityResponse<Stores>();
            var stores = (from s in _context.Stores
                              where s.AUDIT_DELETE_USER == null && s.AUDIT_DELETE_DATE == null
                              select s).AsNoTracking().AsQueryable();

            if (filters.NumberFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
            {
                switch (filters.NumberFilter)
                {
                    case 1:
                        stores = stores.Where(x => x.STORE_NAME!.Contains(filters.TextFilter));
                        break;
                    case 2:
                        stores = stores.Where(x => x.MANAGER!.Contains(filters.TextFilter));
                        break;
                    case 3:
                        stores = stores.Where(x => x.ADDRESS!.Contains(filters.TextFilter));
                        break;
                    case 4:
                        stores = stores.Where(x => x.CITY!.Contains(filters.TextFilter));
                        break;
                }
            }

            if (filters.StateFilter is not null)
            {
                var stateValue = Convert.ToBoolean(filters.StateFilter);
                stores = stores.Where(x => x.STATE == stateValue);
            }

            if (!string.IsNullOrEmpty(filters.StartDate) && !string.IsNullOrEmpty(filters.EndDate))
            {
                var startDate = Convert.ToDateTime(filters.StartDate).Date;
                var endDate = Convert.ToDateTime(filters.EndDate).Date.AddDays(1);

                stores = stores.Where(x => x.AUDIT_CREATE_DATE >= startDate && x.AUDIT_CREATE_DATE < endDate);
            }

            filters.Sort ??= "PK_STORE";

            response.TotalRecords = await stores.CountAsync();
            response.Items = await Ordering(filters, stores, !(bool)filters.Download!).ToListAsync();
            return response;
        }
        public async Task<IEnumerable<Stores>> ListSelectStores()
        {
            var stores = await _context.Stores
                    .Where(x => Convert.ToInt32(x.STATE).Equals((int)StateTypes.ACTIVE) && x.AUDIT_DELETE_USER == null && x.AUDIT_DELETE_DATE == null).AsNoTracking().ToListAsync();
            return stores;
        }
        public async Task<Stores> StoreById(int storeId)
        {
            var store = await _context.Stores.AsNoTracking().FirstOrDefaultAsync(x => x.PK_STORE.Equals(storeId));
            return store!;
        }
        public async Task<bool> RegisterStore(Stores store)
        {
            store.AUDIT_CREATE_USER = 1;
            store.AUDIT_CREATE_DATE = DateTime.Now;
            store.STATE = true;

            await _context.AddAsync(store);

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> EditStore(Stores store)
        {
            store.AUDIT_UPDATE_USER = 1;
            store.AUDIT_UPDATE_DATE = DateTime.Now;

            _context.Update(store);
            _context.Entry(store).Property(x => x.STATE).IsModified = false;
            _context.Entry(store).Property(x => x.AUDIT_CREATE_USER).IsModified = false;
            _context.Entry(store).Property(x => x.AUDIT_CREATE_DATE).IsModified = false;

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> EnableStore(int storeId)
        {
            var store = await _context.Stores.AsNoTracking().SingleOrDefaultAsync(x => x.PK_STORE.Equals(storeId));

            store!.AUDIT_UPDATE_USER = 1;
            store.AUDIT_UPDATE_DATE = DateTime.Now;
            store.STATE = true;
            _context.Update(store);

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> DisableStore(int storeId)
        {
            var store = await _context.Stores.AsNoTracking().SingleOrDefaultAsync(x => x.PK_STORE.Equals(storeId));

            store!.AUDIT_UPDATE_USER = 1;
            store.AUDIT_UPDATE_DATE = DateTime.Now;
            store.STATE = false;
            _context.Update(store);

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> RemoveStore(int storeId)
        {
            var store= await _context.Stores.AsNoTracking().SingleOrDefaultAsync(x => x.PK_STORE.Equals(storeId));

            store!.AUDIT_DELETE_USER = 1;
            store.AUDIT_DELETE_DATE = DateTime.Now;
            store.STATE = false;

            _context.Update(store);

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }


    }
}
