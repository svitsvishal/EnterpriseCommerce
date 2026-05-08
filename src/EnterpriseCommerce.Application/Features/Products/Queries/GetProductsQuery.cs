using EnterpriseCommerce.Application.Common.Models;
using EnterpriseCommerce.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseCommerce.Application.Features.Products.Queries
{
    public class GetProductsQuery
    : IRequest<PagedResult<ProductDto>>
    {
        public string? Search { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
