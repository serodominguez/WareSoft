using Application.Dtos.Request.Brands;
using Application.Dtos.Response.Brands;
using Domain.Entities;

namespace Application.Mappers
{
    public static class BrandsMapp
    {
        public static Brands BrandsMapping(BrandsRequestDto dto)
        {
            return new Brands
            {
                BRAND_NAME = dto.BRAND_NAME,
            };
        }

        public static BrandsResponseDto BrandsResponseDtoMapping(Brands entity)
        {
            return new BrandsResponseDto
            {
                PK_BRAND = entity.PK_BRAND,
                BRAND_NAME = entity.BRAND_NAME,
                AUDIT_CREATE_DATE = entity.AUDIT_CREATE_DATE.HasValue ? entity.AUDIT_CREATE_DATE.Value.ToString("dd/MM/yyyy HH:mm") : null,
                STATE = entity.STATE,
                STATE_BRAND = entity.STATE ? "ACTIVO" : "INACTIVO"
            };
        }

        public static BrandsSelectResponseDto BrandsSelectResponseDtoMapping(Brands entity)
        {
            return new BrandsSelectResponseDto
            {
                PK_BRAND = entity.PK_BRAND,
                BRAND_NAME = entity.BRAND_NAME
            };
        }
    }
}
