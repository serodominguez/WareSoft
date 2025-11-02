using Application.Dtos.Request.Modules;
using FluentValidation;

namespace Application.Validators
{
    public class ModulesValidator : AbstractValidator<ModulesRequestDto>
    {
        public ModulesValidator() 
        {
            RuleFor(x => x.MODULE_NAME)
                .NotNull().WithMessage("El campo nombre de módulo no puede ser nulo!")
                .NotEmpty().WithMessage("El campo nombre de módul no puede estar vacio!")
                .MaximumLength(25).WithMessage("El campo nombre de módul no puede tener más de 25 caracteres!");
        }
    }
}
