using Asp.Versioning;
using EnterpriseCommerce.Application.Common.Models;
using EnterpriseCommerce.Application.Features.Products.Commands;
using EnterpriseCommerce.Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace EnterpriseCommerce.API.Controllers
{

    [ApiController]
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{
        //    var result =
        //        await _mediator.Send(new GetProductsQuery());

        //    return Ok(result);
        //}

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateProductCommand command)
        {
            var id = await _mediator.Send(command);

            return Ok(id);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get(
    [FromQuery] GetProductsQuery query)
        {
            var result = await _mediator.Send(query);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Products retrieved successfully",
                Data = result
            });
        }
    }
}
