using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.Commons.Ordering;
using Application.Dtos.Request.Brands;
using Application.Dtos.Response.Brands;
using Application.Interfaces;
using Application.Mappers;
using FluentValidation;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utilities.Static;

namespace Application.Services
{
    public class BrandsService : IBrandsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<BrandsRequestDto> _validator;
        private readonly IOrderingQuery _orderingQuery;

        public BrandsService(IUnitOfWork unitOfWork, IValidator<BrandsRequestDto> validator, IOrderingQuery orderingQuery)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _orderingQuery = orderingQuery;
        }

        public async Task<BaseResponse<IEnumerable<BrandsResponseDto>>> ListBrands(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<IEnumerable<BrandsResponseDto>>();

            try
            {
                var brands = _unitOfWork.Brands.GetAllQueryable()
                                        .Where(b => b.AUDIT_DELETE_USER == null && b.AUDIT_DELETE_DATE == null);

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

                    brands = brands.Where(x => x.AUDIT_CREATE_DATE >= startDate && x.AUDIT_CREATE_DATE < endDate);
                }
                response.TotalRecords = await brands.CountAsync();

                filters.Sort ??= "PK_ENTITY";
                var items = await _orderingQuery.Ordering(filters, brands, !(bool)filters.Download!).ToListAsync();
                response.IsSuccess = true;
                response.Data = items.Select(BrandsMapp.BrandsResponseDtoMapping);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<IEnumerable<BrandsSelectResponseDto>>> ListSelectBrands()
        {
            var response = new BaseResponse<IEnumerable<BrandsSelectResponseDto>>();

            try
            {
                var brands = (await _unitOfWork.Brands.GetSelectAsync())
                                                .Where(b => b.STATE== true && b.AUDIT_DELETE_USER == null && b.AUDIT_DELETE_DATE == null);

                if (brands is not null && brands.Any())
                {
                    response.Data = brands.Select(BrandsMapp.BrandsSelectResponseDtoMapping);
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_QUERY;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<BrandsResponseDto>> BrandById(int brandId)
        {
            var response = new BaseResponse<BrandsResponseDto>();

            try
            {
                var brand = await _unitOfWork.Brands.GetByIdAsync(brandId);

                if (brand is not null)
                {
                    response.Data = BrandsMapp.BrandsResponseDtoMapping(brand);
                    response.IsSuccess = true;
                    response.Message = ReplyMessage.MESSAGE_QUERY;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_EXCEPTION + ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> RegisterBrand(int authenticatedUserId, BrandsRequestDto requestDto)
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

                var brand = BrandsMapp.BrandsMapping(requestDto);
                brand.AUDIT_CREATE_USER = authenticatedUserId;
                brand.AUDIT_CREATE_DATE = DateTime.Now;
                brand.STATE = true;

                response.Data = await _unitOfWork.Brands.RegisterAsync(brand);
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

        public async Task<BaseResponse<bool>> EditBrand(int authenticatedUserId, int brandId, BrandsRequestDto requestDto)
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

                var isValid = await _unitOfWork.Brands.GetByIdAsync(brandId);
                if (isValid is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                var brand = BrandsMapp.BrandsMapping(requestDto);
                brand.PK_ENTITY = brandId;
                brand.AUDIT_UPDATE_USER = authenticatedUserId;
                brand.AUDIT_UPDATE_DATE = DateTime.Now;

                response.Data = await _unitOfWork.Brands.EditAsync(brand);

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

        public async Task<BaseResponse<bool>> EnableBrand(int authenticatedUserId, int brandId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var brand = await _unitOfWork.Brands.GetByIdAsync(brandId);

                if (brand is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                brand.AUDIT_UPDATE_USER = authenticatedUserId;
                brand.AUDIT_UPDATE_DATE = DateTime.Now;
                brand.STATE = true;

                response.Data = await _unitOfWork.Brands.UpdateAsync(brand);

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

        public async Task<BaseResponse<bool>> DisableBrand(int authenticatedUserId, int brandId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var brand = await _unitOfWork.Brands.GetByIdAsync(brandId);

                if (brand is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                brand.AUDIT_UPDATE_USER = authenticatedUserId;
                brand.AUDIT_UPDATE_DATE = DateTime.Now;
                brand.STATE = false;

                response.Data = await _unitOfWork.Brands.UpdateAsync(brand);

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

        public async Task<BaseResponse<bool>> RemoveBrand(int authenticatedUserId, int brandId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var brand = await _unitOfWork.Brands.GetByIdAsync(brandId);

                if (brand is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_NOT_FOUND;
                    return response;
                }

                brand.AUDIT_DELETE_USER = authenticatedUserId;
                brand.AUDIT_DELETE_DATE = DateTime.Now;
                brand.STATE = false;

                response.Data = await _unitOfWork.Brands.RemoveAsync(brand);

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
