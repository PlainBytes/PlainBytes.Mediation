namespace PlainBytes.Mediation.Mediator.Handlers
{
    /// <summary>
    /// Represents a void return type used internally for request pipeline behaviors that don't return a value.
    /// This allows void requests to use the same pipeline behavior infrastructure as requests with responses.
    /// </summary>
    internal record struct None;
}