using System;
using System.Collections.Generic;

namespace REslava.Result.AdvancedPatterns
{
    /// <summary>
    /// Represents a value that can be one of three possible types.
    /// A type-safe discriminated union for functional programming patterns.
    /// </summary>
    /// <typeparam name="T1">The first possible type.</typeparam>
    /// <typeparam name="T2">The second possible type.</typeparam>
    /// <typeparam name="T3">The third possible type.</typeparam>
    /// <remarks>
    /// <para>
    /// OneOf&lt;T1, T2, T3&gt; provides a type-safe way to represent a value that can be one of three types.
    /// This is useful for scenarios where you need to handle different types of values or states
    /// without using null references, exceptions, or complex enums.
    /// </para>
    /// <para>
    /// Common use cases include:
    /// - API responses with three states: Success, ClientError, ServerError
    /// - Configuration values: String, Integer, Boolean
    /// - Database operations: Created, Updated, Deleted
    /// </para>
    /// <example>
    /// <code>
    /// OneOf&lt;SuccessData, ClientError, ServerError&gt; response = await CallApi();
    /// return response.Match(
    ///     case1: data => ProcessData(data),
    ///     case2: clientError => HandleClientError(clientError),
    ///     case3: serverError => HandleServerError(serverError)
    /// );
    /// </code>
    /// </example>
    /// </remarks>
    public sealed class OneOf<T1, T2, T3> : OneOfBase<T1, T2, T3>, IOneOf<T1, T2, T3>, IEquatable<OneOf<T1, T2, T3>>
    {
        private OneOf(T1? v1, T2? v2, T3? v3, byte index) : base(v1, v2, v3, index) { }

        /// <summary>Creates a OneOf containing a T1 value.</summary>
        public static OneOf<T1, T2, T3> FromT1(T1 value) => new(value, default, default, 0);

        /// <summary>Creates a OneOf containing a T2 value.</summary>
        public static OneOf<T1, T2, T3> FromT2(T2 value) => new(default, value, default, 1);

        /// <summary>Creates a OneOf containing a T3 value.</summary>
        public static OneOf<T1, T2, T3> FromT3(T3 value) => new(default, default, value, 2);

        /// <summary>Implicit conversion from T1 to OneOf&lt;T1, T2, T3&gt;.</summary>
        public static implicit operator OneOf<T1, T2, T3>(T1 value) => FromT1(value);

        /// <summary>Implicit conversion from T2 to OneOf&lt;T1, T2, T3&gt;.</summary>
        public static implicit operator OneOf<T1, T2, T3>(T2 value) => FromT2(value);

        /// <summary>Implicit conversion from T3 to OneOf&lt;T1, T2, T3&gt;.</summary>
        public static implicit operator OneOf<T1, T2, T3>(T3 value) => FromT3(value);

        /// <summary>Maps the T2 value if present, otherwise propagates other types.</summary>
        /// <exception cref="ArgumentNullException">Thrown when mapper is null.</exception>
        public OneOf<T1, TNewT2, T3> MapT2<TNewT2>(Func<T2, TNewT2> mapper)
        {
            if (mapper is null) throw new ArgumentNullException(nameof(mapper));
            return _index switch
            {
                0 => OneOf<T1, TNewT2, T3>.FromT1(_value1!),
                1 => OneOf<T1, TNewT2, T3>.FromT2(mapper(_value2!)),
                2 => OneOf<T1, TNewT2, T3>.FromT3(_value3!),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        /// <summary>Maps the T3 value if present, otherwise propagates other types.</summary>
        /// <exception cref="ArgumentNullException">Thrown when mapper is null.</exception>
        public OneOf<T1, T2, TNewT3> MapT3<TNewT3>(Func<T3, TNewT3> mapper)
        {
            if (mapper is null) throw new ArgumentNullException(nameof(mapper));
            return _index switch
            {
                0 => OneOf<T1, T2, TNewT3>.FromT1(_value1!),
                1 => OneOf<T1, T2, TNewT3>.FromT2(_value2!),
                2 => OneOf<T1, T2, TNewT3>.FromT3(mapper(_value3!)),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        /// <summary>Binds the T2 value if present, otherwise propagates other types.</summary>
        /// <exception cref="ArgumentNullException">Thrown when binder is null.</exception>
        public OneOf<T1, TNewT2, T3> BindT2<TNewT2>(Func<T2, OneOf<T1, TNewT2, T3>> binder)
        {
            if (binder is null) throw new ArgumentNullException(nameof(binder));
            return _index switch
            {
                0 => OneOf<T1, TNewT2, T3>.FromT1(_value1!),
                1 => binder(_value2!),
                2 => OneOf<T1, TNewT2, T3>.FromT3(_value3!),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        /// <summary>Binds the T3 value if present, otherwise propagates other types.</summary>
        /// <exception cref="ArgumentNullException">Thrown when binder is null.</exception>
        public OneOf<T1, T2, TNewT3> BindT3<TNewT3>(Func<T3, OneOf<T1, T2, TNewT3>> binder)
        {
            if (binder is null) throw new ArgumentNullException(nameof(binder));
            return _index switch
            {
                0 => OneOf<T1, T2, TNewT3>.FromT1(_value1!),
                1 => OneOf<T1, T2, TNewT3>.FromT2(_value2!),
                2 => binder(_value3!),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        /// <summary>Determines equality between two OneOf instances.</summary>
        public bool Equals(OneOf<T1, T2, T3>? other)
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
        public override bool Equals(object? obj) => obj is OneOf<T1, T2, T3> other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>Equality operator for OneOf instances.</summary>
        public static bool operator ==(OneOf<T1, T2, T3>? left, OneOf<T1, T2, T3>? right) => Equals(left, right);

        /// <summary>Inequality operator for OneOf instances.</summary>
        public static bool operator !=(OneOf<T1, T2, T3>? left, OneOf<T1, T2, T3>? right) => !Equals(left, right);
    }
}
