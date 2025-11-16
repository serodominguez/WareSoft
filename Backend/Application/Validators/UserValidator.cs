using Application.Dtos.Request.User;
using FluentValidation;

namespace Application.Validators
{
    public class UserValidator : AbstractValidator<UserRequestDto>
    {
        public UserValidator()
        {
            RuleFor(x => x.UserName)
                .NotNull().WithMessage("El campo nombre de usuario no puede ser nulo!")
                .NotEmpty().WithMessage("El campo nombre de usuario no puede estar vacio!")
                .MaximumLength(20).WithMessage("El campo nombre de usuario no puede tener más de 20 caracteres!");

            RuleFor(x => x.Password)
                .NotNull().WithMessage("La contraseña no puede ser nulo!")
                .NotEmpty().WithMessage("La contraseña no puede estar vacio!");

            RuleFor(x => x.Names)
                .NotNull().WithMessage("El campo nombre no puede ser nulo!")
                .NotEmpty().WithMessage("El campo nombre no puede estar vacio!")
                .MaximumLength(30).WithMessage("El campo nombre no puede tener más de 30 caracteres!");

            RuleFor(x => x.LastNames)
                .NotNull().WithMessage("El campo apellidos no puede ser nulo!")
                .NotEmpty().WithMessage("El campo apellidos no puede estar vacio!")
                .MaximumLength(50).WithMessage("El campo apellidos no puede tener más de 50 caracteres!");

            RuleFor(x => x.IdRole)
                .NotNull().WithMessage("El identificador del rol no puede ser nulo!");

            RuleFor(x => x.IdStore)
                .NotNull().WithMessage("El identificador de la sucursal no puede ser nulo!");

        }
    }
}
