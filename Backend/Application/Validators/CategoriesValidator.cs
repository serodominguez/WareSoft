using Application.Dtos.Request.Categories;
using FluentValidation;

namespace Application.Validators
{
    public class CategoriesValidator : AbstractValidator<CategoriesRequestDto>
    {
        public CategoriesValidator()
        {
            RuleFor(x => x.CATEGORY_NAME)
                .NotNull().WithMessage("El campo nombre no puede ser nulo!")
                .NotEmpty().WithMessage("El campo nombre no puede estar vacio!")
                .MaximumLength(25).WithMessage("El campo nombre no puede tener más de 25 caracteres!");

            RuleFor(x => x.DESCRIPTION)
                .MaximumLength(50).WithMessage("El campo descripción no puede tener más de 50 caracteres!");
        }
    }
}
