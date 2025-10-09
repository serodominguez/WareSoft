using Application.Dtos.Request.Stores;
using FluentValidation;

namespace Application.Validators
{
    public class StoresValidator : AbstractValidator<StoresRequestDto>
    {
        public StoresValidator() 
        {
            RuleFor(x => x.STORE_NAME)
                .NotNull().WithMessage("El campo nombre no puede ser nulo!")
                .NotEmpty().WithMessage("El campo nombre no puede estar vacio!")
                .MaximumLength(50).WithMessage("El campo nombre no puede tener más de 50 caracteres!");

            RuleFor(x => x.MANAGER)
                .NotNull().WithMessage("El campo encargado no puede ser nulo!")
                .NotEmpty().WithMessage("El campo encargado no puede estar vacio!")
                .MaximumLength(30).WithMessage("El encargado no puede tener más de 30 caracteres!");
            
            RuleFor(x => x.ADDRESS)
                .NotNull().WithMessage("El campo dirección no puede ser nulo!")
                .NotEmpty().WithMessage("El campo dirección no puede estar vacio!")
                .MaximumLength(60).WithMessage("La dirección no puede tener más de 60 caracteres!");

            RuleFor(x => x.CITY)
                .NotNull().WithMessage("El campo ciudad no puede ser nulo!")
                .NotEmpty().WithMessage("El campo ciudad no puede estar vacio!")
                .MaximumLength(15).WithMessage("La ciudad no puede tener más de 15 caracteres!");

            RuleFor(x => x.EMAIL)
                .MaximumLength(50).WithMessage("EL correo no puede tener más de 50 caracteres!");
            
            RuleFor(x => x.TYPE)
                .NotNull().WithMessage("El campo tipo no puede ser nulo!")
                .NotEmpty().WithMessage("El campo tipo no puede estar vacio!")
                .MaximumLength(15).WithMessage("La tipo no puede tener más de 15 caracteres!");
        }
    }
}
