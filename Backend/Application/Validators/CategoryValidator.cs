using Application.Dtos.Request.Category;
using FluentValidation;

namespace Application.Validators
{
    public class CategoryValidator : AbstractValidator<CategoryRequestDto>
    {
        public CategoryValidator()
        {
            RuleFor(x => x.CategoryName)
                .NotNull().WithMessage("El campo nombre de categoría no puede ser nulo!")
                .NotEmpty().WithMessage("El campo nombre de categoría no puede estar vacio!")
                .MaximumLength(25).WithMessage("El campo nombre de categoría no puede tener más de 25 caracteres!");

            RuleFor(x => x.Description)
                .MaximumLength(50).WithMessage("El campo descripción no puede tener más de 50 caracteres!");
        }
    }
}
