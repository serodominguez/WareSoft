using Application.Commons.Bases;
using Application.Dtos.Request.Stores;
using Application.Dtos.Response.Stores;
using Application.Interfaces;
using Application.Mappers;
using FluentValidation;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Interfaces;
using Utilities.Static;

namespace Application.Services
{
    public class StoresApplication : IStoresApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<StoresRequestDto> _validator;

        public  StoresApplication(IUnitOfWork unitOfWork, IValidator<StoresRequestDto> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<BaseResponse<BaseEntityResponse<StoresResponseDto>>> ListStores(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<BaseEntityResponse<StoresResponseDto>>();
            var storesEntity = await _unitOfWork.Stores.ListStores(filters);

            if (storesEntity is not null && storesEntity.Items?.Any() == true)
            {
                var mappedItems = storesEntity.Items.Select(StoresMapp.StoresResponseDtoMapping).ToList();
                response.Data = new BaseEntityResponse<StoresResponseDto>
                {
                    TotalRecords = storesEntity.TotalRecords,
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
        public async Task<BaseResponse<IEnumerable<StoresSelectResponseDto>>> ListSelectStores()
        {
            var response = new BaseResponse<IEnumerable<StoresSelectResponseDto>>();
            var stores = await _unitOfWork.Stores.ListSelectStores();

            if (stores is not null && stores.Any())
            {
                response.Data = stores.Select(StoresMapp.StoresSelectResponseDtoMapping);
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

        public async Task<BaseResponse<StoresResponseDto>> StoreById(int storeId)
        {
            var response = new BaseResponse<StoresResponseDto>();
            var store = await _unitOfWork.Stores.StoreById(storeId);

            if (store is not null)
            {
                response.Data = StoresMapp.StoresResponseDtoMapping(store);
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
        public async Task<BaseResponse<bool>> RegisterStore(StoresRequestDto requestDto)
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
            var store = StoresMapp.StoresMapping(requestDto);
            response.Data = await _unitOfWork.Stores.RegisterStore(store);

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

        public async Task<BaseResponse<bool>> EditStore(int storeId, StoresRequestDto requestDto)
        {
            var response = new BaseResponse<bool>();
            var existingStore = await _unitOfWork.Stores.StoreById(storeId);

            if (existingStore is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }
            var store = StoresMapp.StoresMapping(requestDto);
            store.PK_STORE = storeId;
            response.Data = await _unitOfWork.Stores.EditStore(store);

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

        public async Task<BaseResponse<bool>> EnableStore(int storeId)
        {
            var response = new BaseResponse<bool>();
            var existingStore = await _unitOfWork.Stores.StoreById(storeId);

            if (existingStore is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }
            response.Data = await _unitOfWork.Stores.EnableStore(storeId);

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

        public async Task<BaseResponse<bool>> DisableStore(int storeId)
        {
            var response = new BaseResponse<bool>();
            var existingStore = await _unitOfWork.Stores.StoreById(storeId);

            if (existingStore is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }
            response.Data = await _unitOfWork.Stores.DisableStore(storeId);

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

        public async Task<BaseResponse<bool>> RemoveStore(int storeId)
        {
            var response = new BaseResponse<bool>();
            var existingStore = await _unitOfWork.Stores.StoreById(storeId);

            if (existingStore is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }
            response.Data = await _unitOfWork.Stores.RemoveStore(storeId);

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
