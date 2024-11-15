using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using WidgetCo.Store.Core.Commands;
using WidgetCo.Store.Core.Common;
using WidgetCo.Store.Core.Exceptions;
using WidgetCo.Store.Core.Extensions;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Models;
using WidgetCo.Store.Infrastructure.Data;

namespace WidgetCo.Store.Infrastructure.Handlers.Commands
{
    public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, string>
    {
        private readonly WidgetCoDbContext _context;
        private readonly ILogger<CreateProductCommandHandler> _logger;

        public CreateProductCommandHandler(
            WidgetCoDbContext context,
            ILogger<CreateProductCommandHandler> logger)
        {
            _context = context;
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
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                return product.Id;
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
        private readonly WidgetCoDbContext _context;
        private readonly ILogger<UpdateProductCommandHandler> _logger;

        public UpdateProductCommandHandler(
            WidgetCoDbContext context,
            ILogger<UpdateProductCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> HandleAsync(UpdateProductCommand command)
        {
            try
            {
                var product = await _context.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == command.Id);

                if (product == null)
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
                _context.Products.Update(updatedProduct);
                await _context.SaveChangesAsync();

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
