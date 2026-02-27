using System.Linq;

namespace REslava.Result;

public partial class Result<TValue>
{
    /// <summary>
    /// Converts a typed exception inside a failed result to a different error.
    /// If the result contains an <see cref="ExceptionError"/> wrapping a <typeparamref name="TException"/>,
    /// it is replaced with the error returned by <paramref name="handler"/>.
    /// All other errors and success reasons are preserved.
    /// If the result is successful, or no matching exception error exists, the result is returned unchanged.
    /// </summary>
    /// <typeparam name="TException">The exception type to handle. Subclasses are matched.</typeparam>
    /// <param name="handler">Maps the caught exception to a replacement <see cref="IError"/>.</param>
    /// <returns>
    /// The original result if successful or no matching exception error is found;
    /// otherwise a new failed result with the exception error replaced.
    /// </returns>
    /// <example>
    /// <code>
    /// var result = await Result.TryAsync(() => httpClient.GetUserAsync(id))
    ///     .Catch&lt;HttpRequestException&gt;(ex => new NotFoundError("User", id));
    /// </code>
    /// </example>
    public Result<TValue> Catch<TException>(Func<TException, IError> handler)
        where TException : Exception
    {
        handler = handler.EnsureNotNull(nameof(handler));

        if (IsSuccess) return this;

        var exceptionError = Errors
            .OfType<ExceptionError>()
            .FirstOrDefault(e => e.Exception is TException);

        if (exceptionError is null) return this;

        var replacementError = handler((TException)exceptionError.Exception);
        var index = Reasons.IndexOf(exceptionError);
        var newReasons = Reasons.SetItem(index, replacementError);

        return new Result<TValue>(_value, newReasons);
    }

    /// <summary>
    /// Asynchronously converts a typed exception inside a failed result to a different error.
    /// </summary>
    /// <typeparam name="TException">The exception type to handle. Subclasses are matched.</typeparam>
    /// <param name="handler">Async function that maps the caught exception to a replacement <see cref="IError"/>.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>
    /// The original result if successful or no matching exception error is found;
    /// otherwise a new failed result with the exception error replaced.
    /// </returns>
    public async Task<Result<TValue>> CatchAsync<TException>(
        Func<TException, Task<IError>> handler,
        CancellationToken cancellationToken = default)
        where TException : Exception
    {
        handler = handler.EnsureNotNull(nameof(handler));
        cancellationToken.ThrowIfCancellationRequested();

        if (IsSuccess) return this;

        var exceptionError = Errors
            .OfType<ExceptionError>()
            .FirstOrDefault(e => e.Exception is TException);

        if (exceptionError is null) return this;

        var replacementError = await handler((TException)exceptionError.Exception).ConfigureAwait(false);
        var index = Reasons.IndexOf(exceptionError);
        var newReasons = Reasons.SetItem(index, replacementError);

        return new Result<TValue>(_value, newReasons);
    }
}
