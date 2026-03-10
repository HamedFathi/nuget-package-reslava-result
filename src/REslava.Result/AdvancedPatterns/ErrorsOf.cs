using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace REslava.Result.AdvancedPatterns
{
    /// <summary>
    /// A type-safe error union of two possible error types.
    /// Inherits shared dispatch from <see cref="OneOfBase{T1,T2}"/> and implements <see cref="IError"/>
    /// by delegating <see cref="IReason.Message"/> and <see cref="IReason.Tags"/> to the active case.
    /// </summary>
    /// <typeparam name="T1">The first error type. Must implement <see cref="IError"/>.</typeparam>
    /// <typeparam name="T2">The second error type. Must implement <see cref="IError"/>.</typeparam>
    /// <remarks>
    /// Use <see cref="ErrorsOf{T1,T2}"/> as the <c>TError</c> in <c>Result&lt;TValue, TError&gt;</c>
    /// to accumulate the union of error types across a pipeline built with <c>Bind</c>.
    /// </remarks>
    public sealed class ErrorsOf<T1, T2> : OneOfBase<T1, T2>, IOneOf<T1, T2>, IError, IEquatable<ErrorsOf<T1, T2>>
        where T1 : IError
        where T2 : IError
    {
        private ErrorsOf(T1? v1, T2? v2, byte index) : base(v1, v2, index) { }

        /// <summary>Creates an ErrorsOf containing a T1 error.</summary>
        public static ErrorsOf<T1, T2> FromT1(T1 value) => new(value, default, 0);

        /// <summary>Creates an ErrorsOf containing a T2 error.</summary>
        public static ErrorsOf<T1, T2> FromT2(T2 value) => new(default, value, 1);

        /// <summary>Implicit conversion from T1 to ErrorsOf&lt;T1, T2&gt;.</summary>
        public static implicit operator ErrorsOf<T1, T2>(T1 value) => FromT1(value);

        /// <summary>Implicit conversion from T2 to ErrorsOf&lt;T1, T2&gt;.</summary>
        public static implicit operator ErrorsOf<T1, T2>(T2 value) => FromT2(value);

        // IError — delegate to active case
        /// <inheritdoc/>
        public string Message { get => Match(e => e.Message, e => e.Message); init { } }

        /// <inheritdoc/>
        public ImmutableDictionary<string, object> Tags { get => Match(e => e.Tags, e => e.Tags); init { } }

        /// <summary>Determines equality between two ErrorsOf instances.</summary>
        public bool Equals(ErrorsOf<T1, T2>? other)
        {
            if (other is null || _index != other._index) return false;
            return _index switch
            {
                0 => EqualityComparer<T1>.Default.Equals(_value1, other._value1),
                1 => EqualityComparer<T2>.Default.Equals(_value2, other._value2),
                _ => false
            };
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is ErrorsOf<T1, T2> other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>Equality operator.</summary>
        public static bool operator ==(ErrorsOf<T1, T2>? left, ErrorsOf<T1, T2>? right) => Equals(left, right);

        /// <summary>Inequality operator.</summary>
        public static bool operator !=(ErrorsOf<T1, T2>? left, ErrorsOf<T1, T2>? right) => !Equals(left, right);
    }

    /// <summary>
    /// A type-safe error union of three possible error types.
    /// </summary>
    public sealed class ErrorsOf<T1, T2, T3> : OneOfBase<T1, T2, T3>, IOneOf<T1, T2, T3>, IError, IEquatable<ErrorsOf<T1, T2, T3>>
        where T1 : IError
        where T2 : IError
        where T3 : IError
    {
        private ErrorsOf(T1? v1, T2? v2, T3? v3, byte index) : base(v1, v2, v3, index) { }

        /// <summary>Creates an ErrorsOf containing a T1 error.</summary>
        public static ErrorsOf<T1, T2, T3> FromT1(T1 value) => new(value, default, default, 0);

        /// <summary>Creates an ErrorsOf containing a T2 error.</summary>
        public static ErrorsOf<T1, T2, T3> FromT2(T2 value) => new(default, value, default, 1);

        /// <summary>Creates an ErrorsOf containing a T3 error.</summary>
        public static ErrorsOf<T1, T2, T3> FromT3(T3 value) => new(default, default, value, 2);

        /// <summary>Implicit conversion from T1.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3>(T1 value) => FromT1(value);

        /// <summary>Implicit conversion from T2.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3>(T2 value) => FromT2(value);

        /// <summary>Implicit conversion from T3.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3>(T3 value) => FromT3(value);

        /// <inheritdoc/>
        public string Message { get => Match(e => e.Message, e => e.Message, e => e.Message); init { } }

        /// <inheritdoc/>
        public ImmutableDictionary<string, object> Tags { get => Match(e => e.Tags, e => e.Tags, e => e.Tags); init { } }

        /// <summary>Determines equality between two ErrorsOf instances.</summary>
        public bool Equals(ErrorsOf<T1, T2, T3>? other)
        {
            if (other is null || _index != other._index) return false;
            return _index switch
            {
                0 => EqualityComparer<T1>.Default.Equals(_value1, other._value1),
                1 => EqualityComparer<T2>.Default.Equals(_value2, other._value2),
                2 => EqualityComparer<T3>.Default.Equals(_value3, other._value3),
                _ => false
            };
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is ErrorsOf<T1, T2, T3> other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>Equality operator.</summary>
        public static bool operator ==(ErrorsOf<T1, T2, T3>? left, ErrorsOf<T1, T2, T3>? right) => Equals(left, right);

        /// <summary>Inequality operator.</summary>
        public static bool operator !=(ErrorsOf<T1, T2, T3>? left, ErrorsOf<T1, T2, T3>? right) => !Equals(left, right);
    }

    /// <summary>
    /// A type-safe error union of four possible error types.
    /// </summary>
    public sealed class ErrorsOf<T1, T2, T3, T4> : OneOfBase<T1, T2, T3, T4>, IOneOf<T1, T2, T3, T4>, IError, IEquatable<ErrorsOf<T1, T2, T3, T4>>
        where T1 : IError
        where T2 : IError
        where T3 : IError
        where T4 : IError
    {
        private ErrorsOf(T1? v1, T2? v2, T3? v3, T4? v4, byte index) : base(v1, v2, v3, v4, index) { }

        /// <summary>Creates an ErrorsOf containing a T1 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4> FromT1(T1 value) => new(value, default, default, default, 0);

        /// <summary>Creates an ErrorsOf containing a T2 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4> FromT2(T2 value) => new(default, value, default, default, 1);

        /// <summary>Creates an ErrorsOf containing a T3 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4> FromT3(T3 value) => new(default, default, value, default, 2);

        /// <summary>Creates an ErrorsOf containing a T4 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4> FromT4(T4 value) => new(default, default, default, value, 3);

        /// <summary>Implicit conversion from T1.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4>(T1 value) => FromT1(value);

        /// <summary>Implicit conversion from T2.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4>(T2 value) => FromT2(value);

        /// <summary>Implicit conversion from T3.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4>(T3 value) => FromT3(value);

        /// <summary>Implicit conversion from T4.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4>(T4 value) => FromT4(value);

        /// <inheritdoc/>
        public string Message { get => Match(e => e.Message, e => e.Message, e => e.Message, e => e.Message); init { } }

        /// <inheritdoc/>
        public ImmutableDictionary<string, object> Tags { get => Match(e => e.Tags, e => e.Tags, e => e.Tags, e => e.Tags); init { } }

        /// <summary>Determines equality between two ErrorsOf instances.</summary>
        public bool Equals(ErrorsOf<T1, T2, T3, T4>? other)
        {
            if (other is null || _index != other._index) return false;
            return _index switch
            {
                0 => EqualityComparer<T1>.Default.Equals(_value1, other._value1),
                1 => EqualityComparer<T2>.Default.Equals(_value2, other._value2),
                2 => EqualityComparer<T3>.Default.Equals(_value3, other._value3),
                3 => EqualityComparer<T4>.Default.Equals(_value4, other._value4),
                _ => false
            };
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is ErrorsOf<T1, T2, T3, T4> other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>Equality operator.</summary>
        public static bool operator ==(ErrorsOf<T1, T2, T3, T4>? left, ErrorsOf<T1, T2, T3, T4>? right) => Equals(left, right);

        /// <summary>Inequality operator.</summary>
        public static bool operator !=(ErrorsOf<T1, T2, T3, T4>? left, ErrorsOf<T1, T2, T3, T4>? right) => !Equals(left, right);
    }

    /// <summary>
    /// A type-safe error union of five possible error types.
    /// </summary>
    public sealed class ErrorsOf<T1, T2, T3, T4, T5> : OneOfBase<T1, T2, T3, T4, T5>, IOneOf<T1, T2, T3, T4, T5>, IError, IEquatable<ErrorsOf<T1, T2, T3, T4, T5>>
        where T1 : IError
        where T2 : IError
        where T3 : IError
        where T4 : IError
        where T5 : IError
    {
        private ErrorsOf(T1? v1, T2? v2, T3? v3, T4? v4, T5? v5, byte index) : base(v1, v2, v3, v4, v5, index) { }

        /// <summary>Creates an ErrorsOf containing a T1 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4, T5> FromT1(T1 value) => new(value, default, default, default, default, 0);

        /// <summary>Creates an ErrorsOf containing a T2 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4, T5> FromT2(T2 value) => new(default, value, default, default, default, 1);

        /// <summary>Creates an ErrorsOf containing a T3 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4, T5> FromT3(T3 value) => new(default, default, value, default, default, 2);

        /// <summary>Creates an ErrorsOf containing a T4 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4, T5> FromT4(T4 value) => new(default, default, default, value, default, 3);

        /// <summary>Creates an ErrorsOf containing a T5 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4, T5> FromT5(T5 value) => new(default, default, default, default, value, 4);

        /// <summary>Implicit conversion from T1.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4, T5>(T1 value) => FromT1(value);

        /// <summary>Implicit conversion from T2.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4, T5>(T2 value) => FromT2(value);

        /// <summary>Implicit conversion from T3.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4, T5>(T3 value) => FromT3(value);

        /// <summary>Implicit conversion from T4.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4, T5>(T4 value) => FromT4(value);

        /// <summary>Implicit conversion from T5.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4, T5>(T5 value) => FromT5(value);

        /// <inheritdoc/>
        public string Message { get => Match(e => e.Message, e => e.Message, e => e.Message, e => e.Message, e => e.Message); init { } }

        /// <inheritdoc/>
        public ImmutableDictionary<string, object> Tags { get => Match(e => e.Tags, e => e.Tags, e => e.Tags, e => e.Tags, e => e.Tags); init { } }

        /// <summary>Determines equality between two ErrorsOf instances.</summary>
        public bool Equals(ErrorsOf<T1, T2, T3, T4, T5>? other)
        {
            if (other is null || _index != other._index) return false;
            return _index switch
            {
                0 => EqualityComparer<T1>.Default.Equals(_value1, other._value1),
                1 => EqualityComparer<T2>.Default.Equals(_value2, other._value2),
                2 => EqualityComparer<T3>.Default.Equals(_value3, other._value3),
                3 => EqualityComparer<T4>.Default.Equals(_value4, other._value4),
                4 => EqualityComparer<T5>.Default.Equals(_value5, other._value5),
                _ => false
            };
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is ErrorsOf<T1, T2, T3, T4, T5> other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>Equality operator.</summary>
        public static bool operator ==(ErrorsOf<T1, T2, T3, T4, T5>? left, ErrorsOf<T1, T2, T3, T4, T5>? right) => Equals(left, right);

        /// <summary>Inequality operator.</summary>
        public static bool operator !=(ErrorsOf<T1, T2, T3, T4, T5>? left, ErrorsOf<T1, T2, T3, T4, T5>? right) => !Equals(left, right);
    }

    /// <summary>
    /// A type-safe error union of six possible error types.
    /// </summary>
    public sealed class ErrorsOf<T1, T2, T3, T4, T5, T6> : OneOfBase<T1, T2, T3, T4, T5, T6>, IOneOf<T1, T2, T3, T4, T5, T6>, IError, IEquatable<ErrorsOf<T1, T2, T3, T4, T5, T6>>
        where T1 : IError
        where T2 : IError
        where T3 : IError
        where T4 : IError
        where T5 : IError
        where T6 : IError
    {
        private ErrorsOf(T1? v1, T2? v2, T3? v3, T4? v4, T5? v5, T6? v6, byte index) : base(v1, v2, v3, v4, v5, v6, index) { }

        /// <summary>Creates an ErrorsOf containing a T1 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4, T5, T6> FromT1(T1 value) => new(value, default, default, default, default, default, 0);

        /// <summary>Creates an ErrorsOf containing a T2 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4, T5, T6> FromT2(T2 value) => new(default, value, default, default, default, default, 1);

        /// <summary>Creates an ErrorsOf containing a T3 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4, T5, T6> FromT3(T3 value) => new(default, default, value, default, default, default, 2);

        /// <summary>Creates an ErrorsOf containing a T4 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4, T5, T6> FromT4(T4 value) => new(default, default, default, value, default, default, 3);

        /// <summary>Creates an ErrorsOf containing a T5 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4, T5, T6> FromT5(T5 value) => new(default, default, default, default, value, default, 4);

        /// <summary>Creates an ErrorsOf containing a T6 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4, T5, T6> FromT6(T6 value) => new(default, default, default, default, default, value, 5);

        /// <summary>Implicit conversion from T1.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4, T5, T6>(T1 value) => FromT1(value);

        /// <summary>Implicit conversion from T2.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4, T5, T6>(T2 value) => FromT2(value);

        /// <summary>Implicit conversion from T3.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4, T5, T6>(T3 value) => FromT3(value);

        /// <summary>Implicit conversion from T4.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4, T5, T6>(T4 value) => FromT4(value);

        /// <summary>Implicit conversion from T5.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4, T5, T6>(T5 value) => FromT5(value);

        /// <summary>Implicit conversion from T6.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4, T5, T6>(T6 value) => FromT6(value);

        /// <inheritdoc/>
        public string Message { get => Match(e => e.Message, e => e.Message, e => e.Message, e => e.Message, e => e.Message, e => e.Message); init { } }

        /// <inheritdoc/>
        public ImmutableDictionary<string, object> Tags { get => Match(e => e.Tags, e => e.Tags, e => e.Tags, e => e.Tags, e => e.Tags, e => e.Tags); init { } }

        /// <summary>Determines equality between two ErrorsOf instances.</summary>
        public bool Equals(ErrorsOf<T1, T2, T3, T4, T5, T6>? other)
        {
            if (other is null || _index != other._index) return false;
            return _index switch
            {
                0 => EqualityComparer<T1>.Default.Equals(_value1, other._value1),
                1 => EqualityComparer<T2>.Default.Equals(_value2, other._value2),
                2 => EqualityComparer<T3>.Default.Equals(_value3, other._value3),
                3 => EqualityComparer<T4>.Default.Equals(_value4, other._value4),
                4 => EqualityComparer<T5>.Default.Equals(_value5, other._value5),
                5 => EqualityComparer<T6>.Default.Equals(_value6, other._value6),
                _ => false
            };
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is ErrorsOf<T1, T2, T3, T4, T5, T6> other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>Equality operator.</summary>
        public static bool operator ==(ErrorsOf<T1, T2, T3, T4, T5, T6>? left, ErrorsOf<T1, T2, T3, T4, T5, T6>? right) => Equals(left, right);

        /// <summary>Inequality operator.</summary>
        public static bool operator !=(ErrorsOf<T1, T2, T3, T4, T5, T6>? left, ErrorsOf<T1, T2, T3, T4, T5, T6>? right) => !Equals(left, right);
    }

    /// <summary>
    /// A type-safe error union of seven possible error types.
    /// </summary>
    public sealed class ErrorsOf<T1, T2, T3, T4, T5, T6, T7> : OneOfBase<T1, T2, T3, T4, T5, T6, T7>, IOneOf<T1, T2, T3, T4, T5, T6, T7>, IError, IEquatable<ErrorsOf<T1, T2, T3, T4, T5, T6, T7>>
        where T1 : IError
        where T2 : IError
        where T3 : IError
        where T4 : IError
        where T5 : IError
        where T6 : IError
        where T7 : IError
    {
        private ErrorsOf(T1? v1, T2? v2, T3? v3, T4? v4, T5? v5, T6? v6, T7? v7, byte index) : base(v1, v2, v3, v4, v5, v6, v7, index) { }

        /// <summary>Creates an ErrorsOf containing a T1 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4, T5, T6, T7> FromT1(T1 value) => new(value, default, default, default, default, default, default, 0);

        /// <summary>Creates an ErrorsOf containing a T2 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4, T5, T6, T7> FromT2(T2 value) => new(default, value, default, default, default, default, default, 1);

        /// <summary>Creates an ErrorsOf containing a T3 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4, T5, T6, T7> FromT3(T3 value) => new(default, default, value, default, default, default, default, 2);

        /// <summary>Creates an ErrorsOf containing a T4 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4, T5, T6, T7> FromT4(T4 value) => new(default, default, default, value, default, default, default, 3);

        /// <summary>Creates an ErrorsOf containing a T5 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4, T5, T6, T7> FromT5(T5 value) => new(default, default, default, default, value, default, default, 4);

        /// <summary>Creates an ErrorsOf containing a T6 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4, T5, T6, T7> FromT6(T6 value) => new(default, default, default, default, default, value, default, 5);

        /// <summary>Creates an ErrorsOf containing a T7 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4, T5, T6, T7> FromT7(T7 value) => new(default, default, default, default, default, default, value, 6);

        /// <summary>Implicit conversion from T1.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4, T5, T6, T7>(T1 value) => FromT1(value);

        /// <summary>Implicit conversion from T2.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4, T5, T6, T7>(T2 value) => FromT2(value);

        /// <summary>Implicit conversion from T3.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4, T5, T6, T7>(T3 value) => FromT3(value);

        /// <summary>Implicit conversion from T4.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4, T5, T6, T7>(T4 value) => FromT4(value);

        /// <summary>Implicit conversion from T5.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4, T5, T6, T7>(T5 value) => FromT5(value);

        /// <summary>Implicit conversion from T6.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4, T5, T6, T7>(T6 value) => FromT6(value);

        /// <summary>Implicit conversion from T7.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4, T5, T6, T7>(T7 value) => FromT7(value);

        /// <inheritdoc/>
        public string Message { get => Match(e => e.Message, e => e.Message, e => e.Message, e => e.Message, e => e.Message, e => e.Message, e => e.Message); init { } }

        /// <inheritdoc/>
        public ImmutableDictionary<string, object> Tags { get => Match(e => e.Tags, e => e.Tags, e => e.Tags, e => e.Tags, e => e.Tags, e => e.Tags, e => e.Tags); init { } }

        /// <summary>Determines equality between two ErrorsOf instances.</summary>
        public bool Equals(ErrorsOf<T1, T2, T3, T4, T5, T6, T7>? other)
        {
            if (other is null || _index != other._index) return false;
            return _index switch
            {
                0 => EqualityComparer<T1>.Default.Equals(_value1, other._value1),
                1 => EqualityComparer<T2>.Default.Equals(_value2, other._value2),
                2 => EqualityComparer<T3>.Default.Equals(_value3, other._value3),
                3 => EqualityComparer<T4>.Default.Equals(_value4, other._value4),
                4 => EqualityComparer<T5>.Default.Equals(_value5, other._value5),
                5 => EqualityComparer<T6>.Default.Equals(_value6, other._value6),
                6 => EqualityComparer<T7>.Default.Equals(_value7, other._value7),
                _ => false
            };
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is ErrorsOf<T1, T2, T3, T4, T5, T6, T7> other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>Equality operator.</summary>
        public static bool operator ==(ErrorsOf<T1, T2, T3, T4, T5, T6, T7>? left, ErrorsOf<T1, T2, T3, T4, T5, T6, T7>? right) => Equals(left, right);

        /// <summary>Inequality operator.</summary>
        public static bool operator !=(ErrorsOf<T1, T2, T3, T4, T5, T6, T7>? left, ErrorsOf<T1, T2, T3, T4, T5, T6, T7>? right) => !Equals(left, right);
    }

    /// <summary>
    /// A type-safe error union of eight possible error types.
    /// </summary>
    public sealed class ErrorsOf<T1, T2, T3, T4, T5, T6, T7, T8> : OneOfBase<T1, T2, T3, T4, T5, T6, T7, T8>, IOneOf<T1, T2, T3, T4, T5, T6, T7, T8>, IError, IEquatable<ErrorsOf<T1, T2, T3, T4, T5, T6, T7, T8>>
        where T1 : IError
        where T2 : IError
        where T3 : IError
        where T4 : IError
        where T5 : IError
        where T6 : IError
        where T7 : IError
        where T8 : IError
    {
        private ErrorsOf(T1? v1, T2? v2, T3? v3, T4? v4, T5? v5, T6? v6, T7? v7, T8? v8, byte index) : base(v1, v2, v3, v4, v5, v6, v7, v8, index) { }

        /// <summary>Creates an ErrorsOf containing a T1 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4, T5, T6, T7, T8> FromT1(T1 value) => new(value, default, default, default, default, default, default, default, 0);

        /// <summary>Creates an ErrorsOf containing a T2 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4, T5, T6, T7, T8> FromT2(T2 value) => new(default, value, default, default, default, default, default, default, 1);

        /// <summary>Creates an ErrorsOf containing a T3 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4, T5, T6, T7, T8> FromT3(T3 value) => new(default, default, value, default, default, default, default, default, 2);

        /// <summary>Creates an ErrorsOf containing a T4 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4, T5, T6, T7, T8> FromT4(T4 value) => new(default, default, default, value, default, default, default, default, 3);

        /// <summary>Creates an ErrorsOf containing a T5 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4, T5, T6, T7, T8> FromT5(T5 value) => new(default, default, default, default, value, default, default, default, 4);

        /// <summary>Creates an ErrorsOf containing a T6 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4, T5, T6, T7, T8> FromT6(T6 value) => new(default, default, default, default, default, value, default, default, 5);

        /// <summary>Creates an ErrorsOf containing a T7 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4, T5, T6, T7, T8> FromT7(T7 value) => new(default, default, default, default, default, default, value, default, 6);

        /// <summary>Creates an ErrorsOf containing a T8 error.</summary>
        public static ErrorsOf<T1, T2, T3, T4, T5, T6, T7, T8> FromT8(T8 value) => new(default, default, default, default, default, default, default, value, 7);

        /// <summary>Implicit conversion from T1.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value) => FromT1(value);

        /// <summary>Implicit conversion from T2.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4, T5, T6, T7, T8>(T2 value) => FromT2(value);

        /// <summary>Implicit conversion from T3.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4, T5, T6, T7, T8>(T3 value) => FromT3(value);

        /// <summary>Implicit conversion from T4.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4, T5, T6, T7, T8>(T4 value) => FromT4(value);

        /// <summary>Implicit conversion from T5.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4, T5, T6, T7, T8>(T5 value) => FromT5(value);

        /// <summary>Implicit conversion from T6.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4, T5, T6, T7, T8>(T6 value) => FromT6(value);

        /// <summary>Implicit conversion from T7.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4, T5, T6, T7, T8>(T7 value) => FromT7(value);

        /// <summary>Implicit conversion from T8.</summary>
        public static implicit operator ErrorsOf<T1, T2, T3, T4, T5, T6, T7, T8>(T8 value) => FromT8(value);

        /// <inheritdoc/>
        public string Message { get => Match(e => e.Message, e => e.Message, e => e.Message, e => e.Message, e => e.Message, e => e.Message, e => e.Message, e => e.Message); init { } }

        /// <inheritdoc/>
        public ImmutableDictionary<string, object> Tags { get => Match(e => e.Tags, e => e.Tags, e => e.Tags, e => e.Tags, e => e.Tags, e => e.Tags, e => e.Tags, e => e.Tags); init { } }

        /// <summary>Determines equality between two ErrorsOf instances.</summary>
        public bool Equals(ErrorsOf<T1, T2, T3, T4, T5, T6, T7, T8>? other)
        {
            if (other is null || _index != other._index) return false;
            return _index switch
            {
                0 => EqualityComparer<T1>.Default.Equals(_value1, other._value1),
                1 => EqualityComparer<T2>.Default.Equals(_value2, other._value2),
                2 => EqualityComparer<T3>.Default.Equals(_value3, other._value3),
                3 => EqualityComparer<T4>.Default.Equals(_value4, other._value4),
                4 => EqualityComparer<T5>.Default.Equals(_value5, other._value5),
                5 => EqualityComparer<T6>.Default.Equals(_value6, other._value6),
                6 => EqualityComparer<T7>.Default.Equals(_value7, other._value7),
                7 => EqualityComparer<T8>.Default.Equals(_value8, other._value8),
                _ => false
            };
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is ErrorsOf<T1, T2, T3, T4, T5, T6, T7, T8> other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>Equality operator.</summary>
        public static bool operator ==(ErrorsOf<T1, T2, T3, T4, T5, T6, T7, T8>? left, ErrorsOf<T1, T2, T3, T4, T5, T6, T7, T8>? right) => Equals(left, right);

        /// <summary>Inequality operator.</summary>
        public static bool operator !=(ErrorsOf<T1, T2, T3, T4, T5, T6, T7, T8>? left, ErrorsOf<T1, T2, T3, T4, T5, T6, T7, T8>? right) => !Equals(left, right);
    }
}
