using System.Collections.Immutable;

namespace REslava.Result;

public partial class Result
{
    /// <summary>
    /// Deconstructs the result into success flag and errors.
    /// </summary>
    /// <example>
    /// <code>
    /// var (isSuccess, errors) = DoSomething();
    /// if (!isSuccess) HandleErrors(errors);
    /// </code>
    /// </example>
    public void Deconstruct(out bool isSuccess, out ImmutableList<IError> errors)
    {
        isSuccess = IsSuccess;
        errors = Errors;
    }
}

public partial class Result<TValue>
{
    /// <summary>
    /// Deconstructs the result into value and errors.
    /// Value is default when the result is failed.
    /// </summary>
    /// <example>
    /// <code>
    /// var (value, errors) = GetUser(id);
    /// if (errors.Count == 0) Console.WriteLine(value!.Name);
    /// </code>
    /// </example>
    public void Deconstruct(out TValue? value, out ImmutableList<IError> errors)
    {
        value = IsSuccess ? _value : default;
        errors = Errors;
    }

    /// <summary>
    /// Deconstructs the result into success flag, value, and errors.
    /// Value is default when the result is failed.
    /// </summary>
    /// <example>
    /// <code>
    /// var (isSuccess, value, errors) = GetUser(id);
    /// if (isSuccess) Console.WriteLine(value!.Name);
    /// </code>
    /// </example>
    public void Deconstruct(out bool isSuccess, out TValue? value, out ImmutableList<IError> errors)
    {
        isSuccess = IsSuccess;
        value = IsSuccess ? _value : default;
        errors = Errors;
    }
}
