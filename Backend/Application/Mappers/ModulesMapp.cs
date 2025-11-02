using Application.Dtos.Request.Modules;
using Application.Dtos.Response.Modules;
using Domain.Entities;

namespace Application.Mappers
{
    public static class ModulesMapp
    {
        public static Modules ModulesMapping(ModulesRequestDto dto)
        {
            return new Modules
            {
                MODULE_NAME = dto.MODULE_NAME,
            };
        }

        public static ModulesResponseDto ModulesResponseDtoMapping(Modules entity)
        {
            return new ModulesResponseDto
            {
                PK_MODULE = entity.PK_ENTITY,
                MODULE_NAME = entity.MODULE_NAME,
                AUDIT_CREATE_DATE = entity.AUDIT_CREATE_DATE.HasValue ? entity.AUDIT_CREATE_DATE.Value.ToString("dd/MM/yyyy HH:mm") : null,
                STATE = entity.STATE,
                STATE_MODULE = entity.STATE ? "ACTIVO" : "INACTIVO"
            };
        }
    }
}
