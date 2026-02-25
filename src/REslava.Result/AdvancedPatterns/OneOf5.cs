using System;
using System.Collections.Generic;

namespace REslava.Result.AdvancedPatterns
{
    /// <summary>
    /// Represents a value that can be one of five possible types.
    /// A type-safe discriminated union for functional programming patterns.
    /// </summary>
    /// <typeparam name="T1">The first possible type.</typeparam>
    /// <typeparam name="T2">The second possible type.</typeparam>
    /// <typeparam name="T3">The third possible type.</typeparam>
    /// <typeparam name="T4">The fourth possible type.</typeparam>
    /// <typeparam name="T5">The fifth possible type.</typeparam>
    /// <remarks>
    /// <para>
    /// OneOf&lt;T1, T2, T3, T4, T5&gt; provides a type-safe way to represent a value that can be one of five types.
    /// This is useful for scenarios where you need to handle multiple distinct states without using
    /// null references, exceptions, or complex enums.
    /// </para>
    /// <example>
    /// <code>
    /// OneOf&lt;SuccessData, ValidationError, NotFoundError, ConflictError, ServerError&gt; response = await CallApi();
    /// return response.Match(
    ///     case1: data => ProcessData(data),
    ///     case2: err => HandleValidation(err),
    ///     case3: err => HandleNotFound(err),
    ///     case4: err => HandleConflict(err),
    ///     case5: err => HandleServerError(err)
    /// );
    /// </code>
    /// </example>
    /// </remarks>
    public readonly struct OneOf<T1, T2, T3, T4, T5> : IEquatable<OneOf<T1, T2, T3, T4, T5>>
    {
        private readonly T1 _value1;
        private readonly T2 _value2;
        private readonly T3 _value3;
        private readonly T4 _value4;
        private readonly T5 _value5;
        private readonly byte _index;

        /// <summary>Gets whether the OneOf contains a value of type T1.</summary>
        public bool IsT1 => _index == 0;

        /// <summary>Gets whether the OneOf contains a value of type T2.</summary>
        public bool IsT2 => _index == 1;

        /// <summary>Gets whether the OneOf contains a value of type T3.</summary>
        public bool IsT3 => _index == 2;

        /// <summary>Gets whether the OneOf contains a value of type T4.</summary>
        public bool IsT4 => _index == 3;

        /// <summary>Gets whether the OneOf contains a value of type T5.</summary>
        public bool IsT5 => _index == 4;

        /// <summary>Gets the value as T1 if it contains T1, otherwise throws InvalidOperationException.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the OneOf does not contain T1.</exception>
        public T1 AsT1 => _index == 0 ? _value1 : throw new InvalidOperationException("OneOf does not contain T1");

        /// <summary>Gets the value as T2 if it contains T2, otherwise throws InvalidOperationException.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the OneOf does not contain T2.</exception>
        public T2 AsT2 => _index == 1 ? _value2 : throw new InvalidOperationException("OneOf does not contain T2");

        /// <summary>Gets the value as T3 if it contains T3, otherwise throws InvalidOperationException.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the OneOf does not contain T3.</exception>
        public T3 AsT3 => _index == 2 ? _value3 : throw new InvalidOperationException("OneOf does not contain T3");

        /// <summary>Gets the value as T4 if it contains T4, otherwise throws InvalidOperationException.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the OneOf does not contain T4.</exception>
        public T4 AsT4 => _index == 3 ? _value4 : throw new InvalidOperationException("OneOf does not contain T4");

        /// <summary>Gets the value as T5 if it contains T5, otherwise throws InvalidOperationException.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the OneOf does not contain T5.</exception>
        public T5 AsT5 => _index == 4 ? _value5 : throw new InvalidOperationException("OneOf does not contain T5");

        private OneOf(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, byte index)
        {
            _value1 = value1;
            _value2 = value2;
            _value3 = value3;
            _value4 = value4;
            _value5 = value5;
            _index = index;
        }

        /// <summary>Creates a OneOf containing a T1 value.</summary>
        public static OneOf<T1, T2, T3, T4, T5> FromT1(T1 value) =>
            new OneOf<T1, T2, T3, T4, T5>(value, default!, default!, default!, default!, 0);

        /// <summary>Creates a OneOf containing a T2 value.</summary>
        public static OneOf<T1, T2, T3, T4, T5> FromT2(T2 value) =>
            new OneOf<T1, T2, T3, T4, T5>(default!, value, default!, default!, default!, 1);

        /// <summary>Creates a OneOf containing a T3 value.</summary>
        public static OneOf<T1, T2, T3, T4, T5> FromT3(T3 value) =>
            new OneOf<T1, T2, T3, T4, T5>(default!, default!, value, default!, default!, 2);

        /// <summary>Creates a OneOf containing a T4 value.</summary>
        public static OneOf<T1, T2, T3, T4, T5> FromT4(T4 value) =>
            new OneOf<T1, T2, T3, T4, T5>(default!, default!, default!, value, default!, 3);

        /// <summary>Creates a OneOf containing a T5 value.</summary>
        public static OneOf<T1, T2, T3, T4, T5> FromT5(T5 value) =>
            new OneOf<T1, T2, T3, T4, T5>(default!, default!, default!, default!, value, 4);

        /// <summary>Implicit conversion from T1 to OneOf&lt;T1, T2, T3, T4, T5&gt;.</summary>
        public static implicit operator OneOf<T1, T2, T3, T4, T5>(T1 value) => FromT1(value);

        /// <summary>Implicit conversion from T2 to OneOf&lt;T1, T2, T3, T4, T5&gt;.</summary>
        public static implicit operator OneOf<T1, T2, T3, T4, T5>(T2 value) => FromT2(value);

        /// <summary>Implicit conversion from T3 to OneOf&lt;T1, T2, T3, T4, T5&gt;.</summary>
        public static implicit operator OneOf<T1, T2, T3, T4, T5>(T3 value) => FromT3(value);

        /// <summary>Implicit conversion from T4 to OneOf&lt;T1, T2, T3, T4, T5&gt;.</summary>
        public static implicit operator OneOf<T1, T2, T3, T4, T5>(T4 value) => FromT4(value);

        /// <summary>Implicit conversion from T5 to OneOf&lt;T1, T2, T3, T4, T5&gt;.</summary>
        public static implicit operator OneOf<T1, T2, T3, T4, T5>(T5 value) => FromT5(value);

        /// <summary>
        /// Pattern matching — executes the appropriate function based on the contained type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <exception cref="ArgumentNullException">Thrown when any case function is null.</exception>
        public TResult Match<TResult>(
            Func<T1, TResult> case1,
            Func<T2, TResult> case2,
            Func<T3, TResult> case3,
            Func<T4, TResult> case4,
            Func<T5, TResult> case5)
        {
            if (case1 == null) throw new ArgumentNullException(nameof(case1));
            if (case2 == null) throw new ArgumentNullException(nameof(case2));
            if (case3 == null) throw new ArgumentNullException(nameof(case3));
            if (case4 == null) throw new ArgumentNullException(nameof(case4));
            if (case5 == null) throw new ArgumentNullException(nameof(case5));

            return _index switch
            {
                0 => case1(_value1),
                1 => case2(_value2),
                2 => case3(_value3),
                3 => case4(_value4),
                4 => case5(_value5),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        /// <summary>
        /// Executes an action based on the contained type.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when any case action is null.</exception>
        public void Switch(
            Action<T1> case1,
            Action<T2> case2,
            Action<T3> case3,
            Action<T4> case4,
            Action<T5> case5)
        {
            if (case1 == null) throw new ArgumentNullException(nameof(case1));
            if (case2 == null) throw new ArgumentNullException(nameof(case2));
            if (case3 == null) throw new ArgumentNullException(nameof(case3));
            if (case4 == null) throw new ArgumentNullException(nameof(case4));
            if (case5 == null) throw new ArgumentNullException(nameof(case5));

            switch (_index)
            {
                case 0: case1(_value1); break;
                case 1: case2(_value2); break;
                case 2: case3(_value3); break;
                case 3: case4(_value4); break;
                case 4: case5(_value5); break;
                default: throw new InvalidOperationException("Invalid OneOf state");
            }
        }

        /// <summary>Maps the T2 value if present, otherwise propagates other types.</summary>
        /// <returns>A new OneOf with the mapped T2 value or the original T1/T3/T4/T5.</returns>
        /// <exception cref="ArgumentNullException">Thrown when mapper is null.</exception>
        public OneOf<T1, TNewT2, T3, T4, T5> MapT2<TNewT2>(Func<T2, TNewT2> mapper)
        {
            if (mapper == null) throw new ArgumentNullException(nameof(mapper));

            return _index switch
            {
                0 => OneOf<T1, TNewT2, T3, T4, T5>.FromT1(_value1),
                1 => OneOf<T1, TNewT2, T3, T4, T5>.FromT2(mapper(_value2)),
                2 => OneOf<T1, TNewT2, T3, T4, T5>.FromT3(_value3),
                3 => OneOf<T1, TNewT2, T3, T4, T5>.FromT4(_value4),
                4 => OneOf<T1, TNewT2, T3, T4, T5>.FromT5(_value5),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        /// <summary>Maps the T3 value if present, otherwise propagates other types.</summary>
        /// <returns>A new OneOf with the mapped T3 value or the original T1/T2/T4/T5.</returns>
        /// <exception cref="ArgumentNullException">Thrown when mapper is null.</exception>
        public OneOf<T1, T2, TNewT3, T4, T5> MapT3<TNewT3>(Func<T3, TNewT3> mapper)
        {
            if (mapper == null) throw new ArgumentNullException(nameof(mapper));

            return _index switch
            {
                0 => OneOf<T1, T2, TNewT3, T4, T5>.FromT1(_value1),
                1 => OneOf<T1, T2, TNewT3, T4, T5>.FromT2(_value2),
                2 => OneOf<T1, T2, TNewT3, T4, T5>.FromT3(mapper(_value3)),
                3 => OneOf<T1, T2, TNewT3, T4, T5>.FromT4(_value4),
                4 => OneOf<T1, T2, TNewT3, T4, T5>.FromT5(_value5),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        /// <summary>Maps the T4 value if present, otherwise propagates other types.</summary>
        /// <returns>A new OneOf with the mapped T4 value or the original T1/T2/T3/T5.</returns>
        /// <exception cref="ArgumentNullException">Thrown when mapper is null.</exception>
        public OneOf<T1, T2, T3, TNewT4, T5> MapT4<TNewT4>(Func<T4, TNewT4> mapper)
        {
            if (mapper == null) throw new ArgumentNullException(nameof(mapper));

            return _index switch
            {
                0 => OneOf<T1, T2, T3, TNewT4, T5>.FromT1(_value1),
                1 => OneOf<T1, T2, T3, TNewT4, T5>.FromT2(_value2),
                2 => OneOf<T1, T2, T3, TNewT4, T5>.FromT3(_value3),
                3 => OneOf<T1, T2, T3, TNewT4, T5>.FromT4(mapper(_value4)),
                4 => OneOf<T1, T2, T3, TNewT4, T5>.FromT5(_value5),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        /// <summary>Binds the T2 value if present, otherwise propagates other types.</summary>
        /// <returns>The result of the binder function or the original T1/T3/T4/T5.</returns>
        /// <exception cref="ArgumentNullException">Thrown when binder is null.</exception>
        public OneOf<T1, TNewT2, T3, T4, T5> BindT2<TNewT2>(Func<T2, OneOf<T1, TNewT2, T3, T4, T5>> binder)
        {
            if (binder == null) throw new ArgumentNullException(nameof(binder));

            return _index switch
            {
                0 => OneOf<T1, TNewT2, T3, T4, T5>.FromT1(_value1),
                1 => binder(_value2),
                2 => OneOf<T1, TNewT2, T3, T4, T5>.FromT3(_value3),
                3 => OneOf<T1, TNewT2, T3, T4, T5>.FromT4(_value4),
                4 => OneOf<T1, TNewT2, T3, T4, T5>.FromT5(_value5),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        /// <summary>Binds the T3 value if present, otherwise propagates other types.</summary>
        /// <returns>The result of the binder function or the original T1/T2/T4/T5.</returns>
        /// <exception cref="ArgumentNullException">Thrown when binder is null.</exception>
        public OneOf<T1, T2, TNewT3, T4, T5> BindT3<TNewT3>(Func<T3, OneOf<T1, T2, TNewT3, T4, T5>> binder)
        {
            if (binder == null) throw new ArgumentNullException(nameof(binder));

            return _index switch
            {
                0 => OneOf<T1, T2, TNewT3, T4, T5>.FromT1(_value1),
                1 => OneOf<T1, T2, TNewT3, T4, T5>.FromT2(_value2),
                2 => binder(_value3),
                3 => OneOf<T1, T2, TNewT3, T4, T5>.FromT4(_value4),
                4 => OneOf<T1, T2, TNewT3, T4, T5>.FromT5(_value5),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        /// <summary>Binds the T4 value if present, otherwise propagates other types.</summary>
        /// <returns>The result of the binder function or the original T1/T2/T3/T5.</returns>
        /// <exception cref="ArgumentNullException">Thrown when binder is null.</exception>
        public OneOf<T1, T2, T3, TNewT4, T5> BindT4<TNewT4>(Func<T4, OneOf<T1, T2, T3, TNewT4, T5>> binder)
        {
            if (binder == null) throw new ArgumentNullException(nameof(binder));

            return _index switch
            {
                0 => OneOf<T1, T2, T3, TNewT4, T5>.FromT1(_value1),
                1 => OneOf<T1, T2, T3, TNewT4, T5>.FromT2(_value2),
                2 => OneOf<T1, T2, T3, TNewT4, T5>.FromT3(_value3),
                3 => binder(_value4),
                4 => OneOf<T1, T2, T3, TNewT4, T5>.FromT5(_value5),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        /// <summary>Returns a string representation of the OneOf for debugging.</summary>
        public override string ToString()
        {
            return _index switch
            {
                0 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}>(T1: {_value1})",
                1 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}>(T2: {_value2})",
                2 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}>(T3: {_value3})",
                3 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}>(T4: {_value4})",
                4 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}>(T5: {_value5})",
                _ => "OneOf<Invalid>"
            };
        }

        /// <summary>Indicates whether the current OneOf is equal to another OneOf of the same type.</summary>
        public bool Equals(OneOf<T1, T2, T3, T4, T5> other)
        {
            if (_index != other._index) return false;

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

        /// <summary>Indicates whether the current OneOf is equal to another object.</summary>
        public override bool Equals(object? obj) =>
            obj is OneOf<T1, T2, T3, T4, T5> other && Equals(other);

        /// <summary>Returns the hash code for the OneOf.</summary>
        public override int GetHashCode() =>
            HashCode.Combine(_index, _value1, _value2, _value3, _value4, _value5);

        /// <summary>Determines whether two OneOf instances are equal.</summary>
        public static bool operator ==(OneOf<T1, T2, T3, T4, T5> left, OneOf<T1, T2, T3, T4, T5> right) =>
            left.Equals(right);

        /// <summary>Determines whether two OneOf instances are not equal.</summary>
        public static bool operator !=(OneOf<T1, T2, T3, T4, T5> left, OneOf<T1, T2, T3, T4, T5> right) =>
            !left.Equals(right);
    }
}
