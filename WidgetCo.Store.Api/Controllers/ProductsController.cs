using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Models;
using WidgetCo.Store.Core.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
        public async Task<IActionResult> CreateProduct(Product product)
        {
            try
            {
                var productId = await _productService.CreateProductAsync(product);
                return Created($"/api/products/{productId}", new { productId });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "creating product");
            }
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProduct(string productId, Product product)
        {
            try
            {
                if (productId != product.Id)
                {
                    return BadRequest("ProductId mismatch");
                }

                await _productService.UpdateProductAsync(product);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex, "updating product");
            }
        }
    }
}
