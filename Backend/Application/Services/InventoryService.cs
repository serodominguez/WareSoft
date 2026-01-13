using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Commons.Ordering;
using Application.Dtos.Response.Inventory;
using Application.Interfaces;
using Application.Mappers;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Static;

namespace Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderingQuery _orderingQuery;

        public InventoryService(IUnitOfWork unitOfWork, IOrderingQuery orderingQuery)
        {
            _unitOfWork = unitOfWork;
            _orderingQuery = orderingQuery;
        }

        public async Task<BaseResponse<IEnumerable<StockByStoreResponseDto>>> ListStockByStore(int storeId, BaseFiltersRequest filters)
        {
            var response = new BaseResponse<IEnumerable<StockByStoreResponseDto>>();
            try
            {
                var inventory = _unitOfWork.Inventory.GetStockByStoreQueryable(storeId);

                if (filters.NumberFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
                {
                    switch (filters.NumberFilter)
                    {
                        case 1:
                            inventory = inventory.Where(x => x.Product.Code!.Contains(filters.TextFilter));
                            break;
                        case 2:
                            inventory = inventory.Where(x => x.Product.Color!.Contains(filters.TextFilter));
                            break;
                        case 3:
                            inventory = inventory.Where(x => x.Product.Brand.BrandName!.Contains(filters.TextFilter));
                            break;
                        case 4:
                            inventory = inventory.Where(x => x.Product.Category.CategoryName!.Contains(filters.TextFilter));
                            break;
                    }
                }

                response.TotalRecords = await inventory.CountAsync();
                filters.Sort ??= "Id";
                var items = await _orderingQuery.Ordering(filters, inventory, true).ToListAsync();
                response.IsSuccess = true;
                response.Data = items.Select(InventoryMapp.StockByStoreMapping);
                response.Message = ReplyMessage.MESSAGE_QUERY;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }
    }
}

