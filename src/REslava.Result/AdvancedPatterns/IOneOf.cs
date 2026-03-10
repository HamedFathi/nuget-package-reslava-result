namespace REslava.Result.AdvancedPatterns
{
    /// <summary>
    /// Marker interface for a discriminated union of two possible types.
    /// Implemented by both <see cref="OneOf{T1,T2}"/> (value union) and
    /// <see cref="ErrorsOf{T1,T2}"/> (error union).
    /// </summary>
    public interface IOneOf<T1, T2>
    {
        bool IsT1 { get; }
        bool IsT2 { get; }
        T1   AsT1 { get; }
        T2   AsT2 { get; }
    }

    /// <summary>
    /// Marker interface for a discriminated union of three possible types.
    /// </summary>
    public interface IOneOf<T1, T2, T3>
    {
        bool IsT1 { get; }
        bool IsT2 { get; }
        bool IsT3 { get; }
        T1   AsT1 { get; }
        T2   AsT2 { get; }
        T3   AsT3 { get; }
    }

    /// <summary>
    /// Marker interface for a discriminated union of four possible types.
    /// </summary>
    public interface IOneOf<T1, T2, T3, T4>
    {
        bool IsT1 { get; }
        bool IsT2 { get; }
        bool IsT3 { get; }
        bool IsT4 { get; }
        T1   AsT1 { get; }
        T2   AsT2 { get; }
        T3   AsT3 { get; }
        T4   AsT4 { get; }
    }

    /// <summary>
    /// Marker interface for a discriminated union of five possible types.
    /// </summary>
    public interface IOneOf<T1, T2, T3, T4, T5>
    {
        bool IsT1 { get; }
        bool IsT2 { get; }
        bool IsT3 { get; }
        bool IsT4 { get; }
        bool IsT5 { get; }
        T1   AsT1 { get; }
        T2   AsT2 { get; }
        T3   AsT3 { get; }
        T4   AsT4 { get; }
        T5   AsT5 { get; }
    }

    /// <summary>
    /// Marker interface for a discriminated union of six possible types.
    /// </summary>
    public interface IOneOf<T1, T2, T3, T4, T5, T6>
    {
        bool IsT1 { get; }
        bool IsT2 { get; }
        bool IsT3 { get; }
        bool IsT4 { get; }
        bool IsT5 { get; }
        bool IsT6 { get; }
        T1   AsT1 { get; }
        T2   AsT2 { get; }
        T3   AsT3 { get; }
        T4   AsT4 { get; }
        T5   AsT5 { get; }
        T6   AsT6 { get; }
    }

    /// <summary>
    /// Marker interface for a discriminated union of seven possible types.
    /// </summary>
    public interface IOneOf<T1, T2, T3, T4, T5, T6, T7>
    {
        bool IsT1 { get; }
        bool IsT2 { get; }
        bool IsT3 { get; }
        bool IsT4 { get; }
        bool IsT5 { get; }
        bool IsT6 { get; }
        bool IsT7 { get; }
        T1   AsT1 { get; }
        T2   AsT2 { get; }
        T3   AsT3 { get; }
        T4   AsT4 { get; }
        T5   AsT5 { get; }
        T6   AsT6 { get; }
        T7   AsT7 { get; }
    }

    /// <summary>
    /// Marker interface for a discriminated union of eight possible types.
    /// </summary>
    public interface IOneOf<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        bool IsT1 { get; }
        bool IsT2 { get; }
        bool IsT3 { get; }
        bool IsT4 { get; }
        bool IsT5 { get; }
        bool IsT6 { get; }
        bool IsT7 { get; }
        bool IsT8 { get; }
        T1   AsT1 { get; }
        T2   AsT2 { get; }
        T3   AsT3 { get; }
        T4   AsT4 { get; }
        T5   AsT5 { get; }
        T6   AsT6 { get; }
        T7   AsT7 { get; }
        T8   AsT8 { get; }
    }
}
