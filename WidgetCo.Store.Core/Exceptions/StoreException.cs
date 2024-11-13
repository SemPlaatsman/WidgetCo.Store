namespace WidgetCo.Store.Core.Exceptions
{

    public class StoreException : Exception
    {
        public int StatusCode { get; }
        public string? DetailedMessage { get; }
        public Exception? OriginalException { get; }

        public StoreException(string message, int statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public StoreException(string message, int statusCode, string detailedMessage)
            : this(message, statusCode)
        {
            DetailedMessage = detailedMessage;
        }

        public StoreException(string message, int statusCode, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            OriginalException = innerException;
        }

        public StoreException(string message, int statusCode, string detailedMessage, Exception innerException)
            : this(message, statusCode, innerException)
        {
            DetailedMessage = detailedMessage;
        }
    }
}
