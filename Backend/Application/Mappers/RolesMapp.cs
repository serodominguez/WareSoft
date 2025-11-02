using Application.Dtos.Request.Roles;
using Application.Dtos.Response.Roles;
using Domain.Entities;

namespace Application.Mappers
{
    public static class RolesMapp
    {
        public static Roles RolesMapping(RolesRequestDto dto)
        {
            return new Roles
            {
                ROLE_NAME = dto.ROLE_NAME,
            };
        }

        public static RolesResponseDto RolesResponseDtoMapping(Roles entity)
        {
            return new RolesResponseDto
            {
                PK_ROLE = entity.PK_ENTITY,
                ROLE_NAME = entity.ROLE_NAME,
                AUDIT_CREATE_DATE = entity.AUDIT_CREATE_DATE.HasValue ? entity.AUDIT_CREATE_DATE.Value.ToString("dd/MM/yyyy HH:mm") : null,
                STATE = entity.STATE,
                STATE_ROLE = entity.STATE ? "ACTIVO" : "INACTIVO"
            };
        }

        public static RolesSelectResponseDto RolesSelectResponseDtoMapping(Roles entity)
        {
            return new RolesSelectResponseDto
            {
                PK_ROLE = entity.PK_ENTITY,
                ROLE_NAME = entity.ROLE_NAME
            };
        }
    }
}
