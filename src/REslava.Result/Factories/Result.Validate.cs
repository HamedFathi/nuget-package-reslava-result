namespace REslava.Result;

public partial class Result
{
    #region Validate — Applicative validation (heterogeneous types + mapper)

    /// <summary>
    /// Validates two independent results and maps their values to a combined result.
    /// Runs ALL validations independently — errors accumulate, not short-circuited.
    /// </summary>
    /// <example>
    /// <code>
    /// Result&lt;OrderDto&gt; dto = Result.Validate(
    ///     ValidateName(request.Name),
    ///     ValidateEmail(request.Email),
    ///     (name, email) => new OrderDto(name, email));
    /// // If both fail, dto.Errors contains BOTH errors simultaneously
    /// </code>
    /// </example>
    public static Result<TResult> Validate<T1, T2, TResult>(
        Result<T1> r1,
        Result<T2> r2,
        Func<T1, T2, TResult> mapper)
    {
        mapper = mapper.EnsureNotNull(nameof(mapper));
        var errors = CollectErrors(r1, r2);
        return errors.Count > 0
            ? Result<TResult>.Fail(errors)
            : Result<TResult>.Ok(mapper(r1.Value!, r2.Value!));
    }

    /// <summary>
    /// Validates three independent results and maps their values to a combined result.
    /// Runs ALL validations independently — errors accumulate, not short-circuited.
    /// </summary>
    /// <example>
    /// <code>
    /// Result&lt;OrderDto&gt; dto = Result.Validate(
    ///     ValidateName(request.Name),
    ///     ValidateEmail(request.Email),
    ///     ValidateAge(request.Age),
    ///     (name, email, age) => new OrderDto(name, email, age));
    /// </code>
    /// </example>
    public static Result<TResult> Validate<T1, T2, T3, TResult>(
        Result<T1> r1,
        Result<T2> r2,
        Result<T3> r3,
        Func<T1, T2, T3, TResult> mapper)
    {
        mapper = mapper.EnsureNotNull(nameof(mapper));
        var errors = CollectErrors(r1, r2, r3);
        return errors.Count > 0
            ? Result<TResult>.Fail(errors)
            : Result<TResult>.Ok(mapper(r1.Value!, r2.Value!, r3.Value!));
    }

    /// <summary>
    /// Validates four independent results and maps their values to a combined result.
    /// Runs ALL validations independently — errors accumulate, not short-circuited.
    /// </summary>
    /// <example>
    /// <code>
    /// Result&lt;OrderDto&gt; dto = Result.Validate(
    ///     ValidateName(request.Name),
    ///     ValidateEmail(request.Email),
    ///     ValidateAge(request.Age),
    ///     ValidateAddress(request.Address),
    ///     (name, email, age, address) => new OrderDto(name, email, age, address));
    /// </code>
    /// </example>
    public static Result<TResult> Validate<T1, T2, T3, T4, TResult>(
        Result<T1> r1,
        Result<T2> r2,
        Result<T3> r3,
        Result<T4> r4,
        Func<T1, T2, T3, T4, TResult> mapper)
    {
        mapper = mapper.EnsureNotNull(nameof(mapper));
        var errors = CollectErrors(r1, r2, r3, r4);
        return errors.Count > 0
            ? Result<TResult>.Fail(errors)
            : Result<TResult>.Ok(mapper(r1.Value!, r2.Value!, r3.Value!, r4.Value!));
    }

    #endregion
}
