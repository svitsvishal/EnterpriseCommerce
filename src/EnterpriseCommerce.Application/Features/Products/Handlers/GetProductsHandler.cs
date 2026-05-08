using EnterpriseCommerce.Application.Common.Models;
using EnterpriseCommerce.Application.DTOs;
using EnterpriseCommerce.Application.Features.Products.Queries;
using EnterpriseCommerce.Application.Interfaces;
using EnterpriseCommerce.Domain.Interfaces;
using MediatR;

namespace EnterpriseCommerce.Application.Features.Products.Handlers;



public class GetProductsHandler
: IRequestHandler<GetProductsQuery,
    PagedResult<ProductDto>>
{
    private readonly IProductRepository _repository;
    private readonly ICacheService _cacheService;

    public GetProductsHandler(IProductRepository repository, ICacheService cacheService)
    {
        _repository = repository;
        _cacheService = cacheService;
    }
    public async Task<PagedResult<ProductDto>> Handle(
    GetProductsQuery request,
    CancellationToken cancellationToken)
    {
        var cacheKey =
            $"products-{request.PageNumber}-{request.PageSize}-{request.Search}";

        var cachedProducts =
            await _cacheService
                .GetAsync<PagedResult<ProductDto>>(cacheKey);

        if (cachedProducts != null)
        {
            return cachedProducts;
        }

        var (products, totalCount) =
            await _repository.GetPagedAsync(
                request.Search,
                request.PageNumber,
                request.PageSize);

        var result = new PagedResult<ProductDto>
        {
            Items = products.Select(x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                StockQuantity = x.StockQuantity
            }),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        await _cacheService.SetAsync(
            cacheKey,
            result,
            TimeSpan.FromMinutes(5));

        return result;
    }

}
   
