using Application.Dtos.Request.Module;
using FluentValidation;

namespace Application.Validators
{
    public class ModuleValidator : AbstractValidator<ModuleRequestDto>
    {
        public ModuleValidator()
        {
            RuleFor(x => x.ModuleName)
                .NotNull().WithMessage("El campo nombre de módulo no puede ser nulo!")
                .NotEmpty().WithMessage("El campo nombre de módul no puede estar vacio!")
                .MaximumLength(25).WithMessage("El campo nombre de módul no puede tener más de 25 caracteres!");
        }
    }
}
