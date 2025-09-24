using Infrastructure.Commons.Bases.Request;
using Infrastructure.Helpers;
using Infrastructure.Persistences.Interfaces;
using System.Linq.Dynamic.Core;

namespace Infrastructure.Persistences.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected IQueryable<Tdto> Ordering<Tdto>(BasePaginationRequest request, IQueryable<Tdto> queryable, bool pagination = false) where Tdto : class
        {
            IQueryable<Tdto> queryDto = request.Order == "desc" ? queryable.OrderBy($"{request.Sort} descending") : queryable.OrderBy($"{request.Sort} ascending");

            if (pagination) queryDto = queryDto.Paginate(request);
            
            return queryDto;
        }
    }
}
