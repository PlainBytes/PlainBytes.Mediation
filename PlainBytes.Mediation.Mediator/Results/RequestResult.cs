namespace PlainBytes.Mediation.Mediator.Results;

/// <summary>
/// Represents the result of a request, containing either a value or an exception.
/// </summary>
/// <typeparam name="T">Type of the wrapped value.</typeparam>
public sealed class RequestResult<T>
{
    private readonly T _value;
    /// <summary>
    /// Indicates if the wrapped value was successfully created.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Indicates if the creation of the expected value failed.
    /// </summary>
    public bool Error => !Success;

    /// <summary>
    /// Source exception which prevented creating the value, or <c>null</c> if successful.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// Wrapped value.
    /// </summary>
    /// <exception cref="InvalidOperationException">If the <see cref="RequestResult{T}"/> is in a failed state and does not have a valid value.</exception>
    public T Value
    {
        get
        {
            if (Success)
            {
                return _value;
            }
            throw new InvalidOperationException("Result does not have a value.", Exception);
        }
    }

    private RequestResult(T value)
    {
        _value = value;
        Success = true;
        Exception = null;
    }

    private RequestResult(Exception exception)
    {
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        Success = false;
        _value = default!;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static RequestResult<T> Successful(T value) => new(value);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    public static RequestResult<T> Failure(Exception exception) => new(exception);

    /// <summary>
    /// Converts the <see cref="RequestResult{T}"/> into the underlying value type.
    /// </summary>
    public static implicit operator T(RequestResult<T> requestResult)
    {
        ArgumentNullException.ThrowIfNull(requestResult);
        return requestResult.Value;
    }

    /// <summary>
    /// Deconstructs the result into its components.
    /// </summary>
    public void Deconstruct(out bool success, out T value, out Exception? exception)
    {
        success = Success;
        value = _value;
        exception = Exception;
    }

    /// <summary>
    /// Attempts to get the value if successful.
    /// </summary>
    public bool TryGetValue(out T value)
    {
        value = _value;
        return Success;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Success ? $"Result: {Value}" : $"Exception: {Exception}";
}