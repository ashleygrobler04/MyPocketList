namespace MyPocketList.Core.Results;

/// <summary>
/// Represents the outcome of an operation.
/// </summary>
public class Result
{
    /// <summary>
    /// Gets a value indicating whether the operation completed successfully.
    /// </summary>
    public bool Succeeded { get; }

    /// <summary>
    /// Gets the error message if the operation failed; otherwise, an empty string.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="succeeded">Indicates whether the operation succeeded.</param>
    /// <param name="message">
    /// The error message if the operation failed; otherwise, an empty string.
    /// </param>
    protected Result(bool succeeded, string message)
    {
        Succeeded = succeeded;
        Message = message;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A successful <see cref="Result"/>.</returns>
    public static Result Ok() => new(true, string.Empty);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="message">The error message describing the failure.</param>
    /// <returns>A failed <see cref="Result"/>.</returns>
    public static Result Fail(string message) => new(false, message);
}

/// <summary>
/// Represents the outcome of an operation that returns a value.
/// </summary>
/// <typeparam name="T">The type of value returned by the operation.</typeparam>
public sealed class Result<T> : Result
{
    /// <summary>
    /// Gets the value returned by the operation if it succeeded; otherwise, <see langword="default"/>.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{T}"/> class.
    /// </summary>
    /// <param name="succeeded">Indicates whether the operation succeeded.</param>
    /// <param name="message">
    /// The error message if the operation failed; otherwise, an empty string.
    /// </param>
    /// <param name="value">
    /// The value returned by the operation if it succeeded; otherwise, <see langword="default"/>.
    /// </param>
    private Result(bool succeeded, string message, T? value)
        : base(succeeded, message)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a successful result containing a value.
    /// </summary>
    /// <param name="value">The value returned by the operation.</param>
    /// <returns>A successful <see cref="Result{T}"/>.</returns>
    public static Result<T> Ok(T value) => new(true, string.Empty, value);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="message">The error message describing the failure.</param>
    /// <returns>A failed <see cref="Result{T}"/>.</returns>
    public new static Result<T> Fail(string message) => new(false, message, default);
}