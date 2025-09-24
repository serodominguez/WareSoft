using Application.Commons.Bases;
using Application.Dtos.Request.Categories;
using Application.Dtos.Response.Categories;
using Application.Interfaces;
using Application.Mappers;
using FluentValidation;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Interfaces;
using Utilities.Static;

namespace Application.Services
{
    public class CategoriesApplication : ICategoriesApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CategoriesRequestDto> _validator;

        public CategoriesApplication(IUnitOfWork unitOfWork, IValidator<CategoriesRequestDto> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<BaseResponse<BaseEntityResponse<CategoriesResponseDto>>> ListCategories(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<BaseEntityResponse<CategoriesResponseDto>>();
            var categoriesEntity = await _unitOfWork.Categories.ListCategories(filters);

            if (categoriesEntity is not null && categoriesEntity.Items?.Any() == true)
            {
                var mappedItems = categoriesEntity.Items.Select(CategoriesMapp.CategoriesResponseDtoMapping).ToList();
                response.Data = new BaseEntityResponse<CategoriesResponseDto>
                {
                    TotalRecords = categoriesEntity.TotalRecords,
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

        public async Task<BaseResponse<IEnumerable<CategoriesSelectResponseDto>>> ListSelectCategories()
        {
            var response = new BaseResponse<IEnumerable<CategoriesSelectResponseDto>>();
            var categories = await _unitOfWork.Categories.ListSelectCategories();

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

            return response;
        }

        public async Task<BaseResponse<CategoriesResponseDto>> CategoryById(int categoryId)
        {
            var response = new BaseResponse<CategoriesResponseDto>();
            var category = await _unitOfWork.Categories.CategoryById(categoryId);

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

            return response;
        }

        public async Task<BaseResponse<bool>> RegisterCategory(CategoriesRequestDto requestDto)
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
            var category = CategoriesMapp.CategoriesMapping(requestDto);
            response.Data = await _unitOfWork.Categories.RegisterCategory(category);

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

        public async Task<BaseResponse<bool>> EditCategory(int categoryId, CategoriesRequestDto requestDto)
        {
            var response = new BaseResponse<bool>();
            var existingCategory = await _unitOfWork.Categories.CategoryById(categoryId);

            if (existingCategory is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }
            var category = CategoriesMapp.CategoriesMapping(requestDto);
            category.PK_CATEGORY = categoryId;
            response.Data = await _unitOfWork.Categories.EditCategory(category);

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

        public async Task<BaseResponse<bool>> RemoveCategory(int categoryId)
        {
            var response = new BaseResponse<bool>();
            var existingCategory = await _unitOfWork.Categories.CategoryById(categoryId);

            if (existingCategory is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }
            response.Data = await _unitOfWork.Categories.RemoveCategory(categoryId);

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
