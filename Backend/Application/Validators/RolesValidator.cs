using Application.Dtos.Request.Roles;
using FluentValidation;

namespace Application.Validators
{
    public class RolesValidator : AbstractValidator<RolesRequestDto>
    {
        public RolesValidator()
        {
            RuleFor(x => x.ROLE_NAME)
                .NotNull().WithMessage("EL campo nombre no puede ser nulo!")
                .NotEmpty().WithMessage("El campo nombre no puede estar vacio!");
        }
    }
}
