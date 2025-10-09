﻿using Application.Dtos.Request.Roles;
using FluentValidation;

namespace Application.Validators
{
    public class RolesValidator : AbstractValidator<RolesRequestDto>
    {
        public RolesValidator()
        {
            RuleFor(x => x.ROLE_NAME)
                .NotNull().WithMessage("El campo nombre no puede ser nulo!")
                .NotEmpty().WithMessage("El campo nombre no puede estar vacio!")
                .MaximumLength(20).WithMessage("El campo nombre no puede tener más de 20 caracteres!");
        }
    }
}
