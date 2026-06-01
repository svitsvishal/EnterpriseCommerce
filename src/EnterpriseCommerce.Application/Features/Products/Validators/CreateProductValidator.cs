using EnterpriseCommerce.Application.Features.Products.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseCommerce.Application.Features.Products.Validators
{
    public class CreateProductValidator
     : AbstractValidator<CreateProductCommand>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);
          

            RuleFor(x => x.Price)
                .GreaterThan(0);

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0);
        }
    }
}
