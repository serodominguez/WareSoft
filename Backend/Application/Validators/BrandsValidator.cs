﻿using Application.Dtos.Request.Brands;
using FluentValidation;

namespace Application.Validators
{
    public class BrandsValidator : AbstractValidator<BrandsRequestDto>
    {
        public BrandsValidator() 
        {
            RuleFor(x => x.BRAND_NAME)
                .NotNull().WithMessage("El campo nombre no puede ser nulo!")
                .NotEmpty().WithMessage("El campo nombre no puede estar vacio!")
                .MaximumLength(25).WithMessage("El campo nombre no puede tener más de 25 caracteres!");
        }
    }
}
