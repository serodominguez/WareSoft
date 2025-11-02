using Application.Dtos.Request.Stores;
using Application.Dtos.Response.Stores;
using Domain.Entities;

namespace Application.Mappers
{
    public class StoresMapp
    {
        public static Stores StoresMapping(StoresRequestDto dto)
        {
            return new Stores
            {
                STORE_NAME = dto.STORE_NAME,
                MANAGER = dto.MANAGER,
                ADDRESS = dto.ADDRESS,
                PHONE_NUMBER = dto.PHONE_NUMBER,
                CITY = dto.CITY,
                EMAIL = string.IsNullOrWhiteSpace(dto.EMAIL) ? null : dto.EMAIL,
                TYPE = dto.TYPE
            };
        }

        public static StoresResponseDto StoresResponseDtoMapping(Stores entity)
        {
            return new StoresResponseDto
            {
                PK_STORE = entity.PK_ENTITY,
                STORE_NAME = entity.STORE_NAME,
                MANAGER = entity.MANAGER,
                ADDRESS = entity.ADDRESS,
                PHONE_NUMBER = entity.PHONE_NUMBER,
                CITY = entity.CITY,
                EMAIL = entity.EMAIL,
                TYPE = entity.TYPE,
                AUDIT_CREATE_DATE = entity.AUDIT_CREATE_DATE.HasValue ? entity.AUDIT_CREATE_DATE.Value.ToString("dd/MM/yyyy HH:mm") : null,
                STATE = entity.STATE,
                STATE_STORE = entity.STATE ? "ACTIVO" : "INACTIVO"
            };
        }

        public static StoresSelectResponseDto StoresSelectResponseDtoMapping(Stores entity)
        {
            return new StoresSelectResponseDto
            {
                PK_STORE = entity.PK_ENTITY,
                STORE_NAME = entity.STORE_NAME
            };
        }
    }
}
