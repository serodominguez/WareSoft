using Application.Dtos.Request.Store;
using FluentValidation;

namespace Application.Validators
{
    public class StoreValidator : AbstractValidator<StoreRequestDto>
    {
        public StoreValidator()
        {
            RuleFor(x => x.StoreName)
                .NotNull().WithMessage("El campo nombre de tienda no puede ser nulo!")
                .NotEmpty().WithMessage("El campo nombre de tienda no puede estar vacio!")
                .MaximumLength(50).WithMessage("El campo nombre de tienda no puede tener más de 50 caracteres!");

            RuleFor(x => x.Manager)
                .NotNull().WithMessage("El campo encargado no puede ser nulo!")
                .NotEmpty().WithMessage("El campo encargado no puede estar vacio!")
                .MaximumLength(30).WithMessage("El encargado no puede tener más de 30 caracteres!");

            RuleFor(x => x.Address)
                .NotNull().WithMessage("El campo dirección no puede ser nulo!")
                .NotEmpty().WithMessage("El campo dirección no puede estar vacio!")
                .MaximumLength(60).WithMessage("La dirección no puede tener más de 60 caracteres!");

            RuleFor(x => x.City)
                .NotNull().WithMessage("El campo ciudad no puede ser nulo!")
                .NotEmpty().WithMessage("El campo ciudad no puede estar vacio!")
                .MaximumLength(15).WithMessage("La ciudad no puede tener más de 15 caracteres!");

            RuleFor(x => x.Email)
                .MaximumLength(50).WithMessage("EL correo no puede tener más de 50 caracteres!");

            RuleFor(x => x.Type)
                .NotNull().WithMessage("El campo tipo no puede ser nulo!")
                .NotEmpty().WithMessage("El campo tipo no puede estar vacio!")
                .MaximumLength(15).WithMessage("La tipo no puede tener más de 15 caracteres!");
        }
    }
}
