using System.Net;
using System.ComponentModel.DataAnnotations;
using WidgetCo.Store.Core.Exceptions;

namespace WidgetCo.Store.Core.Extensions
{
    public static class ValidationExtensions
    {
        public static void ValidateAndThrow<T>(this T model) where T : class
        {
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(model, validationContext, validationResults, validateAllProperties: true))
            {
                var errors = string.Join(", ", validationResults.Select(r => r.ErrorMessage));
                throw new StoreException(
                    "Validation failed",
                    (int)HttpStatusCode.BadRequest,
                    errors);
            }
        }
    }
}
