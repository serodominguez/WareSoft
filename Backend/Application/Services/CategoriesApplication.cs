using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Commons.Ordering;
using Application.Dtos.Request.Categories;
using Application.Dtos.Response.Categories;
using Application.Interfaces;
using Application.Mappers;
using FluentValidation;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Static;

namespace Application.Services
{
    public class CategoriesApplication : ICategoriesApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CategoriesRequestDto> _validator;
        private readonly IOrderingQuery _orderingQuery;

        public CategoriesApplication(IUnitOfWork unitOfWork, IValidator<CategoriesRequestDto> validator, IOrderingQuery orderingQuery)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _orderingQuery = orderingQuery;
        }

        public async Task<BaseResponse<IEnumerable<CategoriesResponseDto>>> ListCategories(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<IEnumerable<CategoriesResponseDto>>();

            try
            {
                var categories = _unitOfWork.Categories.GetAllQueryable();

                if (filters.NumberFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
                {
                    switch (filters.NumberFilter)
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
                    var startDate = Convert.ToDateTime(filters.StartDate).Date;
                    var endDate = Convert.ToDateTime(filters.EndDate).Date.AddDays(1);

                    categories = categories.Where(x => x.AUDIT_CREATE_DATE >= startDate && x.AUDIT_CREATE_DATE < endDate);
                }
                response.TotalRecords = await categories.CountAsync();

                filters.Sort ??= "PK_CATEGORY";
                var items = await _orderingQuery.Ordering(filters, categories, !(bool)filters.Download!).ToListAsync();
                response.IsSuccess = true;
                response.Data = items.Select(CategoriesMapp.CategoriesResponseDtoMapping);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex) 
            { 
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<IEnumerable<CategoriesSelectResponseDto>>> ListSelectCategories()
        {
            var response = new BaseResponse<IEnumerable<CategoriesSelectResponseDto>>();

            try
            {
                var categories = await _unitOfWork.Categories.GetSelectAsync();
                if (categories is not null && categories.Any())
                {
                    response.Data = categories.Select(CategoriesMapp.CategoriesSelectResponseDtoMapping);
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_QUERY;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                }
            }
            catch (Exception ex) 
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<CategoriesResponseDto>> CategoryById(int categoryId)
        {
            var response = new BaseResponse<CategoriesResponseDto>();

            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
                if (category is not null)
                {
                    response.Data = CategoriesMapp.CategoriesResponseDtoMapping(category);
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_QUERY;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RegisterCategory(int authenticatedUserId, CategoriesRequestDto requestDto)
        {
            var response = new BaseResponse<bool>();
            try
            {
                var validationResult = await _validator.ValidateAsync(requestDto);
                if (!validationResult.IsValid)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_VALIDATE;
                    response.Errors = validationResult.Errors;
                    return response;
                }

                var category = CategoriesMapp.CategoriesMapping(requestDto);
                response.Data = await _unitOfWork.Categories.RegisterAsync(authenticatedUserId, category);
                if (response.Data)
                {
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_SAVE;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_FAILED;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> EditCategory(int authenticatedUserId, int categoryId, CategoriesRequestDto requestDto)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var validationResult = await _validator.ValidateAsync(requestDto);
                if (!validationResult.IsValid)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_VALIDATE;
                    response.Errors = validationResult.Errors;
                    return response;
                }

                var isValid = await _unitOfWork.Categories.GetByIdAsync(categoryId);
                if (isValid is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }

                var category = CategoriesMapp.CategoriesMapping(requestDto);
                category.PK_CATEGORY = categoryId;
                response.Data = await _unitOfWork.Categories.EditAsync(authenticatedUserId, category);
                if (response.Data)
                {
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_UPDATE;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_FAILED;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<bool>> EnableCategory(int authenticatedUserId, int categoryId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var existingCategory = await _unitOfWork.Categories.GetByIdAsync(categoryId);
                if (existingCategory is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }

                response.Data = await _unitOfWork.Categories.EnableAsync(authenticatedUserId, categoryId);
                if (response.Data)
                {
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_ACTIVATE;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_FAILED;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }
        public async Task<BaseResponse<bool>> DisableCategory(int authenticatedUserId, int categoryId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var existingCategory = await _unitOfWork.Categories.GetByIdAsync(categoryId);
                if (existingCategory is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }

                response.Data = await _unitOfWork.Categories.DisableAsync(authenticatedUserId, categoryId);
                if (response.Data)
                {
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_INACTIVATE;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_FAILED;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RemoveCategory(int authenticatedUserId, int categoryId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var existingCategory = await _unitOfWork.Categories.GetByIdAsync(categoryId);
                if (existingCategory is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }

                response.Data = await _unitOfWork.Categories.RemoveAsync(authenticatedUserId, categoryId);
                if (response.Data)
                {
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_DELETE;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_FAILED;
                }
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
