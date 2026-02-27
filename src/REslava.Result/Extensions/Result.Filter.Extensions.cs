using System.Threading;
using System.Threading.Tasks;

namespace REslava.Result.Extensions;

/// <summary>
/// Extension methods for filtering <see cref="Result{T}"/> by a predicate on the success value.
/// </summary>
public static class ResultFilterExtensions
{
    #region Filter — sync, value-dependent error factory (primary)

    /// <summary>
    /// Converts a successful result to a failure when <paramref name="predicate"/> returns <see langword="false"/>.
    /// The error is built from the value, enabling contextual error messages.
    /// If the result is already a failure, it is returned unchanged.
    /// </summary>
    /// <typeparam name="T">The result value type.</typeparam>
    /// <param name="result">The result to filter.</param>
    /// <param name="predicate">The condition the value must satisfy.</param>
    /// <param name="errorFactory">Produces the error when the predicate fails; receives the value.</param>
    /// <returns>The original result if successful and predicate passes; otherwise a failed result.</returns>
    /// <example>
    /// <code>
    /// Result&lt;User&gt; activeUser = userResult
    ///     .Filter(u => u.IsActive, u => new Error($"User '{u.Name}' is not active."));
    /// </code>
    /// </example>
    public static Result<T> Filter<T>(
        this Result<T> result,
        Func<T, bool> predicate,
        Func<T, IError> errorFactory)
    {
        predicate = predicate.EnsureNotNull(nameof(predicate));
        errorFactory = errorFactory.EnsureNotNull(nameof(errorFactory));

        if (result.IsFailure) return result;

        try
        {
            return predicate(result.Value!)
                ? result
                : Result<T>.Fail(errorFactory(result.Value!));
        }
        catch (Exception ex)
        {
            return Result<T>.Fail(new ExceptionError(ex));
        }
    }

    #endregion

    #region Filter — sync, static error

    /// <summary>
    /// Converts a successful result to a failure when <paramref name="predicate"/> returns <see langword="false"/>.
    /// If the result is already a failure, it is returned unchanged.
    /// </summary>
    /// <param name="result">The result to filter.</param>
    /// <param name="predicate">The condition the value must satisfy.</param>
    /// <param name="error">The error to use when the predicate fails.</param>
    public static Result<T> Filter<T>(
        this Result<T> result,
        Func<T, bool> predicate,
        IError error)
    {
        error = error.EnsureNotNull(nameof(error));
        return result.Filter(predicate, _ => error);
    }

    /// <summary>
    /// Converts a successful result to a failure when <paramref name="predicate"/> returns <see langword="false"/>.
    /// If the result is already a failure, it is returned unchanged.
    /// </summary>
    /// <param name="result">The result to filter.</param>
    /// <param name="predicate">The condition the value must satisfy.</param>
    /// <param name="errorMessage">The error message to use when the predicate fails.</param>
    public static Result<T> Filter<T>(
        this Result<T> result,
        Func<T, bool> predicate,
        string errorMessage)
    {
        errorMessage = errorMessage.EnsureNotNullOrWhiteSpace(nameof(errorMessage));
        return result.Filter(predicate, _ => new Error(errorMessage));
    }

    #endregion

    #region FilterAsync — async predicate, sync error factory

    /// <summary>
    /// Asynchronously filters a result by an async predicate on the success value.
    /// Converts a successful result to a failure when <paramref name="predicate"/> returns <see langword="false"/>.
    /// If the result is already a failure, it is returned unchanged.
    /// </summary>
    /// <typeparam name="T">The result value type.</typeparam>
    /// <param name="result">The result to filter.</param>
    /// <param name="predicate">The async condition the value must satisfy.</param>
    /// <param name="errorFactory">Produces the error when the predicate fails; receives the value.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The original result if successful and predicate passes; otherwise a failed result.</returns>
    /// <example>
    /// <code>
    /// Result&lt;Order&gt; valid = await orderResult
    ///     .FilterAsync(async o => await validator.IsValidAsync(o),
    ///                  o => new ValidationError("Order", o.Id.ToString(), "failed validation"));
    /// </code>
    /// </example>
    public static async Task<Result<T>> FilterAsync<T>(
        this Result<T> result,
        Func<T, Task<bool>> predicate,
        Func<T, IError> errorFactory,
        CancellationToken cancellationToken = default)
    {
        predicate = predicate.EnsureNotNull(nameof(predicate));
        errorFactory = errorFactory.EnsureNotNull(nameof(errorFactory));
        cancellationToken.ThrowIfCancellationRequested();

        if (result.IsFailure) return result;

        try
        {
            var passes = await predicate(result.Value!).ConfigureAwait(false);
            return passes ? result : Result<T>.Fail(errorFactory(result.Value!));
        }
        catch (Exception ex)
        {
            return Result<T>.Fail(new ExceptionError(ex));
        }
    }

    #endregion

    #region Task extensions — Filter (sync predicate)

    /// <summary>
    /// Awaits the result task and filters the result by a predicate on the success value.
    /// </summary>
    public static async Task<Result<T>> Filter<T>(
        this Task<Result<T>> resultTask,
        Func<T, bool> predicate,
        Func<T, IError> errorFactory,
        CancellationToken cancellationToken = default)
    {
        predicate = predicate.EnsureNotNull(nameof(predicate));
        errorFactory = errorFactory.EnsureNotNull(nameof(errorFactory));
        cancellationToken.ThrowIfCancellationRequested();
        var result = await resultTask.ConfigureAwait(false);
        return result.Filter(predicate, errorFactory);
    }

    /// <summary>
    /// Awaits the result task and filters the result by a predicate on the success value.
    /// </summary>
    public static async Task<Result<T>> Filter<T>(
        this Task<Result<T>> resultTask,
        Func<T, bool> predicate,
        IError error,
        CancellationToken cancellationToken = default)
    {
        error = error.EnsureNotNull(nameof(error));
        cancellationToken.ThrowIfCancellationRequested();
        var result = await resultTask.ConfigureAwait(false);
        return result.Filter(predicate, error);
    }

    #endregion

    #region Task extensions — FilterAsync (async predicate)

    /// <summary>
    /// Awaits the result task and filters the result by an async predicate on the success value.
    /// </summary>
    public static async Task<Result<T>> FilterAsync<T>(
        this Task<Result<T>> resultTask,
        Func<T, Task<bool>> predicate,
        Func<T, IError> errorFactory,
        CancellationToken cancellationToken = default)
    {
        predicate = predicate.EnsureNotNull(nameof(predicate));
        errorFactory = errorFactory.EnsureNotNull(nameof(errorFactory));
        cancellationToken.ThrowIfCancellationRequested();
        var result = await resultTask.ConfigureAwait(false);
        return await result.FilterAsync(predicate, errorFactory, cancellationToken).ConfigureAwait(false);
    }

    #endregion
}
