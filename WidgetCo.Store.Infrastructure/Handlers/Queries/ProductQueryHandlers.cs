using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using WidgetCo.Store.Core.DTOs.Products;
using WidgetCo.Store.Core.Exceptions;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Queries;
using WidgetCo.Store.Infrastructure.Data;

namespace WidgetCo.Store.Infrastructure.Handlers.Queries
{
    public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductDto?>
    {
        private readonly WidgetCoDbContext _context;
        private readonly ILogger<GetProductByIdQueryHandler> _logger;

        public GetProductByIdQueryHandler(
            WidgetCoDbContext context,
            ILogger<GetProductByIdQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ProductDto?> HandleAsync(GetProductByIdQuery query)
        {
            try
            {
                var product = await _context.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == query.Id);

                if (product == null)
                {
                    return null;
                }

                return new ProductDto(
                    product.Id,
                    product.Name,
                    product.Price,
                    product.Description,
                    product.ImageUrl
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product {ProductId}", query.Id);
                throw new StoreException(
                    "Error retrieving product",
                    (int)HttpStatusCode.InternalServerError,
                    ex);
            }
        }
    }

    public class GetAllProductsQueryHandler : IQueryHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
    {
        private readonly WidgetCoDbContext _context;
        private readonly ILogger<GetAllProductsQueryHandler> _logger;

        public GetAllProductsQueryHandler(
            WidgetCoDbContext context,
            ILogger<GetAllProductsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductDto>> HandleAsync(GetAllProductsQuery query)
        {
            try
            {
                return await _context.Products
                    .AsNoTracking()
                    .Select(p => new ProductDto(
                        p.Id,
                        p.Name,
                        p.Price,
                        p.Description,
                        p.ImageUrl
                    ))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products");
                throw new StoreException(
                    "Error retrieving products",
                    (int)HttpStatusCode.InternalServerError,
                    ex);
            }
        }
    }
}
