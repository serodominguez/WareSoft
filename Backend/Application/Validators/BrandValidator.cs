using Application.Dtos.Request.Brand;
using FluentValidation;

namespace Application.Validators
{
    public class BrandValidator : AbstractValidator<BrandRequestDto>
    {
        public BrandValidator()
        {
            RuleFor(x => x.BrandName)
                .NotNull().WithMessage("El campo nombre de marca no puede ser nulo!")
                .NotEmpty().WithMessage("El campo nombre de marca no puede estar vacio!")
                .MaximumLength(25).WithMessage("El campo nombre de marca no puede tener más de 25 caracteres!")
                .Matches("^[a-zA-Z0-9 ]+$");

        }
    }
}
