using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace REslava.Result.Extensions;

/// <summary>
/// Extension methods for enriching OpenTelemetry <see cref="Activity"/> spans with Result outcome metadata.
/// </summary>
public static class ResultActivityExtensions
{
    /// <summary>
    /// Awaits the result task and enriches an existing <see cref="Activity"/> span with outcome metadata.
    /// Sets <c>result.outcome</c> and, on failure, <c>result.error.type</c>, <c>result.error.message</c>,
    /// and optionally <c>result.error.count</c> (when there are multiple errors).
    /// Also updates <see cref="Activity.Status"/> accordingly.
    /// The result is returned unchanged — this is a pure side-effect (Tap-style).
    /// Safe to call when <paramref name="activity"/> is <see langword="null"/> — no-op in that case.
    /// </summary>
    /// <typeparam name="T">The result value type.</typeparam>
    /// <param name="resultTask">The result task to await.</param>
    /// <param name="activity">The active span to enrich. Typically <see cref="Activity.Current"/>.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The original result, unchanged.</returns>
    /// <example>
    /// <code>
    /// Result&lt;User&gt; result = await service.GetUser(id)
    ///     .WithActivity(Activity.Current);
    /// </code>
    /// </example>
    public static async Task<Result<T>> WithActivity<T>(
        this Task<Result<T>> resultTask,
        Activity? activity,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await resultTask.ConfigureAwait(false);
        return result.WithActivity(activity);
    }
}
