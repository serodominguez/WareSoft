using Application.Dtos.Request.Users;
using Application.Dtos.Response.Users;
using Domain.Entities;

namespace Application.Mappers
{
    public class UsersMapp
    {
        public static Users UsersMapping(UsersRequestDto dto)
        {
            return new Users
            {
                USER_NAME = dto.USER_NAME,
                NAMES = dto.NAMES,
                LAST_NAMES = dto.LAST_NAMES,
                IDENTIFICATION_NUMBER = dto.IDENTIFICATION_NUMBER,
                PHONE_NUMBER = dto.PHONE_NUMBER,
                PK_ROLE = dto.PK_ROLE,
                PK_STORE = dto.PK_STORE
            };
        }

        public static UsersResponseDto UsersResponseDtoMapping(Users entity)
        {
            return new UsersResponseDto
            {
                PK_USER = entity.PK_ENTITY,
                USER_NAME = entity.USER_NAME,
                PASSWORD_HASH = entity.PASSWORD_HASH,
                NAMES = entity.NAMES,
                LAST_NAMES = entity.LAST_NAMES,
                IDENTIFICATION_NUMBER = entity.IDENTIFICATION_NUMBER,
                PHONE_NUMBER = entity.PHONE_NUMBER,
                PK_ROLE = entity.PK_ROLE,
                ROLE_NAME = entity.Roles?.ROLE_NAME,
                STORE_NAME = entity.Stores?.STORE_NAME,
                PK_STORE = entity.PK_STORE,
                AUDIT_CREATE_DATE = entity.AUDIT_CREATE_DATE.HasValue ? entity.AUDIT_CREATE_DATE.Value.ToString("dd/MM/yyyy HH:mm") : null,
                STATE = entity.STATE,
                STATE_USER = entity.STATE ? "ACTIVO" : "INACTIVO"
            };
        }
    }
}
