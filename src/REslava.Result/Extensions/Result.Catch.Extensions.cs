namespace REslava.Result.Extensions;

/// <summary>
/// Extension methods for inline typed exception handling on <see cref="Task{T}"/> result pipelines.
/// </summary>
public static class ResultCatchExtensions
{
    /// <summary>
    /// Handles a specific exception type in a <see cref="Task{T}"/> result pipeline.
    /// Covers two cases:
    /// <list type="bullet">
    ///   <item>The task throws <typeparamref name="TException"/> directly — the exception is caught and converted.</item>
    ///   <item>The task returns a failed result containing an <see cref="ExceptionError"/> wrapping <typeparamref name="TException"/> — the error is replaced.</item>
    /// </list>
    /// </summary>
    /// <typeparam name="T">The result value type.</typeparam>
    /// <typeparam name="TException">The exception type to handle. Subclasses are matched.</typeparam>
    /// <param name="resultTask">The result task to await.</param>
    /// <param name="handler">Maps the caught exception to a replacement <see cref="IError"/>.</param>
    /// <returns>
    /// The original result if successful or no matching exception is found;
    /// otherwise a failed result with the exception converted to the handler's error.
    /// </returns>
    /// <example>
    /// <code>
    /// Result&lt;User&gt; result = await service.GetUserAsync(id)
    ///     .Catch&lt;User, HttpRequestException&gt;(ex => new NotFoundError("User", id));
    /// </code>
    /// </example>
    public static async Task<Result<T>> Catch<T, TException>(
        this Task<Result<T>> resultTask,
        Func<TException, IError> handler)
        where TException : Exception
    {
        handler = handler.EnsureNotNull(nameof(handler));

        try
        {
            var result = await resultTask.ConfigureAwait(false);
            return result.Catch(handler);
        }
        catch (TException ex)
        {
            return Result<T>.Fail(handler(ex));
        }
    }

    /// <summary>
    /// Handles a specific exception type in a <see cref="Task{T}"/> result pipeline using an async handler.
    /// </summary>
    /// <typeparam name="T">The result value type.</typeparam>
    /// <typeparam name="TException">The exception type to handle. Subclasses are matched.</typeparam>
    /// <param name="resultTask">The result task to await.</param>
    /// <param name="handler">Async function that maps the caught exception to a replacement <see cref="IError"/>.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>
    /// The original result if successful or no matching exception is found;
    /// otherwise a failed result with the exception converted to the handler's error.
    /// </returns>
    public static async Task<Result<T>> CatchAsync<T, TException>(
        this Task<Result<T>> resultTask,
        Func<TException, Task<IError>> handler,
        CancellationToken cancellationToken = default)
        where TException : Exception
    {
        handler = handler.EnsureNotNull(nameof(handler));
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var result = await resultTask.ConfigureAwait(false);
            return await result.CatchAsync(handler, cancellationToken).ConfigureAwait(false);
        }
        catch (TException ex)
        {
            return Result<T>.Fail(await handler(ex).ConfigureAwait(false));
        }
    }
}
