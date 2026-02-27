using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace REslava.Result.Extensions;

/// <summary>
/// Extension methods for railway recovery on <see cref="Task{T}"/> result pipelines.
/// </summary>
public static class ResultRecoverExtensions
{
    /// <summary>
    /// Recovers from a failure in a <see cref="Task{T}"/> result pipeline using a synchronous fallback.
    /// If the awaited result is successful, it is returned unchanged.
    /// </summary>
    /// <typeparam name="T">The result value type.</typeparam>
    /// <param name="resultTask">The result task to await.</param>
    /// <param name="recover">Function that receives the current errors and returns a new result.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The original result if successful; otherwise the result of <paramref name="recover"/>.</returns>
    /// <example>
    /// <code>
    /// Result&lt;User&gt; result = await userRepository.GetAsync(id)
    ///     .Recover(errors => userCache.Get(id));
    /// </code>
    /// </example>
    public static async Task<Result<T>> Recover<T>(
        this Task<Result<T>> resultTask,
        Func<ImmutableList<IError>, Result<T>> recover,
        CancellationToken cancellationToken = default)
    {
        recover = recover.EnsureNotNull(nameof(recover));
        cancellationToken.ThrowIfCancellationRequested();
        var result = await resultTask.ConfigureAwait(false);
        return result.Recover(recover);
    }

    /// <summary>
    /// Recovers from a failure in a <see cref="Task{T}"/> result pipeline using an async fallback.
    /// If the awaited result is successful, it is returned unchanged.
    /// </summary>
    /// <typeparam name="T">The result value type.</typeparam>
    /// <param name="resultTask">The result task to await.</param>
    /// <param name="recover">Async function that receives the current errors and returns a new result.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The original result if successful; otherwise the result of <paramref name="recover"/>.</returns>
    /// <example>
    /// <code>
    /// Result&lt;User&gt; result = await userRepository.GetAsync(id)
    ///     .RecoverAsync(errors => fallbackApi.GetUserAsync(id));
    /// </code>
    /// </example>
    public static async Task<Result<T>> RecoverAsync<T>(
        this Task<Result<T>> resultTask,
        Func<ImmutableList<IError>, Task<Result<T>>> recover,
        CancellationToken cancellationToken = default)
    {
        recover = recover.EnsureNotNull(nameof(recover));
        cancellationToken.ThrowIfCancellationRequested();
        var result = await resultTask.ConfigureAwait(false);
        return await result.RecoverAsync(recover, cancellationToken).ConfigureAwait(false);
    }
}
