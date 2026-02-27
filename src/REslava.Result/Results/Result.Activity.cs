using System.Diagnostics;

namespace REslava.Result;

public partial class Result<TValue>
{
    /// <summary>
    /// Enriches an existing <see cref="Activity"/> span with Result outcome metadata.
    /// Sets <c>result.outcome</c> and, on failure, <c>result.error.type</c>, <c>result.error.message</c>,
    /// and optionally <c>result.error.count</c> (when there are multiple errors).
    /// Also updates <see cref="Activity.Status"/> to <see cref="ActivityStatusCode.Ok"/> or
    /// <see cref="ActivityStatusCode.Error"/> accordingly.
    /// The result is returned unchanged — this is a pure side-effect (Tap-style).
    /// Safe to call when <paramref name="activity"/> is <see langword="null"/> — no-op in that case.
    /// </summary>
    /// <param name="activity">The active span to enrich. Typically <see cref="Activity.Current"/>.</param>
    /// <returns>The original result, unchanged.</returns>
    /// <example>
    /// <code>
    /// Result&lt;User&gt; result = await service.GetUser(id)
    ///     .WithActivity(Activity.Current);
    /// </code>
    /// </example>
    public Result<TValue> WithActivity(Activity? activity)
    {
        if (activity is null) return this;

        if (IsSuccess)
        {
            activity.SetTag("result.outcome", "success");
            activity.SetStatus(ActivityStatusCode.Ok);
            return this;
        }

        activity.SetTag("result.outcome", "failure");
        activity.SetStatus(ActivityStatusCode.Error, Errors[0].Message);
        activity.SetTag("result.error.type", Errors[0].GetType().Name);
        activity.SetTag("result.error.message", Errors[0].Message);
        if (Errors.Count > 1)
            activity.SetTag("result.error.count", Errors.Count.ToString());

        return this;
    }
}
