using Application.Dtos.Request.Role;
using FluentValidation;

namespace Application.Validators
{
    public class RoleValidator : AbstractValidator<RoleRequestDto>
    {
        public RoleValidator()
        {
            RuleFor(x => x.RoleName)
                .NotNull().WithMessage("El campo nombre de rol no puede ser nulo!")
                .NotEmpty().WithMessage("El campo nombre de rol no puede estar vacio!")
                .MaximumLength(20).WithMessage("El campo nombre de rol no puede tener más de 20 caracteres!");
        }
    }
}
