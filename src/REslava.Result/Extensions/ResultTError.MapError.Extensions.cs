using System;

namespace REslava.Result.Extensions;

/// <summary>
/// <c>MapError</c> overload for <see cref="Result{TValue,TError}"/> typed pipelines.
/// Translates the error surface without touching the success value.
/// </summary>
public static class ResultTErrorMapErrorExtensions
{
    /// <summary>
    /// Applies <paramref name="mapper"/> to the error when the result is a failure,
    /// producing a result with a different error type. The success value is forwarded unchanged.
    /// Use this to translate or consolidate error unions at layer boundaries.
    /// </summary>
    /// <typeparam name="TValue">The success value type.</typeparam>
    /// <typeparam name="TErrorIn">The source error type.</typeparam>
    /// <typeparam name="TErrorOut">The target error type.</typeparam>
    /// <param name="result">The result whose error surface to translate.</param>
    /// <param name="mapper">The function that converts the source error to the target error.</param>
    /// <returns>
    /// A result with the mapped error type, or an <c>Ok</c> result with the original value
    /// when the input is a success.
    /// </returns>
    /// <example>
    /// <code>
    /// // Collapse a rich union into a single domain error at a layer boundary
    /// Result&lt;Order, ErrorsOf&lt;ValidationError, InventoryError&gt;&gt; result = ...;
    ///
    /// Result&lt;Order, DomainError&gt; translated = result.MapError(e => e.Match(
    ///     v => new DomainError(v.Message),
    ///     i => new DomainError(i.Message)));
    /// </code>
    /// </example>
    public static Result<TValue, TErrorOut> MapError<TValue, TErrorIn, TErrorOut>(
        this Result<TValue, TErrorIn> result,
        Func<TErrorIn, TErrorOut> mapper)
        where TErrorIn  : IError
        where TErrorOut : IError
    {
        if (mapper is null) throw new ArgumentNullException(nameof(mapper));

        if (result.IsFailure)
            return Result<TValue, TErrorOut>.Fail(mapper(result.Error));

        return Result<TValue, TErrorOut>.Ok(result.Value);
    }
}
