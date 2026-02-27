using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace REslava.Result;

public partial class Result
{
    /// <summary>
    /// Recovers from a failure by invoking <paramref name="recover"/> with the current errors.
    /// If the result is successful, it is returned unchanged.
    /// </summary>
    /// <param name="recover">Function that receives the current errors and returns a new result.</param>
    /// <returns>The original result if successful; otherwise the result of <paramref name="recover"/>.</returns>
    /// <example>
    /// <code>
    /// Result result = await DeleteUser(id)
    ///     .Recover(errors => ArchiveUser(id));
    /// </code>
    /// </example>
    public Result Recover(Func<ImmutableList<IError>, Result> recover)
    {
        recover = recover.EnsureNotNull(nameof(recover));
        if (IsSuccess) return this;
        return recover(Errors);
    }

    /// <summary>
    /// Asynchronously recovers from a failure by invoking <paramref name="recover"/> with the current errors.
    /// If the result is successful, it is returned unchanged.
    /// </summary>
    /// <param name="recover">Async function that receives the current errors and returns a new result.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The original result if successful; otherwise the result of <paramref name="recover"/>.</returns>
    public async Task<Result> RecoverAsync(
        Func<ImmutableList<IError>, Task<Result>> recover,
        CancellationToken cancellationToken = default)
    {
        recover = recover.EnsureNotNull(nameof(recover));
        cancellationToken.ThrowIfCancellationRequested();
        if (IsSuccess) return this;
        return await recover(Errors).ConfigureAwait(false);
    }
}

public partial class Result<TValue>
{
    /// <summary>
    /// Recovers from a failure by invoking <paramref name="recover"/> with the current errors.
    /// If the result is successful, it is returned unchanged.
    /// </summary>
    /// <param name="recover">Function that receives the current errors and returns a new result.</param>
    /// <returns>The original result if successful; otherwise the result of <paramref name="recover"/>.</returns>
    /// <example>
    /// <code>
    /// Result&lt;User&gt; result = await userRepository.GetAsync(id)
    ///     .Recover(errors => userCache.Get(id));
    /// </code>
    /// </example>
    public Result<TValue> Recover(Func<ImmutableList<IError>, Result<TValue>> recover)
    {
        recover = recover.EnsureNotNull(nameof(recover));
        if (IsSuccess) return this;
        return recover(Errors);
    }

    /// <summary>
    /// Asynchronously recovers from a failure by invoking <paramref name="recover"/> with the current errors.
    /// If the result is successful, it is returned unchanged.
    /// </summary>
    /// <param name="recover">Async function that receives the current errors and returns a new result.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The original result if successful; otherwise the result of <paramref name="recover"/>.</returns>
    public async Task<Result<TValue>> RecoverAsync(
        Func<ImmutableList<IError>, Task<Result<TValue>>> recover,
        CancellationToken cancellationToken = default)
    {
        recover = recover.EnsureNotNull(nameof(recover));
        cancellationToken.ThrowIfCancellationRequested();
        if (IsSuccess) return this;
        return await recover(Errors).ConfigureAwait(false);
    }
}
