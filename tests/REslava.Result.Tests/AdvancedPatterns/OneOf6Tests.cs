using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REslava.Result.AdvancedPatterns;

namespace REslava.Result.Tests.AdvancedPatterns;

[TestClass]
public class OneOf6Tests
{
    // Types used: OneOf<TestError, string, int, double, bool, long>

    #region Construction Tests

    [TestMethod]
    public void OneOf6_FromT1_ShouldCreateT1Instance()
    {
        var error = new TestError("err");
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT1(error);

        Assert.IsTrue(oneOf.IsT1);
        Assert.IsFalse(oneOf.IsT2);
        Assert.IsFalse(oneOf.IsT3);
        Assert.IsFalse(oneOf.IsT4);
        Assert.IsFalse(oneOf.IsT5);
        Assert.IsFalse(oneOf.IsT6);
        Assert.AreEqual(error, oneOf.AsT1);
    }

    [TestMethod]
    public void OneOf6_FromT5_ShouldCreateT5Instance()
    {
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT5(true);

        Assert.IsFalse(oneOf.IsT1);
        Assert.IsFalse(oneOf.IsT2);
        Assert.IsFalse(oneOf.IsT3);
        Assert.IsFalse(oneOf.IsT4);
        Assert.IsTrue(oneOf.IsT5);
        Assert.IsFalse(oneOf.IsT6);
        Assert.AreEqual(true, oneOf.AsT5);
    }

    [TestMethod]
    public void OneOf6_FromT6_ShouldCreateT6Instance()
    {
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT6(999L);

        Assert.IsFalse(oneOf.IsT1);
        Assert.IsFalse(oneOf.IsT2);
        Assert.IsFalse(oneOf.IsT3);
        Assert.IsFalse(oneOf.IsT4);
        Assert.IsFalse(oneOf.IsT5);
        Assert.IsTrue(oneOf.IsT6);
        Assert.AreEqual(999L, oneOf.AsT6);
    }

    [TestMethod]
    public void OneOf6_FromT2_ShouldCreateT2Instance()
    {
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT2("world");

        Assert.IsTrue(oneOf.IsT2);
        Assert.AreEqual("world", oneOf.AsT2);
    }

    [TestMethod]
    public void OneOf6_FromT3_ShouldCreateT3Instance()
    {
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT3(7);

        Assert.IsTrue(oneOf.IsT3);
        Assert.AreEqual(7, oneOf.AsT3);
    }

    [TestMethod]
    public void OneOf6_FromT4_ShouldCreateT4Instance()
    {
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT4(1.5);

        Assert.IsTrue(oneOf.IsT4);
        Assert.AreEqual(1.5, oneOf.AsT4);
    }

    [TestMethod]
    public void OneOf6_ImplicitConversionT5_ShouldCreateT5Instance()
    {
        OneOf<TestError, string, int, double, bool, long> oneOf = false;

        Assert.IsTrue(oneOf.IsT5);
        Assert.AreEqual(false, oneOf.AsT5);
    }

    [TestMethod]
    public void OneOf6_ImplicitConversionT6_ShouldCreateT6Instance()
    {
        OneOf<TestError, string, int, double, bool, long> oneOf = 42L;

        Assert.IsTrue(oneOf.IsT6);
        Assert.AreEqual(42L, oneOf.AsT6);
    }

    #endregion

    #region Property Access — Throw Tests

    [TestMethod]
    public void OneOf6_AsT6_WhenNotT6_ShouldThrow()
    {
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT1(new TestError("x"));

        try { var _ = oneOf.AsT6; Assert.Fail("Expected InvalidOperationException"); }
        catch (InvalidOperationException) { }
    }

    [TestMethod]
    public void OneOf6_AsT5_WhenT6_ShouldThrow()
    {
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT6(1L);

        try { var _ = oneOf.AsT5; Assert.Fail("Expected InvalidOperationException"); }
        catch (InvalidOperationException) { }
    }

    [TestMethod]
    public void OneOf6_AsT1_WhenT6_ShouldThrow()
    {
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT6(99L);

        try { var _ = oneOf.AsT1; Assert.Fail("Expected InvalidOperationException"); }
        catch (InvalidOperationException) { }
    }

    #endregion

    #region Match Tests

    [TestMethod]
    public void OneOf6_Match_WithT5_ShouldExecuteCase5()
    {
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT5(true);

        var result = oneOf.Match(
            case1: e => "error",
            case2: s => "string",
            case3: i => "int",
            case4: d => "double",
            case5: b => $"bool:{b}",
            case6: l => "long"
        );

        Assert.AreEqual("bool:True", result);
    }

    [TestMethod]
    public void OneOf6_Match_WithT6_ShouldExecuteCase6()
    {
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT6(77L);

        var result = oneOf.Match(
            case1: e => "error",
            case2: s => "string",
            case3: i => "int",
            case4: d => "double",
            case5: b => "bool",
            case6: l => $"long:{l}"
        );

        Assert.AreEqual("long:77", result);
    }

    [TestMethod]
    public void OneOf6_Match_WithT1_ShouldExecuteCase1()
    {
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT1(new TestError("fail"));

        var result = oneOf.Match(
            case1: e => $"error:{e.Message}",
            case2: s => "string",
            case3: i => "int",
            case4: d => "double",
            case5: b => "bool",
            case6: l => "long"
        );

        Assert.AreEqual("error:fail", result);
    }

    [TestMethod]
    public void OneOf6_Match_NullCase_ShouldThrow()
    {
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT6(1L);

        try
        {
            oneOf.Match(
                case1: e => "e",
                case2: s => "s",
                case3: i => "i",
                case4: d => "d",
                case5: b => "b",
                case6: null!
            );
            Assert.Fail("Expected ArgumentNullException");
        }
        catch (ArgumentNullException) { }
    }

    #endregion

    #region Switch Tests

    [TestMethod]
    public void OneOf6_Switch_WithT5_ShouldExecuteCase5()
    {
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT5(true);
        var executed = 0;

        oneOf.Switch(
            case1: _ => executed = 1,
            case2: _ => executed = 2,
            case3: _ => executed = 3,
            case4: _ => executed = 4,
            case5: _ => executed = 5,
            case6: _ => executed = 6
        );

        Assert.AreEqual(5, executed);
    }

    [TestMethod]
    public void OneOf6_Switch_WithT6_ShouldExecuteCase6()
    {
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT6(0L);
        var executed = 0;

        oneOf.Switch(
            case1: _ => executed = 1,
            case2: _ => executed = 2,
            case3: _ => executed = 3,
            case4: _ => executed = 4,
            case5: _ => executed = 5,
            case6: _ => executed = 6
        );

        Assert.AreEqual(6, executed);
    }

    #endregion

    #region Map Tests

    [TestMethod]
    public void OneOf6_MapT2_WithT2_ShouldTransformT2()
    {
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT2("abc");

        var result = oneOf.MapT2(s => s.ToUpper());

        Assert.IsTrue(result.IsT2);
        Assert.AreEqual("ABC", result.AsT2);
    }

    [TestMethod]
    public void OneOf6_MapT2_WithT5_ShouldPropagateT5()
    {
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT5(false);

        var result = oneOf.MapT2(s => s.ToUpper());

        Assert.IsTrue(result.IsT5);
        Assert.AreEqual(false, result.AsT5);
    }

    [TestMethod]
    public void OneOf6_MapT2_WithT6_ShouldPropagateT6()
    {
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT6(100L);

        var result = oneOf.MapT2(s => s.ToUpper());

        Assert.IsTrue(result.IsT6);
        Assert.AreEqual(100L, result.AsT6);
    }

    [TestMethod]
    public void OneOf6_MapT5_WithT5_ShouldTransformT5()
    {
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT5(true);

        var result = oneOf.MapT5(b => !b);

        Assert.IsTrue(result.IsT5);
        Assert.AreEqual(false, result.AsT5);
    }

    [TestMethod]
    public void OneOf6_MapT5_WithT6_ShouldPropagateT6()
    {
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT6(50L);

        var result = oneOf.MapT5(b => !b);

        Assert.IsTrue(result.IsT6);
        Assert.AreEqual(50L, result.AsT6);
    }

    [TestMethod]
    public void OneOf6_MapT5_WithT1_ShouldPropagateT1()
    {
        var error = new TestError("e");
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT1(error);

        var result = oneOf.MapT5(b => !b);

        Assert.IsTrue(result.IsT1);
        Assert.AreEqual(error, result.AsT1);
    }

    [TestMethod]
    public void OneOf6_MapT4_WithT4_ShouldTransformT4()
    {
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT4(4.0);

        var result = oneOf.MapT4(d => d * 2.5);

        Assert.IsTrue(result.IsT4);
        Assert.AreEqual(10.0, result.AsT4);
    }

    #endregion

    #region Bind Tests

    [TestMethod]
    public void OneOf6_BindT2_WithT2_ShouldApplyBinder()
    {
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT2("hello");

        var result = oneOf.BindT2(s => OneOf<TestError, int, int, double, bool, long>.FromT2(s.Length));

        Assert.IsTrue(result.IsT2);
        Assert.AreEqual(5, result.AsT2);
    }

    [TestMethod]
    public void OneOf6_BindT2_WithT6_ShouldPropagateT6()
    {
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT6(77L);

        var result = oneOf.BindT2(s => OneOf<TestError, int, int, double, bool, long>.FromT2(s.Length));

        Assert.IsTrue(result.IsT6);
        Assert.AreEqual(77L, result.AsT6);
    }

    [TestMethod]
    public void OneOf6_BindT5_WithT5_ShouldApplyBinder()
    {
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT5(true);

        var result = oneOf.BindT5(b => OneOf<TestError, string, int, double, string, long>.FromT5(b.ToString()));

        Assert.IsTrue(result.IsT5);
        Assert.AreEqual("True", result.AsT5);
    }

    [TestMethod]
    public void OneOf6_BindT5_WithT1_ShouldPropagateT1()
    {
        var error = new TestError("x");
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT1(error);

        var result = oneOf.BindT5(b => OneOf<TestError, string, int, double, string, long>.FromT5(b.ToString()));

        Assert.IsTrue(result.IsT1);
        Assert.AreEqual(error, result.AsT1);
    }

    #endregion

    #region Equality and HashCode Tests

    [TestMethod]
    public void OneOf6_Equals_SameT6Value_ShouldBeEqual()
    {
        var a = OneOf<TestError, string, int, double, bool, long>.FromT6(42L);
        var b = OneOf<TestError, string, int, double, bool, long>.FromT6(42L);

        Assert.IsTrue(a.Equals(b));
        Assert.IsTrue(a == b);
        Assert.IsFalse(a != b);
    }

    [TestMethod]
    public void OneOf6_Equals_DifferentTypes_ShouldNotBeEqual()
    {
        var a = OneOf<TestError, string, int, double, bool, long>.FromT5(true);
        var b = OneOf<TestError, string, int, double, bool, long>.FromT6(1L);

        Assert.IsFalse(a.Equals(b));
        Assert.IsTrue(a != b);
    }

    [TestMethod]
    public void OneOf6_GetHashCode_EqualT6Values_ShouldMatch()
    {
        var a = OneOf<TestError, string, int, double, bool, long>.FromT6(100L);
        var b = OneOf<TestError, string, int, double, bool, long>.FromT6(100L);

        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    #endregion

    #region ToString Tests

    [TestMethod]
    public void OneOf6_ToString_WithT5_ShouldContainT5Label()
    {
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT5(true);
        var result = oneOf.ToString();

        Assert.IsTrue(result.Contains("OneOf<"), result);
        Assert.IsTrue(result.Contains("T5:"), result);
        Assert.IsTrue(result.Contains("Boolean"), result);
    }

    [TestMethod]
    public void OneOf6_ToString_WithT6_ShouldContainT6Label()
    {
        var oneOf = OneOf<TestError, string, int, double, bool, long>.FromT6(123L);
        var result = oneOf.ToString();

        Assert.IsTrue(result.Contains("OneOf<"), result);
        Assert.IsTrue(result.Contains("T6:"), result);
        Assert.IsTrue(result.Contains("Int64"), result);
    }

    #endregion

    #region Helper Classes

    private class TestError : IEquatable<TestError>
    {
        public string Message { get; }
        public TestError(string message) => Message = message;
        public bool Equals(TestError? other) => other != null && Message == other.Message;
        public override bool Equals(object? obj) => obj is TestError other && Equals(other);
        public override int GetHashCode() => Message.GetHashCode();
        public override string ToString() => $"TestError: {Message}";
    }

    #endregion
}
