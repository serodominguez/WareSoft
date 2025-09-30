using Application.Dtos.Request.Categories;
using Application.Dtos.Response.Categories;
using Domain.Entities;

namespace Application.Mappers
{
    public static class CategoriesMapp
    {
        public static Categories CategoriesMapping(CategoriesRequestDto dto)
        {
            return new Categories
            {
                CATEGORY_NAME = dto.CATEGORY_NAME,
                DESCRIPTION = dto.DESCRIPTION
            };
        }
        public static CategoriesResponseDto CategoriesResponseDtoMapping(Categories entity)
        {
            return new CategoriesResponseDto
            {
                PK_CATEGORY = entity.PK_CATEGORY,
                CATEGORY_NAME = entity.CATEGORY_NAME,
                DESCRIPTION = entity.DESCRIPTION,
                AUDIT_CREATE_DATE = entity.AUDIT_CREATE_DATE.HasValue ? entity.AUDIT_CREATE_DATE.Value.ToString("dd/MM/yyyy HH:mm") : null,
                STATE = entity.STATE,
                STATE_CATEGORY = entity.STATE ? "ACTIVO" : "INACTIVO"
            };
        }
        public static CategoriesSelectResponseDto CategoriesSelectResponseDtoMapping(Categories entity)
        {
            return new CategoriesSelectResponseDto
            {
                PK_CATEGORY = entity.PK_CATEGORY,
                CATEGORY_NAME = entity.CATEGORY_NAME
            };
        }
    }
}
