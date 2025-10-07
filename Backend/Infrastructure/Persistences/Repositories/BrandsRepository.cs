using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Static;

namespace Infrastructure.Persistences.Repositories
{
    public class BrandsRepository : GenericRepository<Brands>, IBrandsRepository
    {
        private readonly DbContextSystem _context;

        public BrandsRepository(DbContextSystem context)
        {
            _context = context;
        }

        public async Task<BaseEntityResponse<Brands>> ListBrands(BaseFiltersRequest filters)
        {
            var response = new BaseEntityResponse<Brands>();
            var brands = (from b in _context.Brands
                              where b.AUDIT_DELETE_USER == null && b.AUDIT_DELETE_DATE == null
                              select b).AsNoTracking().AsQueryable();

            if (filters.NumberFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
            {
                switch (filters.NumberFilter)
                {
                    case 1:
                        brands = brands.Where(x => x.BRAND_NAME!.Contains(filters.TextFilter));
                        break;
                }
            }

            if (filters.StateFilter is not null)
            {
                var stateValue = Convert.ToBoolean(filters.StateFilter);
                brands = brands.Where(x => x.STATE == stateValue);
            }

            if (!string.IsNullOrEmpty(filters.StartDate) && !string.IsNullOrEmpty(filters.EndDate))
            {
                var startDate = Convert.ToDateTime(filters.StartDate).Date;
                var endDate = Convert.ToDateTime(filters.EndDate).Date.AddDays(1);

                brands = brands.Where(x => x.AUDIT_CREATE_DATE >= startDate && x.AUDIT_CREATE_DATE <= endDate);
            }

            if (filters.Sort is null) filters.Sort = "PK_BRAND";

            response.TotalRecords = await brands.CountAsync();
            response.Items = await Ordering(filters, brands, !(bool)filters.Download!).ToListAsync();
            return response;
        }

        public async Task<IEnumerable<Brands>> ListSelectBrands()
        {
            var brands = await _context.Brands
                .Where(x => Convert.ToInt32(x.STATE).Equals((int)StateTypes.ACTIVE) && x.AUDIT_DELETE_USER == null && x.AUDIT_DELETE_DATE == null).AsNoTracking().ToListAsync();
            return brands;
        }

        public async Task<Brands> BrandById(int brandId)
        {
            var brand = await _context.Brands.AsNoTracking().FirstOrDefaultAsync(x => x.PK_BRAND.Equals(brandId));
            return brand!;
        }

        public async Task<bool> RegisterBrand(Brands brand)
        {
            brand.AUDIT_CREATE_USER = 1;
            brand.AUDIT_CREATE_DATE = DateTime.Now;
            brand.STATE = true;

            await _context.AddAsync(brand);

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> EditBrand(Brands brand)
        {
            brand.AUDIT_UPDATE_USER = 1;
            brand.AUDIT_UPDATE_DATE = DateTime.Now;

            _context.Update(brand);
            _context.Entry(brand).Property(x => x.STATE).IsModified = false;
            _context.Entry(brand).Property(x => x.AUDIT_CREATE_USER).IsModified = false;
            _context.Entry(brand).Property(x => x.AUDIT_CREATE_DATE).IsModified = false;

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> EnableBrand(int brandId)
        {
            var brand = await _context.Brands.AsNoTracking().SingleOrDefaultAsync(x => x.PK_BRAND.Equals(brandId));

            brand!.AUDIT_UPDATE_USER = 1;
            brand.AUDIT_UPDATE_DATE = DateTime.Now;
            brand.STATE = true;
            _context.Update(brand);

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> DisableBrand(int brandId)
        {
            var brand= await _context.Brands.AsNoTracking().SingleOrDefaultAsync(x => x.PK_BRAND.Equals(brandId));

            brand!.AUDIT_UPDATE_USER = 1;
            brand.AUDIT_UPDATE_DATE = DateTime.Now;
            brand.STATE = false;
            _context.Update(brand);

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> RemoveBrand(int brandId)
        {
            var brand = await _context.Brands.AsNoTracking().SingleOrDefaultAsync(x => x.PK_BRAND.Equals(brandId));

            brand!.AUDIT_DELETE_USER = 1;
            brand.AUDIT_DELETE_DATE = DateTime.Now;
            brand.STATE = false;

            _context.Update(brand);

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }
    }
}
