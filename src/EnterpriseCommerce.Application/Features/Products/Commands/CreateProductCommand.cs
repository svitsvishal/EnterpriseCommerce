using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseCommerce.Application.Features.Products.Commands
{
    public class CreateProductCommand : IRequest<Guid>
    {
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }
    }
}
