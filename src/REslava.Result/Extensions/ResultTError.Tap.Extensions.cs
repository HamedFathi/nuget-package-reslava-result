using System;
using REslava.Result.AdvancedPatterns;

namespace REslava.Result.Extensions;

/// <summary>
/// <c>Tap</c> and <c>TapOnFailure</c> overloads for <see cref="Result{TValue,TError}"/> typed pipelines.
/// Execute side effects without altering the result — the original result is always returned.
/// </summary>
public static class ResultTErrorTapExtensions
{
    /// <summary>
    /// Executes a side effect on the success value and returns the original result unchanged.
    /// The action is not called if the result is a failure.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TError">The error type. Must implement <see cref="IError"/>.</typeparam>
    /// <param name="result">The result to tap into.</param>
    /// <param name="action">The side effect to run on success.</param>
    /// <returns>The original result, unchanged.</returns>
    /// <example>
    /// <code>
    /// Validate(req)
    ///     .Tap(order => logger.LogInformation("Order {Id} validated", order.Id))
    ///     .Bind(ReserveInventory);
    /// </code>
    /// </example>
    public static Result<TValue, TError> Tap<TValue, TError>(
        this Result<TValue, TError> result,
        Action<TValue> action)
        where TError : IError
    {
        if (action is null) throw new ArgumentNullException(nameof(action));

        if (result.IsSuccess)
            action(result.Value);

        return result;
    }

    /// <summary>
    /// Executes a side effect on the error value and returns the original result unchanged.
    /// The action is not called if the result is a success.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TError">The error type. Must implement <see cref="IError"/>.</typeparam>
    /// <param name="result">The result to tap into.</param>
    /// <param name="action">The side effect to run on failure.</param>
    /// <returns>The original result, unchanged.</returns>
    /// <example>
    /// <code>
    /// Validate(req)
    ///     .TapOnFailure(err => logger.LogWarning("Validation failed: {Msg}", err.Message))
    ///     .Bind(ReserveInventory);
    /// </code>
    /// </example>
    public static Result<TValue, TError> TapOnFailure<TValue, TError>(
        this Result<TValue, TError> result,
        Action<TError> action)
        where TError : IError
    {
        if (action is null) throw new ArgumentNullException(nameof(action));

        if (result.IsFailure)
            action(result.Error);

        return result;
    }
}
