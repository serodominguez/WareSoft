using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Static;

namespace Infrastructure.Persistences.Repositories
{
    public class CategoriesRepository : GenericRepository<Categories>, ICategoriesRepository
    {
        private readonly DbContextSystem _context;

        public CategoriesRepository(DbContextSystem context) 
        { 
            _context = context;
        }

        public async Task<BaseEntityResponse<Categories>> ListCategories(BaseFiltersRequest filters)
        {
            var response = new BaseEntityResponse<Categories>(); 
            var categories = (from c in _context.Categories
                              where c.AUDIT_DELETE_USER == null && c.AUDIT_DELETE_DATE == null
                              select c).AsNoTracking().AsQueryable();

            if(filters.NumberFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
            {
                switch(filters.NumberFilter)
                {
                    case 1:
                        categories = categories.Where(x => x.CATEGORY_NAME!.Contains(filters.TextFilter));
                        break;
                    case 2:
                        categories = categories.Where(x => x.DESCRIPTION!.Contains(filters.TextFilter));
                        break;
                }
            }

            if (filters.StateFilter is not null)
            {
                var stateValue = Convert.ToBoolean(filters.StateFilter);
                categories = categories.Where(x => x.STATE == stateValue);
            }

            if (!string.IsNullOrEmpty(filters.StartDate) && !string.IsNullOrEmpty(filters.EndDate))
            {
                categories = categories.Where(x => x.AUDIT_CREATE_DATE >= Convert.ToDateTime(filters.StartDate) && x.AUDIT_CREATE_DATE <= Convert.ToDateTime(filters.EndDate));
            }

            if (filters.Sort is null) filters.Sort = "PK_CATEGORY";

            response.TotalRecords = await categories.CountAsync();
            response.Items = await Ordering(filters, categories, !(bool)filters.Download!).ToListAsync();
            return response;
        }

        public async Task<IEnumerable<Categories>> ListSelectCategories()
        {
            var categories = await _context.Categories
                .Where(x => Convert.ToInt32(x.STATE).Equals((int)StateTypes.ACTIVE) && x.AUDIT_DELETE_USER == null && x.AUDIT_DELETE_DATE == null).AsNoTracking().ToListAsync();
            return categories;
        }

        public async Task<Categories> CategoryById(int categoryId)
        {
            var category = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.PK_CATEGORY.Equals(categoryId));
            return category!;
        }

        public async Task<bool> RegisterCategory(Categories category)
        {
            category.AUDIT_CREATE_USER = 1;
            category.AUDIT_CREATE_DATE = DateTime.Now;
            category.STATE = true;

            await _context.AddAsync(category);

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> EditCategory(Categories category)
        {
            category.AUDIT_UPDATE_USER = 1;
            category.AUDIT_UPDATE_DATE = DateTime.Now;

            _context.Update(category);
            _context.Entry(category).Property(x => x.STATE).IsModified = false;
            _context.Entry(category).Property(x => x.AUDIT_CREATE_USER).IsModified = false;
            _context.Entry(category).Property(x => x.AUDIT_CREATE_DATE).IsModified = false;

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> RemoveCategory(int categoryId)
        {
            var category = await _context.Categories.AsNoTracking().SingleOrDefaultAsync(x => x.PK_CATEGORY.Equals(categoryId));

            category!.AUDIT_DELETE_USER = 1;
            category.AUDIT_DELETE_DATE = DateTime.Now;
            category.STATE = false;

            _context.Update(category);

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }
    }
}
