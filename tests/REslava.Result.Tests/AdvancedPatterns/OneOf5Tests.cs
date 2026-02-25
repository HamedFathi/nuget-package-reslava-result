using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REslava.Result.AdvancedPatterns;

namespace REslava.Result.Tests.AdvancedPatterns;

[TestClass]
public class OneOf5Tests
{
    // Types used: OneOf<TestError, string, int, double, bool>

    #region Construction Tests

    [TestMethod]
    public void OneOf5_FromT1_ShouldCreateT1Instance()
    {
        var error = new TestError("err");
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT1(error);

        Assert.IsTrue(oneOf.IsT1);
        Assert.IsFalse(oneOf.IsT2);
        Assert.IsFalse(oneOf.IsT3);
        Assert.IsFalse(oneOf.IsT4);
        Assert.IsFalse(oneOf.IsT5);
        Assert.AreEqual(error, oneOf.AsT1);
    }

    [TestMethod]
    public void OneOf5_FromT2_ShouldCreateT2Instance()
    {
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT2("hello");

        Assert.IsFalse(oneOf.IsT1);
        Assert.IsTrue(oneOf.IsT2);
        Assert.IsFalse(oneOf.IsT3);
        Assert.IsFalse(oneOf.IsT4);
        Assert.IsFalse(oneOf.IsT5);
        Assert.AreEqual("hello", oneOf.AsT2);
    }

    [TestMethod]
    public void OneOf5_FromT3_ShouldCreateT3Instance()
    {
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT3(42);

        Assert.IsFalse(oneOf.IsT1);
        Assert.IsFalse(oneOf.IsT2);
        Assert.IsTrue(oneOf.IsT3);
        Assert.IsFalse(oneOf.IsT4);
        Assert.IsFalse(oneOf.IsT5);
        Assert.AreEqual(42, oneOf.AsT3);
    }

    [TestMethod]
    public void OneOf5_FromT4_ShouldCreateT4Instance()
    {
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT4(3.14);

        Assert.IsFalse(oneOf.IsT1);
        Assert.IsFalse(oneOf.IsT2);
        Assert.IsFalse(oneOf.IsT3);
        Assert.IsTrue(oneOf.IsT4);
        Assert.IsFalse(oneOf.IsT5);
        Assert.AreEqual(3.14, oneOf.AsT4);
    }

    [TestMethod]
    public void OneOf5_FromT5_ShouldCreateT5Instance()
    {
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT5(true);

        Assert.IsFalse(oneOf.IsT1);
        Assert.IsFalse(oneOf.IsT2);
        Assert.IsFalse(oneOf.IsT3);
        Assert.IsFalse(oneOf.IsT4);
        Assert.IsTrue(oneOf.IsT5);
        Assert.AreEqual(true, oneOf.AsT5);
    }

    [TestMethod]
    public void OneOf5_ImplicitConversionT1_ShouldCreateT1Instance()
    {
        var error = new TestError("err");
        OneOf<TestError, string, int, double, bool> oneOf = error;

        Assert.IsTrue(oneOf.IsT1);
        Assert.AreEqual(error, oneOf.AsT1);
    }

    [TestMethod]
    public void OneOf5_ImplicitConversionT4_ShouldCreateT4Instance()
    {
        OneOf<TestError, string, int, double, bool> oneOf = 2.71;

        Assert.IsTrue(oneOf.IsT4);
        Assert.AreEqual(2.71, oneOf.AsT4);
    }

    [TestMethod]
    public void OneOf5_ImplicitConversionT5_ShouldCreateT5Instance()
    {
        OneOf<TestError, string, int, double, bool> oneOf = false;

        Assert.IsTrue(oneOf.IsT5);
        Assert.AreEqual(false, oneOf.AsT5);
    }

    #endregion

    #region Property Access — Throw Tests

    [TestMethod]
    public void OneOf5_AsT4_WhenNotT4_ShouldThrow()
    {
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT1(new TestError("x"));

        try { var _ = oneOf.AsT4; Assert.Fail("Expected InvalidOperationException"); }
        catch (InvalidOperationException) { }
    }

    [TestMethod]
    public void OneOf5_AsT5_WhenNotT5_ShouldThrow()
    {
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT3(99);

        try { var _ = oneOf.AsT5; Assert.Fail("Expected InvalidOperationException"); }
        catch (InvalidOperationException) { }
    }

    [TestMethod]
    public void OneOf5_AsT1_WhenT5_ShouldThrow()
    {
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT5(true);

        try { var _ = oneOf.AsT1; Assert.Fail("Expected InvalidOperationException"); }
        catch (InvalidOperationException) { }
    }

    #endregion

    #region Match Tests

    [TestMethod]
    public void OneOf5_Match_WithT4_ShouldExecuteCase4()
    {
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT4(1.23);

        var result = oneOf.Match(
            case1: e => "error",
            case2: s => "string",
            case3: i => "int",
            case4: d => $"double:{d}",
            case5: b => "bool"
        );

        Assert.AreEqual("double:1.23", result);
    }

    [TestMethod]
    public void OneOf5_Match_WithT5_ShouldExecuteCase5()
    {
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT5(true);

        var result = oneOf.Match(
            case1: e => "error",
            case2: s => "string",
            case3: i => "int",
            case4: d => "double",
            case5: b => $"bool:{b}"
        );

        Assert.AreEqual("bool:True", result);
    }

    [TestMethod]
    public void OneOf5_Match_WithT1_ShouldExecuteCase1()
    {
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT1(new TestError("oops"));

        var result = oneOf.Match(
            case1: e => $"error:{e.Message}",
            case2: s => "string",
            case3: i => "int",
            case4: d => "double",
            case5: b => "bool"
        );

        Assert.AreEqual("error:oops", result);
    }

    [TestMethod]
    public void OneOf5_Match_NullCase_ShouldThrow()
    {
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT5(true);

        try
        {
            oneOf.Match(
                case1: e => "e",
                case2: s => "s",
                case3: i => "i",
                case4: d => "d",
                case5: null!
            );
            Assert.Fail("Expected ArgumentNullException");
        }
        catch (ArgumentNullException) { }
    }

    #endregion

    #region Switch Tests

    [TestMethod]
    public void OneOf5_Switch_WithT4_ShouldExecuteCase4()
    {
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT4(9.9);
        var executed = 0;

        oneOf.Switch(
            case1: _ => executed = 1,
            case2: _ => executed = 2,
            case3: _ => executed = 3,
            case4: _ => executed = 4,
            case5: _ => executed = 5
        );

        Assert.AreEqual(4, executed);
    }

    [TestMethod]
    public void OneOf5_Switch_WithT5_ShouldExecuteCase5()
    {
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT5(false);
        var executed = 0;

        oneOf.Switch(
            case1: _ => executed = 1,
            case2: _ => executed = 2,
            case3: _ => executed = 3,
            case4: _ => executed = 4,
            case5: _ => executed = 5
        );

        Assert.AreEqual(5, executed);
    }

    #endregion

    #region Map Tests

    [TestMethod]
    public void OneOf5_MapT2_WithT4_ShouldPropagateT4()
    {
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT4(5.0);

        var result = oneOf.MapT2(s => s.ToUpper());

        Assert.IsTrue(result.IsT4);
        Assert.AreEqual(5.0, result.AsT4);
    }

    [TestMethod]
    public void OneOf5_MapT2_WithT5_ShouldPropagateT5()
    {
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT5(true);

        var result = oneOf.MapT2(s => s.ToUpper());

        Assert.IsTrue(result.IsT5);
        Assert.AreEqual(true, result.AsT5);
    }

    [TestMethod]
    public void OneOf5_MapT2_WithT2_ShouldTransformT2()
    {
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT2("hello");

        var result = oneOf.MapT2(s => s.ToUpper());

        Assert.IsTrue(result.IsT2);
        Assert.AreEqual("HELLO", result.AsT2);
    }

    [TestMethod]
    public void OneOf5_MapT3_WithT3_ShouldTransformT3()
    {
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT3(7);

        var result = oneOf.MapT3(i => i * 10);

        Assert.IsTrue(result.IsT3);
        Assert.AreEqual(70, result.AsT3);
    }

    [TestMethod]
    public void OneOf5_MapT3_WithT4_ShouldPropagateT4()
    {
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT4(1.1);

        var result = oneOf.MapT3(i => i * 2);

        Assert.IsTrue(result.IsT4);
        Assert.AreEqual(1.1, result.AsT4);
    }

    [TestMethod]
    public void OneOf5_MapT4_WithT4_ShouldTransformT4()
    {
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT4(2.5);

        var result = oneOf.MapT4(d => d * 4.0);

        Assert.IsTrue(result.IsT4);
        Assert.AreEqual(10.0, result.AsT4);
    }

    [TestMethod]
    public void OneOf5_MapT4_WithT5_ShouldPropagateT5()
    {
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT5(false);

        var result = oneOf.MapT4(d => d + 1.0);

        Assert.IsTrue(result.IsT5);
        Assert.AreEqual(false, result.AsT5);
    }

    [TestMethod]
    public void OneOf5_MapT4_WithT1_ShouldPropagateT1()
    {
        var error = new TestError("fail");
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT1(error);

        var result = oneOf.MapT4(d => d * 2.0);

        Assert.IsTrue(result.IsT1);
        Assert.AreEqual(error, result.AsT1);
    }

    #endregion

    #region Bind Tests

    [TestMethod]
    public void OneOf5_BindT2_WithT2_ShouldApplyBinder()
    {
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT2("hello");

        var result = oneOf.BindT2(s => OneOf<TestError, int, int, double, bool>.FromT2(s.Length));

        Assert.IsTrue(result.IsT2);
        Assert.AreEqual(5, result.AsT2);
    }

    [TestMethod]
    public void OneOf5_BindT2_WithT5_ShouldPropagateT5()
    {
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT5(true);

        var result = oneOf.BindT2(s => OneOf<TestError, int, int, double, bool>.FromT2(s.Length));

        Assert.IsTrue(result.IsT5);
        Assert.AreEqual(true, result.AsT5);
    }

    [TestMethod]
    public void OneOf5_BindT4_WithT4_ShouldApplyBinder()
    {
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT4(3.0);

        var result = oneOf.BindT4(d => OneOf<TestError, string, int, long, bool>.FromT4((long)(d * 10)));

        Assert.IsTrue(result.IsT4);
        Assert.AreEqual(30L, result.AsT4);
    }

    [TestMethod]
    public void OneOf5_BindT4_WithT1_ShouldPropagateT1()
    {
        var error = new TestError("fail");
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT1(error);

        var result = oneOf.BindT4(d => OneOf<TestError, string, int, long, bool>.FromT4((long)d));

        Assert.IsTrue(result.IsT1);
        Assert.AreEqual(error, result.AsT1);
    }

    #endregion

    #region Equality and HashCode Tests

    [TestMethod]
    public void OneOf5_Equals_SameT4Value_ShouldBeEqual()
    {
        var a = OneOf<TestError, string, int, double, bool>.FromT4(1.0);
        var b = OneOf<TestError, string, int, double, bool>.FromT4(1.0);

        Assert.IsTrue(a.Equals(b));
        Assert.IsTrue(a == b);
        Assert.IsFalse(a != b);
    }

    [TestMethod]
    public void OneOf5_Equals_SameT5Value_ShouldBeEqual()
    {
        var a = OneOf<TestError, string, int, double, bool>.FromT5(true);
        var b = OneOf<TestError, string, int, double, bool>.FromT5(true);

        Assert.IsTrue(a.Equals(b));
        Assert.IsTrue(a == b);
    }

    [TestMethod]
    public void OneOf5_Equals_DifferentTypes_ShouldNotBeEqual()
    {
        var a = OneOf<TestError, string, int, double, bool>.FromT4(1.0);
        var b = OneOf<TestError, string, int, double, bool>.FromT5(true);

        Assert.IsFalse(a.Equals(b));
        Assert.IsTrue(a != b);
    }

    [TestMethod]
    public void OneOf5_GetHashCode_EqualValues_ShouldMatch()
    {
        var a = OneOf<TestError, string, int, double, bool>.FromT5(false);
        var b = OneOf<TestError, string, int, double, bool>.FromT5(false);

        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    #endregion

    #region ToString Tests

    [TestMethod]
    public void OneOf5_ToString_WithT4_ShouldContainT4Label()
    {
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT4(9.9);
        var result = oneOf.ToString();

        Assert.IsTrue(result.Contains("OneOf<"), result);
        Assert.IsTrue(result.Contains("T4:"), result);
        Assert.IsTrue(result.Contains("Double"), result);
    }

    [TestMethod]
    public void OneOf5_ToString_WithT5_ShouldContainT5Label()
    {
        var oneOf = OneOf<TestError, string, int, double, bool>.FromT5(true);
        var result = oneOf.ToString();

        Assert.IsTrue(result.Contains("OneOf<"), result);
        Assert.IsTrue(result.Contains("T5:"), result);
        Assert.IsTrue(result.Contains("Boolean"), result);
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
