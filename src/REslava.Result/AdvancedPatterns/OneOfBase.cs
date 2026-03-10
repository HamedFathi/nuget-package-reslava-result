using System;
using System.Collections.Generic;

namespace REslava.Result.AdvancedPatterns
{
    /// <summary>
    /// Shared dispatch base for discriminated unions of two types.
    /// Inherited by <see cref="OneOf{T1,T2}"/> and <see cref="ErrorsOf{T1,T2}"/>.
    /// </summary>
    public abstract class OneOfBase<T1, T2>
    {
        private protected readonly T1? _value1;
        private protected readonly T2? _value2;
        private protected readonly byte _index;

        private protected OneOfBase(T1? value1, T2? value2, byte index)
        {
            _value1 = value1;
            _value2 = value2;
            _index  = index;
        }

        public bool IsT1 => _index == 0;
        public bool IsT2 => _index == 1;

        public T1 AsT1 => _index == 0 ? _value1! : throw new InvalidOperationException("OneOf contains T2, not T1");
        public T2 AsT2 => _index == 1 ? _value2! : throw new InvalidOperationException("OneOf contains T1, not T2");

        public TResult Match<TResult>(Func<T1, TResult> case1, Func<T2, TResult> case2)
        {
            if (case1 is null) throw new ArgumentNullException(nameof(case1));
            if (case2 is null) throw new ArgumentNullException(nameof(case2));
            return _index switch
            {
                0 => case1(_value1!),
                1 => case2(_value2!),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        public void Switch(Action<T1> case1, Action<T2> case2)
        {
            if (case1 is null) throw new ArgumentNullException(nameof(case1));
            if (case2 is null) throw new ArgumentNullException(nameof(case2));
            switch (_index)
            {
                case 0: case1(_value1!); break;
                case 1: case2(_value2!); break;
                default: throw new InvalidOperationException("Invalid OneOf state");
            }
        }

        public override string ToString() => _index switch
        {
            0 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}>(T1: {_value1})",
            1 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}>(T2: {_value2})",
            _ => "OneOf<Invalid>"
        };

        public override bool Equals(object? obj)
        {
            if (obj is not OneOfBase<T1, T2> other || _index != other._index) return false;
            return _index switch
            {
                0 => EqualityComparer<T1>.Default.Equals(_value1, other._value1),
                1 => EqualityComparer<T2>.Default.Equals(_value2, other._value2),
                _ => false
            };
        }

        public override int GetHashCode() => HashCode.Combine(_index, _value1, _value2);
    }

    /// <summary>
    /// Shared dispatch base for discriminated unions of three types.
    /// </summary>
    public abstract class OneOfBase<T1, T2, T3>
    {
        private protected readonly T1? _value1;
        private protected readonly T2? _value2;
        private protected readonly T3? _value3;
        private protected readonly byte _index;

        private protected OneOfBase(T1? value1, T2? value2, T3? value3, byte index)
        {
            _value1 = value1;
            _value2 = value2;
            _value3 = value3;
            _index  = index;
        }

        public bool IsT1 => _index == 0;
        public bool IsT2 => _index == 1;
        public bool IsT3 => _index == 2;

        public T1 AsT1 => _index == 0 ? _value1! : throw new InvalidOperationException("OneOf does not contain T1");
        public T2 AsT2 => _index == 1 ? _value2! : throw new InvalidOperationException("OneOf does not contain T2");
        public T3 AsT3 => _index == 2 ? _value3! : throw new InvalidOperationException("OneOf does not contain T3");

        public TResult Match<TResult>(Func<T1, TResult> case1, Func<T2, TResult> case2, Func<T3, TResult> case3)
        {
            if (case1 is null) throw new ArgumentNullException(nameof(case1));
            if (case2 is null) throw new ArgumentNullException(nameof(case2));
            if (case3 is null) throw new ArgumentNullException(nameof(case3));
            return _index switch
            {
                0 => case1(_value1!),
                1 => case2(_value2!),
                2 => case3(_value3!),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        public void Switch(Action<T1> case1, Action<T2> case2, Action<T3> case3)
        {
            if (case1 is null) throw new ArgumentNullException(nameof(case1));
            if (case2 is null) throw new ArgumentNullException(nameof(case2));
            if (case3 is null) throw new ArgumentNullException(nameof(case3));
            switch (_index)
            {
                case 0: case1(_value1!); break;
                case 1: case2(_value2!); break;
                case 2: case3(_value3!); break;
                default: throw new InvalidOperationException("Invalid OneOf state");
            }
        }

        public override string ToString() => _index switch
        {
            0 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}>(T1: {_value1})",
            1 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}>(T2: {_value2})",
            2 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}>(T3: {_value3})",
            _ => "OneOf<Invalid>"
        };

        public override bool Equals(object? obj)
        {
            if (obj is not OneOfBase<T1, T2, T3> other || _index != other._index) return false;
            return _index switch
            {
                0 => EqualityComparer<T1>.Default.Equals(_value1, other._value1),
                1 => EqualityComparer<T2>.Default.Equals(_value2, other._value2),
                2 => EqualityComparer<T3>.Default.Equals(_value3, other._value3),
                _ => false
            };
        }

        public override int GetHashCode() => HashCode.Combine(_index, _value1, _value2, _value3);
    }

    /// <summary>
    /// Shared dispatch base for discriminated unions of four types.
    /// </summary>
    public abstract class OneOfBase<T1, T2, T3, T4>
    {
        private protected readonly T1? _value1;
        private protected readonly T2? _value2;
        private protected readonly T3? _value3;
        private protected readonly T4? _value4;
        private protected readonly byte _index;

        private protected OneOfBase(T1? value1, T2? value2, T3? value3, T4? value4, byte index)
        {
            _value1 = value1;
            _value2 = value2;
            _value3 = value3;
            _value4 = value4;
            _index  = index;
        }

        public bool IsT1 => _index == 0;
        public bool IsT2 => _index == 1;
        public bool IsT3 => _index == 2;
        public bool IsT4 => _index == 3;

        public T1 AsT1 => _index == 0 ? _value1! : throw new InvalidOperationException("OneOf does not contain T1");
        public T2 AsT2 => _index == 1 ? _value2! : throw new InvalidOperationException("OneOf does not contain T2");
        public T3 AsT3 => _index == 2 ? _value3! : throw new InvalidOperationException("OneOf does not contain T3");
        public T4 AsT4 => _index == 3 ? _value4! : throw new InvalidOperationException("OneOf does not contain T4");

        public TResult Match<TResult>(Func<T1, TResult> case1, Func<T2, TResult> case2, Func<T3, TResult> case3, Func<T4, TResult> case4)
        {
            if (case1 is null) throw new ArgumentNullException(nameof(case1));
            if (case2 is null) throw new ArgumentNullException(nameof(case2));
            if (case3 is null) throw new ArgumentNullException(nameof(case3));
            if (case4 is null) throw new ArgumentNullException(nameof(case4));
            return _index switch
            {
                0 => case1(_value1!),
                1 => case2(_value2!),
                2 => case3(_value3!),
                3 => case4(_value4!),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        public void Switch(Action<T1> case1, Action<T2> case2, Action<T3> case3, Action<T4> case4)
        {
            if (case1 is null) throw new ArgumentNullException(nameof(case1));
            if (case2 is null) throw new ArgumentNullException(nameof(case2));
            if (case3 is null) throw new ArgumentNullException(nameof(case3));
            if (case4 is null) throw new ArgumentNullException(nameof(case4));
            switch (_index)
            {
                case 0: case1(_value1!); break;
                case 1: case2(_value2!); break;
                case 2: case3(_value3!); break;
                case 3: case4(_value4!); break;
                default: throw new InvalidOperationException("Invalid OneOf state");
            }
        }

        public override string ToString() => _index switch
        {
            0 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}>(T1: {_value1})",
            1 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}>(T2: {_value2})",
            2 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}>(T3: {_value3})",
            3 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}>(T4: {_value4})",
            _ => "OneOf<Invalid>"
        };

        public override bool Equals(object? obj)
        {
            if (obj is not OneOfBase<T1, T2, T3, T4> other || _index != other._index) return false;
            return _index switch
            {
                0 => EqualityComparer<T1>.Default.Equals(_value1, other._value1),
                1 => EqualityComparer<T2>.Default.Equals(_value2, other._value2),
                2 => EqualityComparer<T3>.Default.Equals(_value3, other._value3),
                3 => EqualityComparer<T4>.Default.Equals(_value4, other._value4),
                _ => false
            };
        }

        public override int GetHashCode() => HashCode.Combine(_index, _value1, _value2, _value3, _value4);
    }

    /// <summary>
    /// Shared dispatch base for discriminated unions of five types.
    /// </summary>
    public abstract class OneOfBase<T1, T2, T3, T4, T5>
    {
        private protected readonly T1? _value1;
        private protected readonly T2? _value2;
        private protected readonly T3? _value3;
        private protected readonly T4? _value4;
        private protected readonly T5? _value5;
        private protected readonly byte _index;

        private protected OneOfBase(T1? value1, T2? value2, T3? value3, T4? value4, T5? value5, byte index)
        {
            _value1 = value1;
            _value2 = value2;
            _value3 = value3;
            _value4 = value4;
            _value5 = value5;
            _index  = index;
        }

        public bool IsT1 => _index == 0;
        public bool IsT2 => _index == 1;
        public bool IsT3 => _index == 2;
        public bool IsT4 => _index == 3;
        public bool IsT5 => _index == 4;

        public T1 AsT1 => _index == 0 ? _value1! : throw new InvalidOperationException("OneOf does not contain T1");
        public T2 AsT2 => _index == 1 ? _value2! : throw new InvalidOperationException("OneOf does not contain T2");
        public T3 AsT3 => _index == 2 ? _value3! : throw new InvalidOperationException("OneOf does not contain T3");
        public T4 AsT4 => _index == 3 ? _value4! : throw new InvalidOperationException("OneOf does not contain T4");
        public T5 AsT5 => _index == 4 ? _value5! : throw new InvalidOperationException("OneOf does not contain T5");

        public TResult Match<TResult>(Func<T1, TResult> case1, Func<T2, TResult> case2, Func<T3, TResult> case3, Func<T4, TResult> case4, Func<T5, TResult> case5)
        {
            if (case1 is null) throw new ArgumentNullException(nameof(case1));
            if (case2 is null) throw new ArgumentNullException(nameof(case2));
            if (case3 is null) throw new ArgumentNullException(nameof(case3));
            if (case4 is null) throw new ArgumentNullException(nameof(case4));
            if (case5 is null) throw new ArgumentNullException(nameof(case5));
            return _index switch
            {
                0 => case1(_value1!),
                1 => case2(_value2!),
                2 => case3(_value3!),
                3 => case4(_value4!),
                4 => case5(_value5!),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        public void Switch(Action<T1> case1, Action<T2> case2, Action<T3> case3, Action<T4> case4, Action<T5> case5)
        {
            if (case1 is null) throw new ArgumentNullException(nameof(case1));
            if (case2 is null) throw new ArgumentNullException(nameof(case2));
            if (case3 is null) throw new ArgumentNullException(nameof(case3));
            if (case4 is null) throw new ArgumentNullException(nameof(case4));
            if (case5 is null) throw new ArgumentNullException(nameof(case5));
            switch (_index)
            {
                case 0: case1(_value1!); break;
                case 1: case2(_value2!); break;
                case 2: case3(_value3!); break;
                case 3: case4(_value4!); break;
                case 4: case5(_value5!); break;
                default: throw new InvalidOperationException("Invalid OneOf state");
            }
        }

        public override string ToString() => _index switch
        {
            0 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}>(T1: {_value1})",
            1 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}>(T2: {_value2})",
            2 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}>(T3: {_value3})",
            3 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}>(T4: {_value4})",
            4 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}>(T5: {_value5})",
            _ => "OneOf<Invalid>"
        };

        public override bool Equals(object? obj)
        {
            if (obj is not OneOfBase<T1, T2, T3, T4, T5> other || _index != other._index) return false;
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

        public override int GetHashCode() => HashCode.Combine(_index, _value1, _value2, _value3, _value4, _value5);
    }

    /// <summary>
    /// Shared dispatch base for discriminated unions of six types.
    /// </summary>
    public abstract class OneOfBase<T1, T2, T3, T4, T5, T6>
    {
        private protected readonly T1? _value1;
        private protected readonly T2? _value2;
        private protected readonly T3? _value3;
        private protected readonly T4? _value4;
        private protected readonly T5? _value5;
        private protected readonly T6? _value6;
        private protected readonly byte _index;

        private protected OneOfBase(T1? value1, T2? value2, T3? value3, T4? value4, T5? value5, T6? value6, byte index)
        {
            _value1 = value1;
            _value2 = value2;
            _value3 = value3;
            _value4 = value4;
            _value5 = value5;
            _value6 = value6;
            _index  = index;
        }

        public bool IsT1 => _index == 0;
        public bool IsT2 => _index == 1;
        public bool IsT3 => _index == 2;
        public bool IsT4 => _index == 3;
        public bool IsT5 => _index == 4;
        public bool IsT6 => _index == 5;

        public T1 AsT1 => _index == 0 ? _value1! : throw new InvalidOperationException("OneOf does not contain T1");
        public T2 AsT2 => _index == 1 ? _value2! : throw new InvalidOperationException("OneOf does not contain T2");
        public T3 AsT3 => _index == 2 ? _value3! : throw new InvalidOperationException("OneOf does not contain T3");
        public T4 AsT4 => _index == 3 ? _value4! : throw new InvalidOperationException("OneOf does not contain T4");
        public T5 AsT5 => _index == 4 ? _value5! : throw new InvalidOperationException("OneOf does not contain T5");
        public T6 AsT6 => _index == 5 ? _value6! : throw new InvalidOperationException("OneOf does not contain T6");

        public TResult Match<TResult>(Func<T1, TResult> case1, Func<T2, TResult> case2, Func<T3, TResult> case3, Func<T4, TResult> case4, Func<T5, TResult> case5, Func<T6, TResult> case6)
        {
            if (case1 is null) throw new ArgumentNullException(nameof(case1));
            if (case2 is null) throw new ArgumentNullException(nameof(case2));
            if (case3 is null) throw new ArgumentNullException(nameof(case3));
            if (case4 is null) throw new ArgumentNullException(nameof(case4));
            if (case5 is null) throw new ArgumentNullException(nameof(case5));
            if (case6 is null) throw new ArgumentNullException(nameof(case6));
            return _index switch
            {
                0 => case1(_value1!),
                1 => case2(_value2!),
                2 => case3(_value3!),
                3 => case4(_value4!),
                4 => case5(_value5!),
                5 => case6(_value6!),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        public void Switch(Action<T1> case1, Action<T2> case2, Action<T3> case3, Action<T4> case4, Action<T5> case5, Action<T6> case6)
        {
            if (case1 is null) throw new ArgumentNullException(nameof(case1));
            if (case2 is null) throw new ArgumentNullException(nameof(case2));
            if (case3 is null) throw new ArgumentNullException(nameof(case3));
            if (case4 is null) throw new ArgumentNullException(nameof(case4));
            if (case5 is null) throw new ArgumentNullException(nameof(case5));
            if (case6 is null) throw new ArgumentNullException(nameof(case6));
            switch (_index)
            {
                case 0: case1(_value1!); break;
                case 1: case2(_value2!); break;
                case 2: case3(_value3!); break;
                case 3: case4(_value4!); break;
                case 4: case5(_value5!); break;
                case 5: case6(_value6!); break;
                default: throw new InvalidOperationException("Invalid OneOf state");
            }
        }

        public override string ToString() => _index switch
        {
            0 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}, {typeof(T6).Name}>(T1: {_value1})",
            1 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}, {typeof(T6).Name}>(T2: {_value2})",
            2 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}, {typeof(T6).Name}>(T3: {_value3})",
            3 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}, {typeof(T6).Name}>(T4: {_value4})",
            4 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}, {typeof(T6).Name}>(T5: {_value5})",
            5 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}, {typeof(T6).Name}>(T6: {_value6})",
            _ => "OneOf<Invalid>"
        };

        public override bool Equals(object? obj)
        {
            if (obj is not OneOfBase<T1, T2, T3, T4, T5, T6> other || _index != other._index) return false;
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

        public override int GetHashCode() => HashCode.Combine(_index, _value1, _value2, _value3, _value4, _value5, _value6);
    }

    /// <summary>
    /// Shared dispatch base for discriminated unions of seven types.
    /// </summary>
    public abstract class OneOfBase<T1, T2, T3, T4, T5, T6, T7>
    {
        private protected readonly T1? _value1;
        private protected readonly T2? _value2;
        private protected readonly T3? _value3;
        private protected readonly T4? _value4;
        private protected readonly T5? _value5;
        private protected readonly T6? _value6;
        private protected readonly T7? _value7;
        private protected readonly byte _index;

        private protected OneOfBase(T1? value1, T2? value2, T3? value3, T4? value4, T5? value5, T6? value6, T7? value7, byte index)
        {
            _value1 = value1;
            _value2 = value2;
            _value3 = value3;
            _value4 = value4;
            _value5 = value5;
            _value6 = value6;
            _value7 = value7;
            _index  = index;
        }

        public bool IsT1 => _index == 0;
        public bool IsT2 => _index == 1;
        public bool IsT3 => _index == 2;
        public bool IsT4 => _index == 3;
        public bool IsT5 => _index == 4;
        public bool IsT6 => _index == 5;
        public bool IsT7 => _index == 6;

        public T1 AsT1 => _index == 0 ? _value1! : throw new InvalidOperationException("OneOf does not contain T1");
        public T2 AsT2 => _index == 1 ? _value2! : throw new InvalidOperationException("OneOf does not contain T2");
        public T3 AsT3 => _index == 2 ? _value3! : throw new InvalidOperationException("OneOf does not contain T3");
        public T4 AsT4 => _index == 3 ? _value4! : throw new InvalidOperationException("OneOf does not contain T4");
        public T5 AsT5 => _index == 4 ? _value5! : throw new InvalidOperationException("OneOf does not contain T5");
        public T6 AsT6 => _index == 5 ? _value6! : throw new InvalidOperationException("OneOf does not contain T6");
        public T7 AsT7 => _index == 6 ? _value7! : throw new InvalidOperationException("OneOf does not contain T7");

        public TResult Match<TResult>(Func<T1, TResult> case1, Func<T2, TResult> case2, Func<T3, TResult> case3, Func<T4, TResult> case4, Func<T5, TResult> case5, Func<T6, TResult> case6, Func<T7, TResult> case7)
        {
            if (case1 is null) throw new ArgumentNullException(nameof(case1));
            if (case2 is null) throw new ArgumentNullException(nameof(case2));
            if (case3 is null) throw new ArgumentNullException(nameof(case3));
            if (case4 is null) throw new ArgumentNullException(nameof(case4));
            if (case5 is null) throw new ArgumentNullException(nameof(case5));
            if (case6 is null) throw new ArgumentNullException(nameof(case6));
            if (case7 is null) throw new ArgumentNullException(nameof(case7));
            return _index switch
            {
                0 => case1(_value1!),
                1 => case2(_value2!),
                2 => case3(_value3!),
                3 => case4(_value4!),
                4 => case5(_value5!),
                5 => case6(_value6!),
                6 => case7(_value7!),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        public void Switch(Action<T1> case1, Action<T2> case2, Action<T3> case3, Action<T4> case4, Action<T5> case5, Action<T6> case6, Action<T7> case7)
        {
            if (case1 is null) throw new ArgumentNullException(nameof(case1));
            if (case2 is null) throw new ArgumentNullException(nameof(case2));
            if (case3 is null) throw new ArgumentNullException(nameof(case3));
            if (case4 is null) throw new ArgumentNullException(nameof(case4));
            if (case5 is null) throw new ArgumentNullException(nameof(case5));
            if (case6 is null) throw new ArgumentNullException(nameof(case6));
            if (case7 is null) throw new ArgumentNullException(nameof(case7));
            switch (_index)
            {
                case 0: case1(_value1!); break;
                case 1: case2(_value2!); break;
                case 2: case3(_value3!); break;
                case 3: case4(_value4!); break;
                case 4: case5(_value5!); break;
                case 5: case6(_value6!); break;
                case 6: case7(_value7!); break;
                default: throw new InvalidOperationException("Invalid OneOf state");
            }
        }

        public override string ToString() => _index switch
        {
            0 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}, {typeof(T6).Name}, {typeof(T7).Name}>(T1: {_value1})",
            1 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}, {typeof(T6).Name}, {typeof(T7).Name}>(T2: {_value2})",
            2 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}, {typeof(T6).Name}, {typeof(T7).Name}>(T3: {_value3})",
            3 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}, {typeof(T6).Name}, {typeof(T7).Name}>(T4: {_value4})",
            4 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}, {typeof(T6).Name}, {typeof(T7).Name}>(T5: {_value5})",
            5 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}, {typeof(T6).Name}, {typeof(T7).Name}>(T6: {_value6})",
            6 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}, {typeof(T6).Name}, {typeof(T7).Name}>(T7: {_value7})",
            _ => "OneOf<Invalid>"
        };

        public override bool Equals(object? obj)
        {
            if (obj is not OneOfBase<T1, T2, T3, T4, T5, T6, T7> other || _index != other._index) return false;
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

        public override int GetHashCode() => HashCode.Combine(_index, _value1, _value2, _value3, _value4, _value5, _value6, _value7);
    }

    /// <summary>
    /// Shared dispatch base for discriminated unions of eight types.
    /// </summary>
    public abstract class OneOfBase<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        private protected readonly T1? _value1;
        private protected readonly T2? _value2;
        private protected readonly T3? _value3;
        private protected readonly T4? _value4;
        private protected readonly T5? _value5;
        private protected readonly T6? _value6;
        private protected readonly T7? _value7;
        private protected readonly T8? _value8;
        private protected readonly byte _index;

        private protected OneOfBase(T1? value1, T2? value2, T3? value3, T4? value4, T5? value5, T6? value6, T7? value7, T8? value8, byte index)
        {
            _value1 = value1;
            _value2 = value2;
            _value3 = value3;
            _value4 = value4;
            _value5 = value5;
            _value6 = value6;
            _value7 = value7;
            _value8 = value8;
            _index  = index;
        }

        public bool IsT1 => _index == 0;
        public bool IsT2 => _index == 1;
        public bool IsT3 => _index == 2;
        public bool IsT4 => _index == 3;
        public bool IsT5 => _index == 4;
        public bool IsT6 => _index == 5;
        public bool IsT7 => _index == 6;
        public bool IsT8 => _index == 7;

        public T1 AsT1 => _index == 0 ? _value1! : throw new InvalidOperationException("OneOf does not contain T1");
        public T2 AsT2 => _index == 1 ? _value2! : throw new InvalidOperationException("OneOf does not contain T2");
        public T3 AsT3 => _index == 2 ? _value3! : throw new InvalidOperationException("OneOf does not contain T3");
        public T4 AsT4 => _index == 3 ? _value4! : throw new InvalidOperationException("OneOf does not contain T4");
        public T5 AsT5 => _index == 4 ? _value5! : throw new InvalidOperationException("OneOf does not contain T5");
        public T6 AsT6 => _index == 5 ? _value6! : throw new InvalidOperationException("OneOf does not contain T6");
        public T7 AsT7 => _index == 6 ? _value7! : throw new InvalidOperationException("OneOf does not contain T7");
        public T8 AsT8 => _index == 7 ? _value8! : throw new InvalidOperationException("OneOf does not contain T8");

        public TResult Match<TResult>(Func<T1, TResult> case1, Func<T2, TResult> case2, Func<T3, TResult> case3, Func<T4, TResult> case4, Func<T5, TResult> case5, Func<T6, TResult> case6, Func<T7, TResult> case7, Func<T8, TResult> case8)
        {
            if (case1 is null) throw new ArgumentNullException(nameof(case1));
            if (case2 is null) throw new ArgumentNullException(nameof(case2));
            if (case3 is null) throw new ArgumentNullException(nameof(case3));
            if (case4 is null) throw new ArgumentNullException(nameof(case4));
            if (case5 is null) throw new ArgumentNullException(nameof(case5));
            if (case6 is null) throw new ArgumentNullException(nameof(case6));
            if (case7 is null) throw new ArgumentNullException(nameof(case7));
            if (case8 is null) throw new ArgumentNullException(nameof(case8));
            return _index switch
            {
                0 => case1(_value1!),
                1 => case2(_value2!),
                2 => case3(_value3!),
                3 => case4(_value4!),
                4 => case5(_value5!),
                5 => case6(_value6!),
                6 => case7(_value7!),
                7 => case8(_value8!),
                _ => throw new InvalidOperationException("Invalid OneOf state")
            };
        }

        public void Switch(Action<T1> case1, Action<T2> case2, Action<T3> case3, Action<T4> case4, Action<T5> case5, Action<T6> case6, Action<T7> case7, Action<T8> case8)
        {
            if (case1 is null) throw new ArgumentNullException(nameof(case1));
            if (case2 is null) throw new ArgumentNullException(nameof(case2));
            if (case3 is null) throw new ArgumentNullException(nameof(case3));
            if (case4 is null) throw new ArgumentNullException(nameof(case4));
            if (case5 is null) throw new ArgumentNullException(nameof(case5));
            if (case6 is null) throw new ArgumentNullException(nameof(case6));
            if (case7 is null) throw new ArgumentNullException(nameof(case7));
            if (case8 is null) throw new ArgumentNullException(nameof(case8));
            switch (_index)
            {
                case 0: case1(_value1!); break;
                case 1: case2(_value2!); break;
                case 2: case3(_value3!); break;
                case 3: case4(_value4!); break;
                case 4: case5(_value5!); break;
                case 5: case6(_value6!); break;
                case 6: case7(_value7!); break;
                case 7: case8(_value8!); break;
                default: throw new InvalidOperationException("Invalid OneOf state");
            }
        }

        public override string ToString() => _index switch
        {
            0 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}, {typeof(T6).Name}, {typeof(T7).Name}, {typeof(T8).Name}>(T1: {_value1})",
            1 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}, {typeof(T6).Name}, {typeof(T7).Name}, {typeof(T8).Name}>(T2: {_value2})",
            2 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}, {typeof(T6).Name}, {typeof(T7).Name}, {typeof(T8).Name}>(T3: {_value3})",
            3 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}, {typeof(T6).Name}, {typeof(T7).Name}, {typeof(T8).Name}>(T4: {_value4})",
            4 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}, {typeof(T6).Name}, {typeof(T7).Name}, {typeof(T8).Name}>(T5: {_value5})",
            5 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}, {typeof(T6).Name}, {typeof(T7).Name}, {typeof(T8).Name}>(T6: {_value6})",
            6 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}, {typeof(T6).Name}, {typeof(T7).Name}, {typeof(T8).Name}>(T7: {_value7})",
            7 => $"OneOf<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name}, {typeof(T5).Name}, {typeof(T6).Name}, {typeof(T7).Name}, {typeof(T8).Name}>(T8: {_value8})",
            _ => "OneOf<Invalid>"
        };

        public override bool Equals(object? obj)
        {
            if (obj is not OneOfBase<T1, T2, T3, T4, T5, T6, T7, T8> other || _index != other._index) return false;
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

        public override int GetHashCode()
        {
            // HashCode.Combine supports up to 8 arguments
            return HashCode.Combine(_index, _value1, _value2, _value3, _value4, _value5, _value6,
                HashCode.Combine(_value7, _value8));
        }
    }
}
