using WidgetCo.Store.Core.Commands;
using WidgetCo.Store.Core.Common;
using WidgetCo.Store.Core.DTOs.Products;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Queries;

namespace WidgetCo.Store.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IQueryHandler<GetProductByIdQuery, ProductDto?> _getProductByIdHandler;
        private readonly IQueryHandler<GetAllProductsQuery, IEnumerable<ProductDto>> _getAllProductsHandler;
        private readonly ICommandHandler<CreateProductCommand, string> _createProductHandler;
        private readonly ICommandHandler<UpdateProductCommand, Unit> _updateProductHandler;

        public ProductService(
            IQueryHandler<GetProductByIdQuery, ProductDto?> getProductByIdHandler,
            IQueryHandler<GetAllProductsQuery, IEnumerable<ProductDto>> getAllProductsHandler,
            ICommandHandler<CreateProductCommand, string> createProductHandler,
            ICommandHandler<UpdateProductCommand, Unit> updateProductHandler)
        {
            _getProductByIdHandler = getProductByIdHandler;
            _getAllProductsHandler = getAllProductsHandler;
            _createProductHandler = createProductHandler;
            _updateProductHandler = updateProductHandler;
        }

        public async Task<ProductDto?> GetProductByIdAsync(string productId)
            => await _getProductByIdHandler.HandleAsync(new GetProductByIdQuery(productId));

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
            => await _getAllProductsHandler.HandleAsync(new GetAllProductsQuery());

        public async Task<string> CreateProductAsync(CreateProductCommand command)
            => await _createProductHandler.HandleAsync(command);

        public async Task UpdateProductAsync(UpdateProductCommand command)
            => await _updateProductHandler.HandleAsync(command);
    }
}
