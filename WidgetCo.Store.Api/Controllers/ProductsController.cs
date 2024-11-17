using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Models;
using WidgetCo.Store.Core.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WidgetCo.Store.Core.DTOs.Products;
using WidgetCo.Store.Core.Commands;
using Azure.Core;
using WidgetCo.Store.Core.Extensions;

namespace WidgetCo.Store.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : BaseApiController
    {
        private readonly IProductService _productService;

        public ProductsController(
            IProductService productService,
            ILogger<ProductsController> logger,
            IOptions<ApiOptions> apiOptions)
            : base(logger, apiOptions)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "retrieving products");
            }
        }

        [HttpGet("{productId}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProduct(string productId)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(productId);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "retrieving product");
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(CreateProductResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProduct(CreateProductRequest request)
        {
            request.ValidateAndThrow();
            try
            {
                var command = new CreateProductCommand(
                    request.Name,
                    request.Price,
                    request.Description,
                    request.ImageUrl
                );

                var productId = await _productService.CreateProductAsync(command);
                return Created($"/api/products/{productId}", new CreateProductResponse(productId));
            }
            catch (Exception ex)
            {
                return HandleException(ex, "creating product");
            }
        }

        [HttpPut("{productId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProduct(string productId, UpdateProductRequest request)
        {
            request.ValidateAndThrow();
            try
            {
                if (productId != request.Id)
                {
                    return BadRequest("ProductId mismatch");
                }

                var command = new UpdateProductCommand(
                    request.Id,
                    request.Name,
                    request.Price,
                    request.Description,
                    request.ImageUrl
                );

                await _productService.UpdateProductAsync(command);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex, "updating product");
            }
        }
    }
}
