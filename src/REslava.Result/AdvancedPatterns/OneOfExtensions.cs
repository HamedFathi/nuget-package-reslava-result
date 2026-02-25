using System;

namespace REslava.Result.AdvancedPatterns
{
    /// <summary>
    /// Extension methods for OneOf types, providing conversion between 2-way through 6-way OneOf instances.
    /// </summary>
    /// <remarks>
    /// <para>
    /// These extension methods provide explicit, type-safe conversions across the full OneOf arity chain:
    /// OneOf&lt;T1,T2&gt; ↔ OneOf&lt;T1,T2,T3&gt; ↔ OneOf&lt;T1,T2,T3,T4&gt; ↔ OneOf&lt;T1,T2,T3,T4,T5&gt; ↔ OneOf&lt;T1,T2,T3,T4,T5,T6&gt;.
    /// All conversions are explicit to ensure developers make conscious decisions about how to handle
    /// the additional type.
    /// </para>
    /// <para>
    /// The design philosophy is explicit over implicit - conversions require developers to specify
    /// how to handle the removed type, preventing accidental data loss or unexpected behavior.
    /// </para>
    /// </remarks>
    public static class OneOfExtensions
    {
        /// <summary>
        /// Converts a 2-way OneOf to a 3-way OneOf by providing a default value for the third type.
        /// </summary>
        /// <typeparam name="T1">The first type.</typeparam>
        /// <typeparam name="T2">The second type.</typeparam>
        /// <typeparam name="T3">The third type.</typeparam>
        /// <param name="oneOf">The 2-way OneOf to convert.</param>
        /// <param name="defaultValue">The default value to use if the 3-way OneOf needs to represent T3.</param>
        /// <returns>A 3-way OneOf containing the original value or the default for T3.</returns>
        /// <remarks>
        /// This conversion is safe and preserves the original value. The default value is only used
        /// when the 3-way OneOf needs to represent a T3 value in other operations.
        /// </remarks>
        /// <example>
        /// <code>
        /// OneOf&lt;Error, Success&gt; twoWay = new Success("Data processed");
        /// OneOf&lt;Error, Success, Warning&gt; threeWay = twoWay.ToThreeWay&lt;Warning&gt;(new Warning("No warnings"));
        /// </code>
        /// </example>
        public static OneOf<T1, T2, T3> ToThreeWay<T1, T2, T3>(this OneOf<T1, T2> oneOf, T3 defaultValue)
        {
            return oneOf.Match(
                case1: t1 => OneOf<T1, T2, T3>.FromT1(t1),
                case2: t2 => OneOf<T1, T2, T3>.FromT2(t2)
            );
        }

        /// <summary>
        /// Converts a 3-way OneOf to a 2-way OneOf by filtering out the third type.
        /// </summary>
        /// <typeparam name="T1">The first type.</typeparam>
        /// <typeparam name="T2">The second type.</typeparam>
        /// <typeparam name="T3">The third type to filter out.</typeparam>
        /// <param name="oneOf">The 3-way OneOf to convert.</param>
        /// <returns>A 2-way OneOf if the original contains T1 or T2, null if it contains T3.</returns>
        /// <remarks>
        /// This conversion filters out the T3 case. If the 3-way OneOf contains T3, the result is null
        /// to indicate that the value couldn't be represented in the 2-way type system.
        /// This explicit null return makes it clear that data might be lost in the conversion.
        /// </remarks>
        /// <example>
        /// <code>
        /// OneOf&lt;Error, Success, Warning&gt; threeWay = new Success("Data processed");
        /// OneOf&lt;Error, Success&gt;? twoWay = threeWay.ToTwoWay&lt;Error, Success, Warning&gt;();
        /// if (twoWay.HasValue) { /* process T1/T2 case */ }
        /// </code>
        /// </example>
        public static OneOf<T1, T2>? ToTwoWay<T1, T2, T3>(this OneOf<T1, T2, T3> oneOf)
        {
            return oneOf.Match(
                case1: t1 => OneOf<T1, T2>.FromT1(t1),
                case2: t2 => OneOf<T1, T2>.FromT2(t2),
                case3: _ => (OneOf<T1, T2>?)null // T3 case filtered out
            );
        }

        /// <summary>
        /// Converts a 3-way OneOf to a 2-way OneOf by mapping the third type to one of the first two.
        /// </summary>
        /// <typeparam name="T1">The first type.</typeparam>
        /// <typeparam name="T2">The second type.</typeparam>
        /// <typeparam name="T3">The third type to map.</typeparam>
        /// <param name="oneOf">The 3-way OneOf to convert.</param>
        /// <param name="t3ToT1">Function to convert T3 to T1 when needed.</param>
        /// <param name="t3ToT2">Function to convert T3 to T2 when needed.</param>
        /// <returns>A 2-way OneOf with T3 mapped to either T1 or T2.</returns>
        /// <exception cref="ArgumentNullException">Thrown when both mapping functions are null.</exception>
        /// <remarks>
        /// This conversion allows you to handle the T3 case by mapping it to either T1 or T2.
        /// You must provide at least one mapping function to handle the T3 case.
        /// If both functions are provided, t3ToT1 takes precedence.
        /// </remarks>
        /// <example>
        /// <code>
        /// OneOf&lt;Error, Success, Warning&gt; threeWay = new Warning("Deprecated API");
        /// OneOf&lt;Error, Success&gt; twoWay = threeWay.ToTwoWay(
        ///     t3ToT1: warning => new Error($"Warning: {warning.Message}"),
        ///     t3ToT2: warning => new Success($"Warning handled: {warning.Message}")
        /// );
        /// </code>
        /// </example>
        public static OneOf<T1, T2> ToTwoWay<T1, T2, T3>(
            this OneOf<T1, T2, T3> oneOf,
            Func<T3, T1>? t3ToT1 = null,
            Func<T3, T2>? t3ToT2 = null)
        {
            if (t3ToT1 == null && t3ToT2 == null)
                throw new ArgumentNullException("At least one mapping function must be provided");

            return oneOf.Match(
                case1: t1 => OneOf<T1, T2>.FromT1(t1),
                case2: t2 => OneOf<T1, T2>.FromT2(t2),
                case3: t3 => t3ToT1 != null 
                    ? OneOf<T1, T2>.FromT1(t3ToT1(t3))
                    : OneOf<T1, T2>.FromT2(t3ToT2!(t3))
            );
        }

        /// <summary>
        /// Converts a 3-way OneOf to a 2-way OneOf by providing a fallback value for the third type.
        /// </summary>
        /// <typeparam name="T1">The first type.</typeparam>
        /// <typeparam name="T2">The second type.</typeparam>
        /// <typeparam name="T3">The third type.</typeparam>
        /// <param name="oneOf">The 3-way OneOf to convert.</param>
        /// <param name="fallbackT1">Fallback T1 value when the OneOf contains T3.</param>
        /// <param name="fallbackT2">Fallback T2 value when the OneOf contains T3.</param>
        /// <returns>A 2-way OneOf with T3 replaced by the specified fallback.</returns>
        /// <exception cref="ArgumentException">Thrown when both fallback values are null.</exception>
        /// <remarks>
        /// This conversion allows you to handle the T3 case by replacing it with a fallback value.
        /// You must provide at least one fallback value. If both are provided, fallbackT1 takes precedence.
        /// </remarks>
        /// <example>
        /// <code>
        /// OneOf&lt;Error, Success, Warning&gt; threeWay = new Warning("Deprecated API");
        /// OneOf&lt;Error, Success&gt; twoWay = threeWay.ToTwoWayWithFallback(
        ///     fallbackT1: new Error("Warning occurred"),
        ///     fallbackT2: new Success("Warning handled")
        /// );
        /// </code>
        /// </example>
        public static OneOf<T1, T2> ToTwoWayWithFallback<T1, T2, T3>(
            this OneOf<T1, T2, T3> oneOf,
            T1? fallbackT1 = default,
            T2? fallbackT2 = default)
            where T1 : class
            where T2 : class
        {
            if (fallbackT1 == null && fallbackT2 == null)
                throw new ArgumentException("At least one fallback value must be provided");

            return oneOf.Match(
                case1: t1 => OneOf<T1, T2>.FromT1(t1),
                case2: t2 => OneOf<T1, T2>.FromT2(t2),
                case3: _ => fallbackT1 != null
                    ? OneOf<T1, T2>.FromT1(fallbackT1)
                    : OneOf<T1, T2>.FromT2(fallbackT2!)
            );
        }

        // ── 3 ↔ 4 ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Converts a 3-way OneOf to a 4-way OneOf by providing a default value for the fourth type.
        /// </summary>
        /// <param name="oneOf">The 3-way OneOf to convert.</param>
        /// <param name="defaultValue">Anchors the T4 type parameter; not used at runtime.</param>
        public static OneOf<T1, T2, T3, T4> ToFourWay<T1, T2, T3, T4>(
            this OneOf<T1, T2, T3> oneOf, T4 defaultValue)
        {
            return oneOf.Match(
                case1: t1 => OneOf<T1, T2, T3, T4>.FromT1(t1),
                case2: t2 => OneOf<T1, T2, T3, T4>.FromT2(t2),
                case3: t3 => OneOf<T1, T2, T3, T4>.FromT3(t3)
            );
        }

        /// <summary>
        /// Converts a 4-way OneOf to a 3-way OneOf, returning null when the value is T4.
        /// </summary>
        public static OneOf<T1, T2, T3>? ToThreeWay<T1, T2, T3, T4>(
            this OneOf<T1, T2, T3, T4> oneOf)
        {
            return oneOf.Match(
                case1: t1 => OneOf<T1, T2, T3>.FromT1(t1),
                case2: t2 => OneOf<T1, T2, T3>.FromT2(t2),
                case3: t3 => OneOf<T1, T2, T3>.FromT3(t3),
                case4: _ => (OneOf<T1, T2, T3>?)null
            );
        }

        /// <summary>
        /// Converts a 4-way OneOf to a 3-way OneOf by mapping the T4 case to one of the first three types.
        /// </summary>
        public static OneOf<T1, T2, T3> ToThreeWay<T1, T2, T3, T4>(
            this OneOf<T1, T2, T3, T4> oneOf,
            Func<T4, T1>? t4ToT1 = null,
            Func<T4, T2>? t4ToT2 = null,
            Func<T4, T3>? t4ToT3 = null)
        {
            if (t4ToT1 == null && t4ToT2 == null && t4ToT3 == null)
                throw new ArgumentNullException("At least one mapping function must be provided");

            return oneOf.Match(
                case1: t1 => OneOf<T1, T2, T3>.FromT1(t1),
                case2: t2 => OneOf<T1, T2, T3>.FromT2(t2),
                case3: t3 => OneOf<T1, T2, T3>.FromT3(t3),
                case4: t4 => t4ToT1 != null
                    ? OneOf<T1, T2, T3>.FromT1(t4ToT1(t4))
                    : t4ToT2 != null
                        ? OneOf<T1, T2, T3>.FromT2(t4ToT2(t4))
                        : OneOf<T1, T2, T3>.FromT3(t4ToT3!(t4))
            );
        }

        /// <summary>
        /// Converts a 4-way OneOf to a 3-way OneOf by replacing the T4 case with a fallback value.
        /// </summary>
        public static OneOf<T1, T2, T3> ToThreeWayWithFallback<T1, T2, T3, T4>(
            this OneOf<T1, T2, T3, T4> oneOf,
            T1? fallbackT1 = default,
            T2? fallbackT2 = default,
            T3? fallbackT3 = default)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            if (fallbackT1 == null && fallbackT2 == null && fallbackT3 == null)
                throw new ArgumentException("At least one fallback value must be provided");

            return oneOf.Match(
                case1: t1 => OneOf<T1, T2, T3>.FromT1(t1),
                case2: t2 => OneOf<T1, T2, T3>.FromT2(t2),
                case3: t3 => OneOf<T1, T2, T3>.FromT3(t3),
                case4: _ => fallbackT1 != null
                    ? OneOf<T1, T2, T3>.FromT1(fallbackT1)
                    : fallbackT2 != null
                        ? OneOf<T1, T2, T3>.FromT2(fallbackT2)
                        : OneOf<T1, T2, T3>.FromT3(fallbackT3!)
            );
        }

        // ── 4 ↔ 5 ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Converts a 4-way OneOf to a 5-way OneOf by providing a default value for the fifth type.
        /// </summary>
        /// <param name="oneOf">The 4-way OneOf to convert.</param>
        /// <param name="defaultValue">Anchors the T5 type parameter; not used at runtime.</param>
        public static OneOf<T1, T2, T3, T4, T5> ToFiveWay<T1, T2, T3, T4, T5>(
            this OneOf<T1, T2, T3, T4> oneOf, T5 defaultValue)
        {
            return oneOf.Match(
                case1: t1 => OneOf<T1, T2, T3, T4, T5>.FromT1(t1),
                case2: t2 => OneOf<T1, T2, T3, T4, T5>.FromT2(t2),
                case3: t3 => OneOf<T1, T2, T3, T4, T5>.FromT3(t3),
                case4: t4 => OneOf<T1, T2, T3, T4, T5>.FromT4(t4)
            );
        }

        /// <summary>
        /// Converts a 5-way OneOf to a 4-way OneOf, returning null when the value is T5.
        /// </summary>
        public static OneOf<T1, T2, T3, T4>? ToFourWay<T1, T2, T3, T4, T5>(
            this OneOf<T1, T2, T3, T4, T5> oneOf)
        {
            return oneOf.Match(
                case1: t1 => OneOf<T1, T2, T3, T4>.FromT1(t1),
                case2: t2 => OneOf<T1, T2, T3, T4>.FromT2(t2),
                case3: t3 => OneOf<T1, T2, T3, T4>.FromT3(t3),
                case4: t4 => OneOf<T1, T2, T3, T4>.FromT4(t4),
                case5: _ => (OneOf<T1, T2, T3, T4>?)null
            );
        }

        /// <summary>
        /// Converts a 5-way OneOf to a 4-way OneOf by mapping the T5 case to one of the first four types.
        /// </summary>
        public static OneOf<T1, T2, T3, T4> ToFourWay<T1, T2, T3, T4, T5>(
            this OneOf<T1, T2, T3, T4, T5> oneOf,
            Func<T5, T1>? t5ToT1 = null,
            Func<T5, T2>? t5ToT2 = null,
            Func<T5, T3>? t5ToT3 = null,
            Func<T5, T4>? t5ToT4 = null)
        {
            if (t5ToT1 == null && t5ToT2 == null && t5ToT3 == null && t5ToT4 == null)
                throw new ArgumentNullException("At least one mapping function must be provided");

            return oneOf.Match(
                case1: t1 => OneOf<T1, T2, T3, T4>.FromT1(t1),
                case2: t2 => OneOf<T1, T2, T3, T4>.FromT2(t2),
                case3: t3 => OneOf<T1, T2, T3, T4>.FromT3(t3),
                case4: t4 => OneOf<T1, T2, T3, T4>.FromT4(t4),
                case5: t5 => t5ToT1 != null
                    ? OneOf<T1, T2, T3, T4>.FromT1(t5ToT1(t5))
                    : t5ToT2 != null
                        ? OneOf<T1, T2, T3, T4>.FromT2(t5ToT2(t5))
                        : t5ToT3 != null
                            ? OneOf<T1, T2, T3, T4>.FromT3(t5ToT3(t5))
                            : OneOf<T1, T2, T3, T4>.FromT4(t5ToT4!(t5))
            );
        }

        /// <summary>
        /// Converts a 5-way OneOf to a 4-way OneOf by replacing the T5 case with a fallback value.
        /// </summary>
        public static OneOf<T1, T2, T3, T4> ToFourWayWithFallback<T1, T2, T3, T4, T5>(
            this OneOf<T1, T2, T3, T4, T5> oneOf,
            T1? fallbackT1 = default,
            T2? fallbackT2 = default,
            T3? fallbackT3 = default,
            T4? fallbackT4 = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            if (fallbackT1 == null && fallbackT2 == null && fallbackT3 == null && fallbackT4 == null)
                throw new ArgumentException("At least one fallback value must be provided");

            return oneOf.Match(
                case1: t1 => OneOf<T1, T2, T3, T4>.FromT1(t1),
                case2: t2 => OneOf<T1, T2, T3, T4>.FromT2(t2),
                case3: t3 => OneOf<T1, T2, T3, T4>.FromT3(t3),
                case4: t4 => OneOf<T1, T2, T3, T4>.FromT4(t4),
                case5: _ => fallbackT1 != null
                    ? OneOf<T1, T2, T3, T4>.FromT1(fallbackT1)
                    : fallbackT2 != null
                        ? OneOf<T1, T2, T3, T4>.FromT2(fallbackT2)
                        : fallbackT3 != null
                            ? OneOf<T1, T2, T3, T4>.FromT3(fallbackT3)
                            : OneOf<T1, T2, T3, T4>.FromT4(fallbackT4!)
            );
        }

        // ── 5 ↔ 6 ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Converts a 5-way OneOf to a 6-way OneOf by providing a default value for the sixth type.
        /// </summary>
        /// <param name="oneOf">The 5-way OneOf to convert.</param>
        /// <param name="defaultValue">Anchors the T6 type parameter; not used at runtime.</param>
        public static OneOf<T1, T2, T3, T4, T5, T6> ToSixWay<T1, T2, T3, T4, T5, T6>(
            this OneOf<T1, T2, T3, T4, T5> oneOf, T6 defaultValue)
        {
            return oneOf.Match(
                case1: t1 => OneOf<T1, T2, T3, T4, T5, T6>.FromT1(t1),
                case2: t2 => OneOf<T1, T2, T3, T4, T5, T6>.FromT2(t2),
                case3: t3 => OneOf<T1, T2, T3, T4, T5, T6>.FromT3(t3),
                case4: t4 => OneOf<T1, T2, T3, T4, T5, T6>.FromT4(t4),
                case5: t5 => OneOf<T1, T2, T3, T4, T5, T6>.FromT5(t5)
            );
        }

        /// <summary>
        /// Converts a 6-way OneOf to a 5-way OneOf, returning null when the value is T6.
        /// </summary>
        public static OneOf<T1, T2, T3, T4, T5>? ToFiveWay<T1, T2, T3, T4, T5, T6>(
            this OneOf<T1, T2, T3, T4, T5, T6> oneOf)
        {
            return oneOf.Match(
                case1: t1 => OneOf<T1, T2, T3, T4, T5>.FromT1(t1),
                case2: t2 => OneOf<T1, T2, T3, T4, T5>.FromT2(t2),
                case3: t3 => OneOf<T1, T2, T3, T4, T5>.FromT3(t3),
                case4: t4 => OneOf<T1, T2, T3, T4, T5>.FromT4(t4),
                case5: t5 => OneOf<T1, T2, T3, T4, T5>.FromT5(t5),
                case6: _ => (OneOf<T1, T2, T3, T4, T5>?)null
            );
        }

        /// <summary>
        /// Converts a 6-way OneOf to a 5-way OneOf by mapping the T6 case to one of the first five types.
        /// </summary>
        public static OneOf<T1, T2, T3, T4, T5> ToFiveWay<T1, T2, T3, T4, T5, T6>(
            this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
            Func<T6, T1>? t6ToT1 = null,
            Func<T6, T2>? t6ToT2 = null,
            Func<T6, T3>? t6ToT3 = null,
            Func<T6, T4>? t6ToT4 = null,
            Func<T6, T5>? t6ToT5 = null)
        {
            if (t6ToT1 == null && t6ToT2 == null && t6ToT3 == null && t6ToT4 == null && t6ToT5 == null)
                throw new ArgumentNullException("At least one mapping function must be provided");

            return oneOf.Match(
                case1: t1 => OneOf<T1, T2, T3, T4, T5>.FromT1(t1),
                case2: t2 => OneOf<T1, T2, T3, T4, T5>.FromT2(t2),
                case3: t3 => OneOf<T1, T2, T3, T4, T5>.FromT3(t3),
                case4: t4 => OneOf<T1, T2, T3, T4, T5>.FromT4(t4),
                case5: t5 => OneOf<T1, T2, T3, T4, T5>.FromT5(t5),
                case6: t6 => t6ToT1 != null
                    ? OneOf<T1, T2, T3, T4, T5>.FromT1(t6ToT1(t6))
                    : t6ToT2 != null
                        ? OneOf<T1, T2, T3, T4, T5>.FromT2(t6ToT2(t6))
                        : t6ToT3 != null
                            ? OneOf<T1, T2, T3, T4, T5>.FromT3(t6ToT3(t6))
                            : t6ToT4 != null
                                ? OneOf<T1, T2, T3, T4, T5>.FromT4(t6ToT4(t6))
                                : OneOf<T1, T2, T3, T4, T5>.FromT5(t6ToT5!(t6))
            );
        }

        /// <summary>
        /// Converts a 6-way OneOf to a 5-way OneOf by replacing the T6 case with a fallback value.
        /// </summary>
        public static OneOf<T1, T2, T3, T4, T5> ToFiveWayWithFallback<T1, T2, T3, T4, T5, T6>(
            this OneOf<T1, T2, T3, T4, T5, T6> oneOf,
            T1? fallbackT1 = default,
            T2? fallbackT2 = default,
            T3? fallbackT3 = default,
            T4? fallbackT4 = default,
            T5? fallbackT5 = default)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            if (fallbackT1 == null && fallbackT2 == null && fallbackT3 == null && fallbackT4 == null && fallbackT5 == null)
                throw new ArgumentException("At least one fallback value must be provided");

            return oneOf.Match(
                case1: t1 => OneOf<T1, T2, T3, T4, T5>.FromT1(t1),
                case2: t2 => OneOf<T1, T2, T3, T4, T5>.FromT2(t2),
                case3: t3 => OneOf<T1, T2, T3, T4, T5>.FromT3(t3),
                case4: t4 => OneOf<T1, T2, T3, T4, T5>.FromT4(t4),
                case5: t5 => OneOf<T1, T2, T3, T4, T5>.FromT5(t5),
                case6: _ => fallbackT1 != null
                    ? OneOf<T1, T2, T3, T4, T5>.FromT1(fallbackT1)
                    : fallbackT2 != null
                        ? OneOf<T1, T2, T3, T4, T5>.FromT2(fallbackT2)
                        : fallbackT3 != null
                            ? OneOf<T1, T2, T3, T4, T5>.FromT3(fallbackT3)
                            : fallbackT4 != null
                                ? OneOf<T1, T2, T3, T4, T5>.FromT4(fallbackT4)
                                : OneOf<T1, T2, T3, T4, T5>.FromT5(fallbackT5!)
            );
        }
    }
}
