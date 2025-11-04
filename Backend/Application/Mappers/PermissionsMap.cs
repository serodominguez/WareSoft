using Application.Dtos.Request.Permissions;
using Application.Dtos.Response.Permissions;
using Domain.Entities;

namespace Application.Mappers
{
    public static class PermissionsMap
    {
        public static Permissions PermissionsMapping(PermissionsRequestDto dto)
        {
            return new Permissions
            {
                PK_ENTITY = dto.PK_PERMISSION,
                STATE = dto.STATE
            };
        }

        public static PermissionsByUserResponseDto PermissionsByUserResponseDtoMapping(Permissions entity)
        {
            return new PermissionsByUserResponseDto
            {
                ACTION = entity.Actions?.ACTION_NAME ?? string.Empty,
                MODULE = entity.Modules?.MODULE_NAME ?? string.Empty
            };
        }

        public static PermissionsByRoleResponseDto PermissionsByRoleResponseDtoMapping(Permissions entity)
        {
            return new PermissionsByRoleResponseDto
            {
                PK_PERMISSION = entity.PK_ENTITY,
                PK_ROLE = entity.PK_ROLE,
                PK_MODULE = entity.PK_MODULE,
                MODULE_NAME = entity.Modules?.MODULE_NAME,
                PK_ACTION = entity.PK_ACTION,
                ACTION_NAME = entity.Actions?.ACTION_NAME,
                STATE = entity.STATE

            };
        }
    }
}
