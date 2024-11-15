namespace WidgetCo.Store.Core.Common
{
    // Represents a void type, used when a return value is needed but there is no meaningful value to return.
    // Used by CQRS-related code for having all handlers return a value, even when it's nothing.
    // This makes each handler follow the same pattern. 
    public readonly struct Unit
    {
        private static readonly Unit _value = new();
        public static Unit Value => _value;
    }
}