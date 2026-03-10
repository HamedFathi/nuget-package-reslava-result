using System;
using System.Collections.Generic;

namespace REslava.Result.AdvancedPatterns
{
    /// <summary>
    /// Represents a value that can be one of seven possible types.
    /// A type-safe discriminated union for functional programming patterns.
    /// </summary>
    /// <typeparam name="T1">The first possible type.</typeparam>
    /// <typeparam name="T2">The second possible type.</typeparam>
    /// <typeparam name="T3">The third possible type.</typeparam>
    /// <typeparam name="T4">The fourth possible type.</typeparam>
    /// <typeparam name="T5">The fifth possible type.</typeparam>
    /// <typeparam name="T6">The sixth possible type.</typeparam>
    /// <typeparam name="T7">The seventh possible type.</typeparam>
    public sealed class OneOf<T1, T2, T3, T4, T5, T6, T7> : OneOfBase<T1, T2, T3, T4, T5, T6, T7>, IOneOf<T1, T2, T3, T4, T5, T6, T7>, IEquatable<OneOf<T1, T2, T3, T4, T5, T6, T7>>
    {
        private OneOf(T1? v1, T2? v2, T3? v3, T4? v4, T5? v5, T6? v6, T7? v7, byte index) : base(v1, v2, v3, v4, v5, v6, v7, index) { }

        /// <summary>Creates a OneOf containing a T1 value.</summary>
        public static OneOf<T1, T2, T3, T4, T5, T6, T7> FromT1(T1 value) => new(value, default, default, default, default, default, default, 0);

        /// <summary>Creates a OneOf containing a T2 value.</summary>
        public static OneOf<T1, T2, T3, T4, T5, T6, T7> FromT2(T2 value) => new(default, value, default, default, default, default, default, 1);

        /// <summary>Creates a OneOf containing a T3 value.</summary>
        public static OneOf<T1, T2, T3, T4, T5, T6, T7> FromT3(T3 value) => new(default, default, value, default, default, default, default, 2);

        /// <summary>Creates a OneOf containing a T4 value.</summary>
        public static OneOf<T1, T2, T3, T4, T5, T6, T7> FromT4(T4 value) => new(default, default, default, value, default, default, default, 3);

        /// <summary>Creates a OneOf containing a T5 value.</summary>
        public static OneOf<T1, T2, T3, T4, T5, T6, T7> FromT5(T5 value) => new(default, default, default, default, value, default, default, 4);

        /// <summary>Creates a OneOf containing a T6 value.</summary>
        public static OneOf<T1, T2, T3, T4, T5, T6, T7> FromT6(T6 value) => new(default, default, default, default, default, value, default, 5);

        /// <summary>Creates a OneOf containing a T7 value.</summary>
        public static OneOf<T1, T2, T3, T4, T5, T6, T7> FromT7(T7 value) => new(default, default, default, default, default, default, value, 6);

        /// <summary>Implicit conversion from T1.</summary>
        public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7>(T1 value) => FromT1(value);

        /// <summary>Implicit conversion from T2.</summary>
        public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7>(T2 value) => FromT2(value);

        /// <summary>Implicit conversion from T3.</summary>
        public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7>(T3 value) => FromT3(value);

        /// <summary>Implicit conversion from T4.</summary>
        public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7>(T4 value) => FromT4(value);

        /// <summary>Implicit conversion from T5.</summary>
        public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7>(T5 value) => FromT5(value);

        /// <summary>Implicit conversion from T6.</summary>
        public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7>(T6 value) => FromT6(value);

        /// <summary>Implicit conversion from T7.</summary>
        public static implicit operator OneOf<T1, T2, T3, T4, T5, T6, T7>(T7 value) => FromT7(value);

        /// <summary>Maps the T2 value if present, otherwise propagates other types.</summary>
        /// <exception cref="ArgumentNullException">Thrown when mapper is null.</exception>
        public OneOf<T1, TNewT2, T3, T4, T5, T6, T7> MapT2<TNewT2>(Func<T2, TNewT2> mapper)
        {
            if (mapper is null) throw new ArgumentNullException(nameof(mapper));
            return _index switch
            {
                0 => OneOf<T1, TNewT2, T3, T4, T5, T6, T7>.FromT1(_value1!),
                1 => OneOf<T1, TNewT2, T3, T4, T5, T6, T7>.FromT2(mapper(_value2!)),
                2 => OneOf<T1, TNewT2, T3, T4, T5, T6, T7>.FromT3(_value3!),
                3 => OneOf<T1, TNewT2, T3, T4, T5, T6, T7>.FromT4(_value4!),
                4 => OneOf<T1, TNewT2, T3, T4, T5, T6, T7>.FromT5(_value5!),
                5 => OneOf<T1, TNewT2, T3, T4, T5, T6, T7>.FromT6(_value6!),
                6 => OneOf<T1, TNewT2, T3, T4, T5, T6, T7>.FromT7(_value7!),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        /// <summary>Maps the T3 value if present, otherwise propagates other types.</summary>
        /// <exception cref="ArgumentNullException">Thrown when mapper is null.</exception>
        public OneOf<T1, T2, TNewT3, T4, T5, T6, T7> MapT3<TNewT3>(Func<T3, TNewT3> mapper)
        {
            if (mapper is null) throw new ArgumentNullException(nameof(mapper));
            return _index switch
            {
                0 => OneOf<T1, T2, TNewT3, T4, T5, T6, T7>.FromT1(_value1!),
                1 => OneOf<T1, T2, TNewT3, T4, T5, T6, T7>.FromT2(_value2!),
                2 => OneOf<T1, T2, TNewT3, T4, T5, T6, T7>.FromT3(mapper(_value3!)),
                3 => OneOf<T1, T2, TNewT3, T4, T5, T6, T7>.FromT4(_value4!),
                4 => OneOf<T1, T2, TNewT3, T4, T5, T6, T7>.FromT5(_value5!),
                5 => OneOf<T1, T2, TNewT3, T4, T5, T6, T7>.FromT6(_value6!),
                6 => OneOf<T1, T2, TNewT3, T4, T5, T6, T7>.FromT7(_value7!),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        /// <summary>Maps the T4 value if present, otherwise propagates other types.</summary>
        /// <exception cref="ArgumentNullException">Thrown when mapper is null.</exception>
        public OneOf<T1, T2, T3, TNewT4, T5, T6, T7> MapT4<TNewT4>(Func<T4, TNewT4> mapper)
        {
            if (mapper is null) throw new ArgumentNullException(nameof(mapper));
            return _index switch
            {
                0 => OneOf<T1, T2, T3, TNewT4, T5, T6, T7>.FromT1(_value1!),
                1 => OneOf<T1, T2, T3, TNewT4, T5, T6, T7>.FromT2(_value2!),
                2 => OneOf<T1, T2, T3, TNewT4, T5, T6, T7>.FromT3(_value3!),
                3 => OneOf<T1, T2, T3, TNewT4, T5, T6, T7>.FromT4(mapper(_value4!)),
                4 => OneOf<T1, T2, T3, TNewT4, T5, T6, T7>.FromT5(_value5!),
                5 => OneOf<T1, T2, T3, TNewT4, T5, T6, T7>.FromT6(_value6!),
                6 => OneOf<T1, T2, T3, TNewT4, T5, T6, T7>.FromT7(_value7!),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        /// <summary>Maps the T5 value if present, otherwise propagates other types.</summary>
        /// <exception cref="ArgumentNullException">Thrown when mapper is null.</exception>
        public OneOf<T1, T2, T3, T4, TNewT5, T6, T7> MapT5<TNewT5>(Func<T5, TNewT5> mapper)
        {
            if (mapper is null) throw new ArgumentNullException(nameof(mapper));
            return _index switch
            {
                0 => OneOf<T1, T2, T3, T4, TNewT5, T6, T7>.FromT1(_value1!),
                1 => OneOf<T1, T2, T3, T4, TNewT5, T6, T7>.FromT2(_value2!),
                2 => OneOf<T1, T2, T3, T4, TNewT5, T6, T7>.FromT3(_value3!),
                3 => OneOf<T1, T2, T3, T4, TNewT5, T6, T7>.FromT4(_value4!),
                4 => OneOf<T1, T2, T3, T4, TNewT5, T6, T7>.FromT5(mapper(_value5!)),
                5 => OneOf<T1, T2, T3, T4, TNewT5, T6, T7>.FromT6(_value6!),
                6 => OneOf<T1, T2, T3, T4, TNewT5, T6, T7>.FromT7(_value7!),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        /// <summary>Maps the T6 value if present, otherwise propagates other types.</summary>
        /// <exception cref="ArgumentNullException">Thrown when mapper is null.</exception>
        public OneOf<T1, T2, T3, T4, T5, TNewT6, T7> MapT6<TNewT6>(Func<T6, TNewT6> mapper)
        {
            if (mapper is null) throw new ArgumentNullException(nameof(mapper));
            return _index switch
            {
                0 => OneOf<T1, T2, T3, T4, T5, TNewT6, T7>.FromT1(_value1!),
                1 => OneOf<T1, T2, T3, T4, T5, TNewT6, T7>.FromT2(_value2!),
                2 => OneOf<T1, T2, T3, T4, T5, TNewT6, T7>.FromT3(_value3!),
                3 => OneOf<T1, T2, T3, T4, T5, TNewT6, T7>.FromT4(_value4!),
                4 => OneOf<T1, T2, T3, T4, T5, TNewT6, T7>.FromT5(_value5!),
                5 => OneOf<T1, T2, T3, T4, T5, TNewT6, T7>.FromT6(mapper(_value6!)),
                6 => OneOf<T1, T2, T3, T4, T5, TNewT6, T7>.FromT7(_value7!),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        /// <summary>Binds the T2 value if present, otherwise propagates other types.</summary>
        /// <exception cref="ArgumentNullException">Thrown when binder is null.</exception>
        public OneOf<T1, TNewT2, T3, T4, T5, T6, T7> BindT2<TNewT2>(Func<T2, OneOf<T1, TNewT2, T3, T4, T5, T6, T7>> binder)
        {
            if (binder is null) throw new ArgumentNullException(nameof(binder));
            return _index switch
            {
                0 => OneOf<T1, TNewT2, T3, T4, T5, T6, T7>.FromT1(_value1!),
                1 => binder(_value2!),
                2 => OneOf<T1, TNewT2, T3, T4, T5, T6, T7>.FromT3(_value3!),
                3 => OneOf<T1, TNewT2, T3, T4, T5, T6, T7>.FromT4(_value4!),
                4 => OneOf<T1, TNewT2, T3, T4, T5, T6, T7>.FromT5(_value5!),
                5 => OneOf<T1, TNewT2, T3, T4, T5, T6, T7>.FromT6(_value6!),
                6 => OneOf<T1, TNewT2, T3, T4, T5, T6, T7>.FromT7(_value7!),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        /// <summary>Binds the T3 value if present, otherwise propagates other types.</summary>
        /// <exception cref="ArgumentNullException">Thrown when binder is null.</exception>
        public OneOf<T1, T2, TNewT3, T4, T5, T6, T7> BindT3<TNewT3>(Func<T3, OneOf<T1, T2, TNewT3, T4, T5, T6, T7>> binder)
        {
            if (binder is null) throw new ArgumentNullException(nameof(binder));
            return _index switch
            {
                0 => OneOf<T1, T2, TNewT3, T4, T5, T6, T7>.FromT1(_value1!),
                1 => OneOf<T1, T2, TNewT3, T4, T5, T6, T7>.FromT2(_value2!),
                2 => binder(_value3!),
                3 => OneOf<T1, T2, TNewT3, T4, T5, T6, T7>.FromT4(_value4!),
                4 => OneOf<T1, T2, TNewT3, T4, T5, T6, T7>.FromT5(_value5!),
                5 => OneOf<T1, T2, TNewT3, T4, T5, T6, T7>.FromT6(_value6!),
                6 => OneOf<T1, T2, TNewT3, T4, T5, T6, T7>.FromT7(_value7!),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        /// <summary>Binds the T4 value if present, otherwise propagates other types.</summary>
        /// <exception cref="ArgumentNullException">Thrown when binder is null.</exception>
        public OneOf<T1, T2, T3, TNewT4, T5, T6, T7> BindT4<TNewT4>(Func<T4, OneOf<T1, T2, T3, TNewT4, T5, T6, T7>> binder)
        {
            if (binder is null) throw new ArgumentNullException(nameof(binder));
            return _index switch
            {
                0 => OneOf<T1, T2, T3, TNewT4, T5, T6, T7>.FromT1(_value1!),
                1 => OneOf<T1, T2, T3, TNewT4, T5, T6, T7>.FromT2(_value2!),
                2 => OneOf<T1, T2, T3, TNewT4, T5, T6, T7>.FromT3(_value3!),
                3 => binder(_value4!),
                4 => OneOf<T1, T2, T3, TNewT4, T5, T6, T7>.FromT5(_value5!),
                5 => OneOf<T1, T2, T3, TNewT4, T5, T6, T7>.FromT6(_value6!),
                6 => OneOf<T1, T2, T3, TNewT4, T5, T6, T7>.FromT7(_value7!),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        /// <summary>Binds the T5 value if present, otherwise propagates other types.</summary>
        /// <exception cref="ArgumentNullException">Thrown when binder is null.</exception>
        public OneOf<T1, T2, T3, T4, TNewT5, T6, T7> BindT5<TNewT5>(Func<T5, OneOf<T1, T2, T3, T4, TNewT5, T6, T7>> binder)
        {
            if (binder is null) throw new ArgumentNullException(nameof(binder));
            return _index switch
            {
                0 => OneOf<T1, T2, T3, T4, TNewT5, T6, T7>.FromT1(_value1!),
                1 => OneOf<T1, T2, T3, T4, TNewT5, T6, T7>.FromT2(_value2!),
                2 => OneOf<T1, T2, T3, T4, TNewT5, T6, T7>.FromT3(_value3!),
                3 => OneOf<T1, T2, T3, T4, TNewT5, T6, T7>.FromT4(_value4!),
                4 => binder(_value5!),
                5 => OneOf<T1, T2, T3, T4, TNewT5, T6, T7>.FromT6(_value6!),
                6 => OneOf<T1, T2, T3, T4, TNewT5, T6, T7>.FromT7(_value7!),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        /// <summary>Binds the T6 value if present, otherwise propagates other types.</summary>
        /// <exception cref="ArgumentNullException">Thrown when binder is null.</exception>
        public OneOf<T1, T2, T3, T4, T5, TNewT6, T7> BindT6<TNewT6>(Func<T6, OneOf<T1, T2, T3, T4, T5, TNewT6, T7>> binder)
        {
            if (binder is null) throw new ArgumentNullException(nameof(binder));
            return _index switch
            {
                0 => OneOf<T1, T2, T3, T4, T5, TNewT6, T7>.FromT1(_value1!),
                1 => OneOf<T1, T2, T3, T4, T5, TNewT6, T7>.FromT2(_value2!),
                2 => OneOf<T1, T2, T3, T4, T5, TNewT6, T7>.FromT3(_value3!),
                3 => OneOf<T1, T2, T3, T4, T5, TNewT6, T7>.FromT4(_value4!),
                4 => OneOf<T1, T2, T3, T4, T5, TNewT6, T7>.FromT5(_value5!),
                5 => binder(_value6!),
                6 => OneOf<T1, T2, T3, T4, T5, TNewT6, T7>.FromT7(_value7!),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        /// <summary>Determines equality between two OneOf instances.</summary>
        public bool Equals(OneOf<T1, T2, T3, T4, T5, T6, T7>? other)
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
        public override bool Equals(object? obj) => obj is OneOf<T1, T2, T3, T4, T5, T6, T7> other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>Equality operator for OneOf instances.</summary>
        public static bool operator ==(OneOf<T1, T2, T3, T4, T5, T6, T7>? left, OneOf<T1, T2, T3, T4, T5, T6, T7>? right) => Equals(left, right);

        /// <summary>Inequality operator for OneOf instances.</summary>
        public static bool operator !=(OneOf<T1, T2, T3, T4, T5, T6, T7>? left, OneOf<T1, T2, T3, T4, T5, T6, T7>? right) => !Equals(left, right);
    }
}
