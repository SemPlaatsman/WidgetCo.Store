using Microsoft.Extensions.Logging;
using System.Net;
using WidgetCo.Store.Core.Commands;
using WidgetCo.Store.Core.Common;
using WidgetCo.Store.Core.Exceptions;
using WidgetCo.Store.Core.Extensions;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Models;
using WidgetCo.Store.Infrastructure.Storage.Interfaces;

namespace WidgetCo.Store.Infrastructure.Handlers.Commands
{
    public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, string>
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<CreateProductCommandHandler> _logger;

        public CreateProductCommandHandler(
            IProductRepository productRepository,
            ILogger<CreateProductCommandHandler> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<string> HandleAsync(CreateProductCommand command)
        {
            try
            {
                var product = new Product
                {
                    Name = command.Name,
                    Price = command.Price,
                    Description = command.Description,
                    ImageUrl = command.ImageUrl
                };

                product.ValidateAndThrow();
                return await _productRepository.CreateAsync(product);
            }
            catch (Exception ex) when (ex is not StoreException)
            {
                _logger.LogError(ex, "Error creating product");
                throw new StoreException(
                    "Error creating product",
                    (int)HttpStatusCode.InternalServerError,
                    ex);
            }
        }
    }

    public class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand, Unit>
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<UpdateProductCommandHandler> _logger;

        public UpdateProductCommandHandler(
            IProductRepository productRepository,
            ILogger<UpdateProductCommandHandler> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<Unit> HandleAsync(UpdateProductCommand command)
        {
            try
            {
                if (!await _productRepository.ExistsAsync(command.Id))
                {
                    throw new StoreException(
                        $"Product with ID {command.Id} not found",
                        (int)HttpStatusCode.NotFound);
                }

                var updatedProduct = new Product
                {
                    Id = command.Id,
                    Name = command.Name,
                    Price = command.Price,
                    Description = command.Description,
                    ImageUrl = command.ImageUrl
                };

                updatedProduct.ValidateAndThrow();
                await _productRepository.UpdateAsync(updatedProduct);

                return Unit.Value;
            }
            catch (Exception ex) when (ex is not StoreException)
            {
                _logger.LogError(ex, "Error updating product {ProductId}", command.Id);
                throw new StoreException(
                    "Error updating product",
                    (int)HttpStatusCode.InternalServerError,
                    ex);
            }
        }
    }
}