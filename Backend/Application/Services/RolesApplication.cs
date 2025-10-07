using Application.Commons.Bases;
using Application.Dtos.Request.Roles;
using Application.Dtos.Response.Roles;
using Application.Interfaces;
using Application.Mappers;
using FluentValidation;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Interfaces;
using Utilities.Static;

namespace Application.Services
{
    public class RolesApplication : IRolesApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<RolesRequestDto> _validator;

        public RolesApplication(IUnitOfWork unitOfWork, IValidator<RolesRequestDto> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<BaseResponse<BaseEntityResponse<RolesResponseDto>>> ListRoles(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<BaseEntityResponse<RolesResponseDto>>();
            var rolesEntity = await _unitOfWork.Roles.ListRoles(filters);

            if (rolesEntity is not null && rolesEntity.Items?.Any() == true)
            {
                var mappedItems = rolesEntity.Items.Select(RolesMapp.RolesResponseDtoMapping).ToList();
                response.Data = new BaseEntityResponse<RolesResponseDto>
                {
                    TotalRecords = rolesEntity.TotalRecords,
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

        public async Task<BaseResponse<IEnumerable<RolesSelectResponseDto>>> ListSelectRoles()
        {
            var response = new BaseResponse<IEnumerable<RolesSelectResponseDto>>();
            var roles = await _unitOfWork.Roles.ListSelectRoles();

            if (roles is not null && roles.Any())
            {
                response.Data = roles.Select(RolesMapp.RolesSelectResponseDtoMapping);
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
        public async Task<BaseResponse<RolesResponseDto>> RoleById(int roleId)
        {
            var response = new BaseResponse<RolesResponseDto>();
            var role = await _unitOfWork.Roles.RoleById(roleId);

            if (role is not null)
            {
                response.Data = RolesMapp.RolesResponseDtoMapping(role);
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

        public async Task<BaseResponse<bool>> RegisterRole(RolesRequestDto requestDto)
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
            var role = RolesMapp.RolesMapping(requestDto);
            response.Data = await _unitOfWork.Roles.RegisterRole(role);

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

        public async Task<BaseResponse<bool>> EditRole(int roleId, RolesRequestDto requestDto)
        {
            var response = new BaseResponse<bool>();
            var existingRole = await _unitOfWork.Roles.RoleById(roleId);

            if (existingRole is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }
            var role = RolesMapp.RolesMapping(requestDto);
            role.PK_ROLE = roleId;
            response.Data = await _unitOfWork.Roles.EditRole(role);

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

        public async Task<BaseResponse<bool>> EnableRole(int roleId)
        {
            var response = new BaseResponse<bool>();
            var existingRole = await _unitOfWork.Roles.RoleById(roleId);

            if (existingRole is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }
            response.Data = await _unitOfWork.Roles.EnableRole(roleId);

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

        public async Task<BaseResponse<bool>> DisableRole(int roleId)
        {
            var response = new BaseResponse<bool>();
            var existingRole = await _unitOfWork.Roles.RoleById(roleId);

            if (existingRole is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }
            response.Data = await _unitOfWork.Roles.DisableRole(roleId);

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

        public async Task<BaseResponse<bool>> RemoveRole(int roleId)
        {
            var response = new BaseResponse<bool>();
            var existingRole = await _unitOfWork.Roles.RoleById(roleId);

            if (existingRole is null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }
            response.Data = await _unitOfWork.Roles.RemoveRole(roleId);

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
