using Application.Dtos.Response.Permissions;
using Domain.Entities;

namespace Application.Mappers
{
    public static class PermissionsMap
    {
        public static PermissionsResponseDto PermissionsResponseDtoMapping(Permissions entity)
        {
            return new PermissionsResponseDto
            {
                ACTION = entity.Actions?.ACTION_NAME ?? string.Empty,
                MODULE = entity.Modules?.MODULE_NAME ?? string.Empty
            };
        }
    }
}
