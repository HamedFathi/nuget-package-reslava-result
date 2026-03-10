using System;
using System.Collections.Generic;

namespace REslava.Result.AdvancedPatterns
{
    /// <summary>
    /// Represents a value that can be one of two possible types.
    /// A type-safe discriminated union for functional programming patterns.
    /// </summary>
    /// <typeparam name="T1">The first possible type.</typeparam>
    /// <typeparam name="T2">The second possible type.</typeparam>
    /// <remarks>
    /// <para>
    /// OneOf&lt;T1, T2&gt; provides a type-safe way to represent a value that can be one of two types.
    /// This is useful for scenarios where you need to handle different types of values or errors
    /// without using null references or exceptions.
    /// </para>
    /// <para>
    /// Common use cases include:
    /// - Error handling with typed errors: OneOf&lt;Error, Success&gt;
    /// - Configuration values: OneOf&lt;string, int&gt;
    /// - API responses: OneOf&lt;ValidationError, Data&gt;
    /// </para>
    /// <example>
    /// <code>
    /// // Error handling
    /// OneOf&lt;Error, User&gt; user = GetUser(id);
    /// return user.Match(
    ///     error => HandleError(error),
    ///     user => ProcessUser(user)
    /// );
    ///
    /// // Configuration parsing
    /// OneOf&lt;string, int&gt; config = GetConfig("timeout");
    /// int timeout = config.Match(
    ///     str => int.Parse(str),
    ///     num => num
    /// );
    /// </code>
    /// </example>
    /// </remarks>
    public sealed class OneOf<T1, T2> : OneOfBase<T1, T2>, IOneOf<T1, T2>, IEquatable<OneOf<T1, T2>>
    {
        private OneOf(T1? value1, T2? value2, byte index) : base(value1, value2, index) { }

        /// <summary>
        /// Creates a OneOf containing a T1 value.
        /// </summary>
        /// <param name="value">The T1 value to wrap.</param>
        /// <returns>A OneOf containing the specified T1 value.</returns>
        /// <example>
        /// <code>
        /// OneOf&lt;Error, User&gt; result = OneOf&lt;Error, User&gt;.FromT1(new NotFoundError());
        /// </code>
        /// </example>
        public static OneOf<T1, T2> FromT1(T1 value) => new(value, default, 0);

        /// <summary>
        /// Creates a OneOf containing a T2 value.
        /// </summary>
        /// <param name="value">The T2 value to wrap.</param>
        /// <returns>A OneOf containing the specified T2 value.</returns>
        /// <example>
        /// <code>
        /// OneOf&lt;Error, User&gt; result = OneOf&lt;Error, User&gt;.FromT2(new User("Alice"));
        /// </code>
        /// </example>
        public static OneOf<T1, T2> FromT2(T2 value) => new(default, value, 1);

        /// <summary>
        /// Implicit conversion from T1 to OneOf&lt;T1, T2&gt;.
        /// </summary>
        /// <example>
        /// <code>
        /// OneOf&lt;Error, User&gt; result = new NotFoundError(); // Implicit conversion
        /// </code>
        /// </example>
        public static implicit operator OneOf<T1, T2>(T1 value) => FromT1(value);

        /// <summary>
        /// Implicit conversion from T2 to OneOf&lt;T1, T2&gt;.
        /// </summary>
        /// <example>
        /// <code>
        /// OneOf&lt;Error, User&gt; result = new User("Alice"); // Implicit conversion
        /// </code>
        /// </example>
        public static implicit operator OneOf<T1, T2>(T2 value) => FromT2(value);

        /// <summary>
        /// Maps the T2 value if present, otherwise propagates T1.
        /// </summary>
        /// <typeparam name="TNewT2">The new T2 type.</typeparam>
        /// <param name="mapper">The function to apply to the T2 value.</param>
        /// <returns>A new OneOf with the mapped T2 value or the original T1.</returns>
        /// <exception cref="ArgumentNullException">Thrown when mapper is null.</exception>
        /// <example>
        /// <code>
        /// OneOf&lt;Error, User&gt; user = GetUser(id);
        /// OneOf&lt;Error, string&gt; userName = user.Map(u => u.Name);
        /// </code>
        /// </example>
        public OneOf<T1, TNewT2> Map<TNewT2>(Func<T2, TNewT2> mapper)
        {
            if (mapper is null) throw new ArgumentNullException(nameof(mapper));
            return _index switch
            {
                0 => OneOf<T1, TNewT2>.FromT1(_value1!),
                1 => OneOf<T1, TNewT2>.FromT2(mapper(_value2!)),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        /// <summary>
        /// Binds the T2 value if present, otherwise propagates T1.
        /// </summary>
        /// <typeparam name="TNewT2">The new T2 type.</typeparam>
        /// <param name="binder">The function that takes T2 and returns a OneOf.</param>
        /// <returns>The result of the binder function or the original T1.</returns>
        /// <exception cref="ArgumentNullException">Thrown when binder is null.</exception>
        /// <example>
        /// <code>
        /// OneOf&lt;Error, int&gt; userId = GetUserId();
        /// OneOf&lt;Error, User&gt; user = userId.Bind(id =&gt; GetUser(id));
        /// </code>
        /// </example>
        public OneOf<T1, TNewT2> Bind<TNewT2>(Func<T2, OneOf<T1, TNewT2>> binder)
        {
            if (binder is null) throw new ArgumentNullException(nameof(binder));
            return _index switch
            {
                0 => OneOf<T1, TNewT2>.FromT1(_value1!),
                1 => binder(_value2!),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        /// <summary>
        /// Filters the T2 value if it satisfies the predicate, otherwise converts to T1.
        /// </summary>
        /// <param name="predicate">The condition to test the T2 value against.</param>
        /// <param name="fallbackT1">The T1 value to use when the predicate fails.</param>
        /// <returns>The original OneOf if the predicate is true, otherwise the fallback T1.</returns>
        /// <exception cref="ArgumentNullException">Thrown when predicate is null.</exception>
        /// <example>
        /// <code>
        /// OneOf&lt;Error, User&gt; user = GetUser(id);
        /// OneOf&lt;Error, User&gt; activeUser = user.Filter(u => u.IsActive, new UserInactiveError());
        /// </code>
        /// </example>
        public OneOf<T1, T2> Filter(Func<T2, bool> predicate, T1 fallbackT1)
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            return _index switch
            {
                0 => this,
                1 => predicate(_value2!) ? this : FromT1(fallbackT1),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        /// <summary>
        /// Determines equality between two OneOf instances.
        /// </summary>
        /// <param name="other">The other OneOf to compare with.</param>
        /// <returns>true if the OneOf instances are equal; otherwise, false.</returns>
        public bool Equals(OneOf<T1, T2>? other)
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
        public override bool Equals(object? obj) => obj is OneOf<T1, T2> other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>Equality operator for OneOf instances.</summary>
        public static bool operator ==(OneOf<T1, T2>? left, OneOf<T1, T2>? right) => Equals(left, right);

        /// <summary>Inequality operator for OneOf instances.</summary>
        public static bool operator !=(OneOf<T1, T2>? left, OneOf<T1, T2>? right) => !Equals(left, right);
    }
}
