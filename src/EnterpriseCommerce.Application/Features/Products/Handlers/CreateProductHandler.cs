using EnterpriseCommerce.Application.Features.Products.Commands;
using EnterpriseCommerce.Application.Interfaces;
using EnterpriseCommerce.Domain.Entities;
using EnterpriseCommerce.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseCommerce.Application.Features.Products.Handlers
{
    public class CreateProductHandler
    : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IProductRepository _repository;
     
        private readonly ICacheService _cacheService;
        public CreateProductHandler(IProductRepository repository, ICacheService cacheService)
        {
            _repository = repository;
            _cacheService = cacheService;
        }

        public async Task<Guid> Handle(
            CreateProductCommand request,
            CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Price = request.Price,
                StockQuantity = request.StockQuantity
            };

            await _repository.CreateAsync(product);
            await _cacheService.RemoveAsync(
    "products-1-10-");

            return product.Id;
        }
    }
}
