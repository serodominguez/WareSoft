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
    public class BrandsApplication : IBrandsApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<BrandsRequestDto> _validator;
        private readonly IOrderingQuery _orderingQuery;

        public BrandsApplication(IUnitOfWork unitOfWork, IValidator<BrandsRequestDto> validator, IOrderingQuery orderingQuery)
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
                var brands = _unitOfWork.Brands.GetAllQueryable();

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

                filters.Sort ??= "PK_BRAND";
                var items = await _orderingQuery.Ordering(filters, brands, !(bool)filters.Download!).ToListAsync();
                response.IsSuccess = true;
                response.TotalRecords = await brands.CountAsync();
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
                var brands = await _unitOfWork.Brands.GetSelectAsync();

                if (brands is not null && brands.Any())
                {
                    response.Data = brands.Select(BrandsMapp.BrandsSelectResponseDtoMapping);
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

        public async Task<BaseResponse<bool>> RegisterBrand(BrandsRequestDto requestDto)
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

        public async Task<BaseResponse<bool>> EditBrand(int brandId, BrandsRequestDto requestDto)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var existingBrand = await _unitOfWork.Brands.GetByIdAsync(brandId);

                if (existingBrand is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }
                var brand = BrandsMapp.BrandsMapping(requestDto);
                brand.PK_BRAND = brandId;
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

        public async Task<BaseResponse<bool>> EnableBrand(int brandId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var existingBrand = await _unitOfWork.Brands.GetByIdAsync(brandId);

                if (existingBrand is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }
                response.Data = await _unitOfWork.Brands.EnableAsync(brandId);

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

        public async Task<BaseResponse<bool>> DisableBrand(int brandId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var existingBrand = await _unitOfWork.Brands.GetByIdAsync(brandId);

                if (existingBrand is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }
                response.Data = await _unitOfWork.Brands.DisableAsync(brandId);

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

        public async Task<BaseResponse<bool>> RemoveBrand(int brandId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var existingBrand = await _unitOfWork.Brands.GetByIdAsync(brandId);

                if (existingBrand is null)
                {
                    response.IsSuccess = false;
                    response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                    return response;
                }
                response.Data = await _unitOfWork.Brands.RemoveAsync(brandId);

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
