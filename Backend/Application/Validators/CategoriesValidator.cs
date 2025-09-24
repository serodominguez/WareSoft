using Application.Dtos.Request.Categories;
using FluentValidation;

namespace Application.Validators
{
    public class CategoriesValidator : AbstractValidator<CategoriesRequestDto>
    {
        public CategoriesValidator()
        {
            RuleFor(x => x.CATEGORY_NAME)
                .NotNull().WithMessage("EL campo nombre no puede ser nulo!")
                .NotEmpty().WithMessage("El campo nombre no puede estar vacio!");
        }
    }
}
