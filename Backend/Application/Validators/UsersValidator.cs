using Application.Dtos.Request.Users;
using FluentValidation;

namespace Application.Validators
{
    public class UsersValidator : AbstractValidator<UsersRequestDto>
    {
        public UsersValidator() 
        {
            RuleFor(x => x.USER_NAME)
                .NotNull().WithMessage("El campo nombre de usuario no puede ser nulo!")
                .NotEmpty().WithMessage("El campo nombre de usuario no puede estar vacio!")
                .MaximumLength(20).WithMessage("El campo nombre de usuario no puede tener más de 20 caracteres!");

            RuleFor(x => x.PASSWORD)
                .NotNull().WithMessage("La contraseña no puede ser nulo!")
                .NotEmpty().WithMessage("La contraseña no puede estar vacio!");

            RuleFor(x => x.NAMES)
                .NotNull().WithMessage("El campo nombre no puede ser nulo!")
                .NotEmpty().WithMessage("El campo nombre no puede estar vacio!")
                .MaximumLength(30).WithMessage("El campo nombre no puede tener más de 30 caracteres!");
           
            RuleFor(x => x.LAST_NAMES)
                .NotNull().WithMessage("El campo apellidos no puede ser nulo!")
                .NotEmpty().WithMessage("El campo apellidos no puede estar vacio!")
                .MaximumLength(50).WithMessage("El campo apellidos no puede tener más de 50 caracteres!");

            RuleFor(x => x.PK_ROLE)
                .NotNull().WithMessage("El identificador del rol no puede ser nulo!");
            
            RuleFor(x => x.PK_STORE)
                .NotNull().WithMessage("El identificador de la sucursal no puede ser nulo!");

        }
    }
}
