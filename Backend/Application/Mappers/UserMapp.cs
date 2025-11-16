using Application.Dtos.Request.User;
using Application.Dtos.Response.User;
using Domain.Entities;
using Infrastructure.Extensions;
using Utilities.Static;

namespace Application.Mappers
{
    public static class UserMapp
    {
        public static UserEntity UsersMapping(UserRequestDto dto)
        {
            return new UserEntity
            {
                UserName = dto.UserName,
                Names = dto.Names.NormalizeString(),
                LastNames = dto.LastNames.NormalizeString(),
                IdentificationNumber = dto.IdentificationNumber.NormalizeString(),
                PhoneNumber = dto.PhoneNumber,
                IdRole = dto.IdRole,
                IdStore = dto.IdStore
            };
        }

        public static UserResponseDto UsersResponseDtoMapping(UserEntity entity)
        {
            return new UserResponseDto
            {
                IdUser = entity.Id,
                UserName = entity.UserName,
                PasswordHash = entity.PasswordHash,
                Names = entity.Names.ToTitleCase(),
                LastNames = entity.LastNames.ToTitleCase(),
                IdentificationNumber = entity.IdentificationNumber.ToTitleCase(),
                PhoneNumber = entity.PhoneNumber,
                IdRole = entity.IdRole,
                RoleName = entity.Role?.RoleName.ToTitleCase(),
                IdStore = entity.IdStore,
                StoreName = entity.Store?.StoreName.ToTitleCase(),
                AuditCreateDate = entity.AuditCreateDate.HasValue ? entity.AuditCreateDate.Value.ToString("dd/MM/yyyy HH:mm") : null,
                Status = entity.Status,
                StatusUser = ((StateTypes)(entity.Status ? 1 : 0)).ToString()
            };

        }
    }
}
