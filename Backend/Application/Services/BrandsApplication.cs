using Application.Commons.Bases;
using Application.Dtos.Request.Brands;
using Application.Dtos.Response.Brands;
using Application.Interfaces;
using Application.Mappers;
using FluentValidation;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Interfaces;
using Utilities.Static;

namespace Application.Services
{
    public class BrandsApplication : IBrandsApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<BrandsRequestDto> _validator;

        public BrandsApplication(IUnitOfWork unitOfWork, IValidator<BrandsRequestDto> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<BaseResponse<BaseEntityResponse<BrandsResponseDto>>> ListBrands(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<BaseEntityResponse<BrandsResponseDto>>();
            var brandsEntity = await _unitOfWork.Brands.ListBrands(filters);

            if (brandsEntity is not null && brandsEntity.Items?.Any() == true)
            {
                var mappedItems = brandsEntity.Items.Select(BrandsMapp.BrandsResponseDtoMapping).ToList();
                response.Data = new BaseEntityResponse<BrandsResponseDto>
                {
                    TotalRecords = brandsEntity.TotalRecords,
                    Items = mappedItems
                };
                response.IsSuccess = true;
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<IEnumerable<BrandsSelectResponseDto>>> ListSelectBrands()
        {
            var response = new BaseResponse<IEnumerable<BrandsSelectResponseDto>>();
            var brands = await _unitOfWork.Brands.ListSelectBrands();

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

            return response;
        }

        public async Task<BaseResponse<BrandsResponseDto>> BrandById(int brandId)
        {
            var response = new BaseResponse<BrandsResponseDto>();
            var brand = await _unitOfWork.Brands.BrandById(brandId);

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

            return response;
        }

        public async Task<BaseResponse<bool>> RegisterBrand(BrandsRequestDto requestDto)
        {
            var response = new BaseResponse<bool>();
            var validationResult = await _validator.ValidateAsync(requestDto);

            if (!validationResult.IsValid)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_VALIDATE;
                response.Errors = validationResult.Errors;
                return response;
            }
            var brand = BrandsMapp.BrandsMapping(requestDto);
            response.Data = await _unitOfWork.Brands.RegisterBrand(brand);

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

            return response;
        }

        public async Task<BaseResponse<bool>> EditBrand(int brandId, BrandsRequestDto requestDto)
        {
            var response = new BaseResponse<bool>();
            var existingBrand = await _unitOfWork.Brands.BrandById(brandId);

            if (existingBrand is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }
            var brand = BrandsMapp.BrandsMapping(requestDto);
            brand.PK_BRAND = brandId;
            response.Data = await _unitOfWork.Brands.EditBrand(brand);

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

            return response;
        }

        public async Task<BaseResponse<bool>> EnableBrand(int brandId)
        {
            var response = new BaseResponse<bool>();
            var existingBrand = await _unitOfWork.Brands.BrandById(brandId);

            if (existingBrand is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }
            response.Data = await _unitOfWork.Brands.EnableBrand(brandId);

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

            return response;
        }

        public async Task<BaseResponse<bool>> DisableBrand(int brandId)
        {
            var response = new BaseResponse<bool>();
            var existingBrand = await _unitOfWork.Brands.BrandById(brandId);

            if (existingBrand is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }
            response.Data = await _unitOfWork.Brands.DisableBrand(brandId);

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

            return response;
        }

        public async Task<BaseResponse<bool>> RemoveBrand(int brandId)
        {
            var response = new BaseResponse<bool>();
            var existingBrand = await _unitOfWork.Brands.BrandById(brandId);

            if (existingBrand is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }
            response.Data = await _unitOfWork.Brands.RemoveBrand(brandId);

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

            return response;
        }
    }
}
