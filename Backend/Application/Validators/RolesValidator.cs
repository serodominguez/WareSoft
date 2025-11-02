using Application.Dtos.Request.Roles;
using FluentValidation;

namespace Application.Validators
{
    public class RolesValidator : AbstractValidator<RolesRequestDto>
    {
        public RolesValidator()
        {
            RuleFor(x => x.ROLE_NAME)
                .NotNull().WithMessage("El campo nombre de rol no puede ser nulo!")
                .NotEmpty().WithMessage("El campo nombre de rol no puede estar vacio!")
                .MaximumLength(20).WithMessage("El campo nombre de rol no puede tener más de 20 caracteres!");
        }
    }
}
