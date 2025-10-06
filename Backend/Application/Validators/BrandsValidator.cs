using Application.Dtos.Request.Brands;
using FluentValidation;

namespace Application.Validators
{
    public class BrandsValidator : AbstractValidator<BrandsRequestDto>
    {
        public BrandsValidator() 
        {
            RuleFor(x => x.BRAND_NAME)
                .NotNull().WithMessage("EL campo nombre no puede ser nulo!")
                .NotEmpty().WithMessage("El campo nombre no puede estar vacio!");
        }
    }
}
