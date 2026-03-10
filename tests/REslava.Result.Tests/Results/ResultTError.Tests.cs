using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REslava.Result.AdvancedPatterns;
using REslava.Result.Extensions;

namespace REslava.Result.Tests.Results;

/// <summary>
/// Tests for <c>Result&lt;TValue, TError&gt;</c> core, Bind (all arities),
/// Map, Tap, TapOnFailure, and implicit conversion chain.
/// </summary>
[TestClass]
public sealed class ResultTErrorTests
{
    #region Test error types

    private sealed class E1 : Error { public E1() : base("E1") { } }
    private sealed class E2 : Error { public E2() : base("E2") { } }
    private sealed class E3 : Error { public E3() : base("E3") { } }
    private sealed class E4 : Error { public E4() : base("E4") { } }
    private sealed class E5 : Error { public E5() : base("E5") { } }
    private sealed class E6 : Error { public E6() : base("E6") { } }
    private sealed class E7 : Error { public E7() : base("E7") { } }
    private sealed class E8 : Error { public E8() : base("E8") { } }

    #endregion

    // =========================================================================
    // Core
    // =========================================================================

    #region Ok / Fail construction

    [TestMethod]
    public void Ok_SetsIsSuccessTrue()
    {
        var r = Result<int, E1>.Ok(42);
        Assert.IsTrue(r.IsSuccess);
        Assert.IsFalse(r.IsFailure);
    }

    [TestMethod]
    public void Fail_SetsIsFailureTrue()
    {
        var r = Result<int, E1>.Fail(new E1());
        Assert.IsFalse(r.IsSuccess);
        Assert.IsTrue(r.IsFailure);
    }

    [TestMethod]
    public void Ok_Value_ReturnsValue()
    {
        var r = Result<string, E1>.Ok("hello");
        Assert.AreEqual("hello", r.Value);
    }

    [TestMethod]
    public void Fail_Error_ReturnsError()
    {
        var e = new E1();
        var r = Result<int, E1>.Fail(e);
        Assert.AreSame(e, r.Error);
    }

    [TestMethod]
    public void Ok_Error_Throws()
    {
        var r = Result<int, E1>.Ok(1);
        Assert.ThrowsExactly<InvalidOperationException>(() => _ = r.Error);
    }

    [TestMethod]
    public void Fail_Value_Throws()
    {
        var r = Result<int, E1>.Fail(new E1());
        Assert.ThrowsExactly<InvalidOperationException>(() => _ = r.Value);
    }

    [TestMethod]
    public void ToString_OnSuccess_ContainsOk()
    {
        var r = Result<int, E1>.Ok(99);
        StringAssert.Contains(r.ToString(), "Ok");
    }

    [TestMethod]
    public void ToString_OnFailure_ContainsFail()
    {
        var r = Result<int, E1>.Fail(new E1());
        StringAssert.Contains(r.ToString(), "Fail");
    }

    #endregion

    // =========================================================================
    // Bind
    // =========================================================================

    #region Bind 1→2

    [TestMethod]
    public void Bind_1to2_Success_ReturnsOk()
    {
        var r = Result<int, E1>.Ok(1)
            .Bind(v => Result<string, E2>.Ok($"v={v}"));

        Assert.IsTrue(r.IsSuccess);
        Assert.AreEqual("v=1", r.Value);
    }

    [TestMethod]
    public void Bind_1to2_FirstFailure_ShortCircuits()
    {
        var e1 = new E1();
        var r = Result<int, E1>.Fail(e1)
            .Bind(v => Result<string, E2>.Ok("should not reach"));

        Assert.IsTrue(r.IsFailure);
        Assert.IsTrue(r.Error.IsT1);
        Assert.AreSame(e1, r.Error.AsT1);
    }

    [TestMethod]
    public void Bind_1to2_SecondFailure_CapturedInSlot2()
    {
        var e2 = new E2();
        var r = Result<int, E1>.Ok(1)
            .Bind(v => Result<string, E2>.Fail(e2));

        Assert.IsTrue(r.IsFailure);
        Assert.IsTrue(r.Error.IsT2);
        Assert.AreSame(e2, r.Error.AsT2);
    }

    [TestMethod]
    public void Bind_1to2_NullNext_Throws()
    {
        var r = Result<int, E1>.Ok(1);
        Assert.ThrowsExactly<ArgumentNullException>(
            () => r.Bind((Func<int, Result<int, E2>>)null!));
    }

    #endregion

    #region Bind 2→3

    [TestMethod]
    public void Bind_2to3_Success_ReturnsOk()
    {
        var r = Result<int, E1>.Ok(1)
            .Bind(v => Result<int, E2>.Ok(v + 1))
            .Bind(v => Result<string, E3>.Ok($"v={v}"));

        Assert.IsTrue(r.IsSuccess);
        Assert.AreEqual("v=2", r.Value);
    }

    [TestMethod]
    public void Bind_2to3_FirstSlotFailure_WidenedToSlot1()
    {
        var e1 = new E1();
        var r = Result<int, E1>.Fail(e1)
            .Bind(v => Result<int, E2>.Ok(v))
            .Bind(v => Result<string, E3>.Ok("ok"));

        Assert.IsTrue(r.IsFailure);
        Assert.IsTrue(r.Error.IsT1);
        Assert.AreSame(e1, r.Error.AsT1);
    }

    [TestMethod]
    public void Bind_2to3_SecondSlotFailure_WidenedToSlot2()
    {
        var e2 = new E2();
        var r = Result<int, E1>.Ok(1)
            .Bind(v => Result<int, E2>.Fail(e2))
            .Bind(v => Result<string, E3>.Ok("ok"));

        Assert.IsTrue(r.IsFailure);
        Assert.IsTrue(r.Error.IsT2);
        Assert.AreSame(e2, r.Error.AsT2);
    }

    [TestMethod]
    public void Bind_2to3_ThirdSlotFailure_InSlot3()
    {
        var e3 = new E3();
        var r = Result<int, E1>.Ok(1)
            .Bind(v => Result<int, E2>.Ok(v))
            .Bind(v => Result<string, E3>.Fail(e3));

        Assert.IsTrue(r.IsFailure);
        Assert.IsTrue(r.Error.IsT3);
        Assert.AreSame(e3, r.Error.AsT3);
    }

    #endregion

    #region Bind 3→4

    [TestMethod]
    public void Bind_3to4_Success_ReturnsOk()
    {
        var r = Result<int, E1>.Ok(0)
            .Bind(v => Result<int, E2>.Ok(v + 1))
            .Bind(v => Result<int, E3>.Ok(v + 1))
            .Bind(v => Result<string, E4>.Ok($"v={v}"));

        Assert.IsTrue(r.IsSuccess);
        Assert.AreEqual("v=2", r.Value);
    }

    [TestMethod]
    public void Bind_3to4_FourthSlotFailure_InSlot4()
    {
        var e4 = new E4();
        var r = Result<int, E1>.Ok(0)
            .Bind(v => Result<int, E2>.Ok(v))
            .Bind(v => Result<int, E3>.Ok(v))
            .Bind(v => Result<string, E4>.Fail(e4));

        Assert.IsTrue(r.IsFailure);
        Assert.IsTrue(r.Error.IsT4);
        Assert.AreSame(e4, r.Error.AsT4);
    }

    #endregion

    #region Bind 4→5

    [TestMethod]
    public void Bind_4to5_Success_ReturnsOk()
    {
        var r = Result<int, E1>.Ok(0)
            .Bind(v => Result<int, E2>.Ok(v + 1))
            .Bind(v => Result<int, E3>.Ok(v + 1))
            .Bind(v => Result<int, E4>.Ok(v + 1))
            .Bind(v => Result<string, E5>.Ok($"v={v}"));

        Assert.IsTrue(r.IsSuccess);
        Assert.AreEqual("v=3", r.Value);
    }

    [TestMethod]
    public void Bind_4to5_FifthSlotFailure_InSlot5()
    {
        var e5 = new E5();
        var r = Result<int, E1>.Ok(0)
            .Bind(v => Result<int, E2>.Ok(v))
            .Bind(v => Result<int, E3>.Ok(v))
            .Bind(v => Result<int, E4>.Ok(v))
            .Bind(v => Result<string, E5>.Fail(e5));

        Assert.IsTrue(r.IsFailure);
        Assert.IsTrue(r.Error.IsT5);
        Assert.AreSame(e5, r.Error.AsT5);
    }

    #endregion

    #region Bind 5→6

    [TestMethod]
    public void Bind_5to6_Success_ReturnsOk()
    {
        var r = Result<int, E1>.Ok(0)
            .Bind(v => Result<int, E2>.Ok(v + 1))
            .Bind(v => Result<int, E3>.Ok(v + 1))
            .Bind(v => Result<int, E4>.Ok(v + 1))
            .Bind(v => Result<int, E5>.Ok(v + 1))
            .Bind(v => Result<string, E6>.Ok($"v={v}"));

        Assert.IsTrue(r.IsSuccess);
        Assert.AreEqual("v=4", r.Value);
    }

    [TestMethod]
    public void Bind_5to6_SixthSlotFailure_InSlot6()
    {
        var e6 = new E6();
        var r = Result<int, E1>.Ok(0)
            .Bind(v => Result<int, E2>.Ok(v))
            .Bind(v => Result<int, E3>.Ok(v))
            .Bind(v => Result<int, E4>.Ok(v))
            .Bind(v => Result<int, E5>.Ok(v))
            .Bind(v => Result<string, E6>.Fail(e6));

        Assert.IsTrue(r.IsFailure);
        Assert.IsTrue(r.Error.IsT6);
        Assert.AreSame(e6, r.Error.AsT6);
    }

    #endregion

    #region Bind 6→7

    [TestMethod]
    public void Bind_6to7_Success_ReturnsOk()
    {
        var r = Result<int, E1>.Ok(0)
            .Bind(v => Result<int, E2>.Ok(v + 1))
            .Bind(v => Result<int, E3>.Ok(v + 1))
            .Bind(v => Result<int, E4>.Ok(v + 1))
            .Bind(v => Result<int, E5>.Ok(v + 1))
            .Bind(v => Result<int, E6>.Ok(v + 1))
            .Bind(v => Result<string, E7>.Ok($"v={v}"));

        Assert.IsTrue(r.IsSuccess);
        Assert.AreEqual("v=5", r.Value);
    }

    [TestMethod]
    public void Bind_6to7_SeventhSlotFailure_InSlot7()
    {
        var e7 = new E7();
        var r = Result<int, E1>.Ok(0)
            .Bind(v => Result<int, E2>.Ok(v))
            .Bind(v => Result<int, E3>.Ok(v))
            .Bind(v => Result<int, E4>.Ok(v))
            .Bind(v => Result<int, E5>.Ok(v))
            .Bind(v => Result<int, E6>.Ok(v))
            .Bind(v => Result<string, E7>.Fail(e7));

        Assert.IsTrue(r.IsFailure);
        Assert.IsTrue(r.Error.IsT7);
        Assert.AreSame(e7, r.Error.AsT7);
    }

    #endregion

    #region Bind 7→8

    [TestMethod]
    public void Bind_7to8_Success_ReturnsOk()
    {
        var r = Result<int, E1>.Ok(0)
            .Bind(v => Result<int, E2>.Ok(v + 1))
            .Bind(v => Result<int, E3>.Ok(v + 1))
            .Bind(v => Result<int, E4>.Ok(v + 1))
            .Bind(v => Result<int, E5>.Ok(v + 1))
            .Bind(v => Result<int, E6>.Ok(v + 1))
            .Bind(v => Result<int, E7>.Ok(v + 1))
            .Bind(v => Result<string, E8>.Ok($"v={v}"));

        Assert.IsTrue(r.IsSuccess);
        Assert.AreEqual("v=6", r.Value);
    }

    [TestMethod]
    public void Bind_7to8_EighthSlotFailure_InSlot8()
    {
        var e8 = new E8();
        var r = Result<int, E1>.Ok(0)
            .Bind(v => Result<int, E2>.Ok(v))
            .Bind(v => Result<int, E3>.Ok(v))
            .Bind(v => Result<int, E4>.Ok(v))
            .Bind(v => Result<int, E5>.Ok(v))
            .Bind(v => Result<int, E6>.Ok(v))
            .Bind(v => Result<int, E7>.Ok(v))
            .Bind(v => Result<string, E8>.Fail(e8));

        Assert.IsTrue(r.IsFailure);
        Assert.IsTrue(r.Error.IsT8);
        Assert.AreSame(e8, r.Error.AsT8);
    }

    [TestMethod]
    public void Bind_7to8_FirstSlotEarlyFailure_WidenedToSlot1()
    {
        var e1 = new E1();
        var r = Result<int, E1>.Fail(e1)
            .Bind(v => Result<int, E2>.Ok(v))
            .Bind(v => Result<int, E3>.Ok(v))
            .Bind(v => Result<int, E4>.Ok(v))
            .Bind(v => Result<int, E5>.Ok(v))
            .Bind(v => Result<int, E6>.Ok(v))
            .Bind(v => Result<int, E7>.Ok(v))
            .Bind(v => Result<string, E8>.Ok("ok"));

        Assert.IsTrue(r.IsFailure);
        Assert.IsTrue(r.Error.IsT1);
        Assert.AreSame(e1, r.Error.AsT1);
    }

    #endregion

    // =========================================================================
    // Map
    // =========================================================================

    #region Map

    [TestMethod]
    public void Map_OnSuccess_TransformsValue()
    {
        var r = Result<int, E1>.Ok(21).Map(v => v * 2);
        Assert.IsTrue(r.IsSuccess);
        Assert.AreEqual(42, r.Value);
    }

    [TestMethod]
    public void Map_OnFailure_ForwardsError()
    {
        var e = new E1();
        var r = Result<int, E1>.Fail(e).Map(v => v * 2);
        Assert.IsTrue(r.IsFailure);
        Assert.AreSame(e, r.Error);
    }

    [TestMethod]
    public void Map_NullMapper_Throws()
    {
        var r = Result<int, E1>.Ok(1);
        Assert.ThrowsExactly<ArgumentNullException>(
            () => r.Map((Func<int, string>)null!));
    }

    #endregion

    // =========================================================================
    // Tap / TapOnFailure
    // =========================================================================

    #region Tap

    [TestMethod]
    public void Tap_OnSuccess_ActionCalled_ResultUnchanged()
    {
        var called = false;
        var r = Result<int, E1>.Ok(7)
            .Tap(v => { called = true; Assert.AreEqual(7, v); });

        Assert.IsTrue(called);
        Assert.IsTrue(r.IsSuccess);
        Assert.AreEqual(7, r.Value);
    }

    [TestMethod]
    public void Tap_OnFailure_ActionNotCalled()
    {
        var called = false;
        var r = Result<int, E1>.Fail(new E1())
            .Tap(_ => called = true);

        Assert.IsFalse(called);
        Assert.IsTrue(r.IsFailure);
    }

    [TestMethod]
    public void Tap_NullAction_Throws()
    {
        var r = Result<int, E1>.Ok(1);
        Assert.ThrowsExactly<ArgumentNullException>(
            () => r.Tap((Action<int>)null!));
    }

    #endregion

    #region TapOnFailure

    [TestMethod]
    public void TapOnFailure_OnFailure_ActionCalled_ResultUnchanged()
    {
        var e = new E1();
        var called = false;
        var r = Result<int, E1>.Fail(e)
            .TapOnFailure(err => { called = true; Assert.AreSame(e, err); });

        Assert.IsTrue(called);
        Assert.IsTrue(r.IsFailure);
        Assert.AreSame(e, r.Error);
    }

    [TestMethod]
    public void TapOnFailure_OnSuccess_ActionNotCalled()
    {
        var called = false;
        var r = Result<int, E1>.Ok(1)
            .TapOnFailure(_ => called = true);

        Assert.IsFalse(called);
        Assert.IsTrue(r.IsSuccess);
    }

    [TestMethod]
    public void TapOnFailure_NullAction_Throws()
    {
        var r = Result<int, E1>.Fail(new E1());
        Assert.ThrowsExactly<ArgumentNullException>(
            () => r.TapOnFailure((Action<E1>)null!));
    }

    #endregion

    // =========================================================================
    // Implicit conversion chain (T → ErrorsOf)
    // =========================================================================

    #region Implicit conversions

    [TestMethod]
    public void ImplicitConversion_T1_To_ErrorsOf2()
    {
        ErrorsOf<E1, E2> union = new E1();
        Assert.IsTrue(union.IsT1);
    }

    [TestMethod]
    public void ImplicitConversion_T2_To_ErrorsOf2()
    {
        ErrorsOf<E1, E2> union = new E2();
        Assert.IsTrue(union.IsT2);
    }

    [TestMethod]
    public void ImplicitConversion_T3_To_ErrorsOf3()
    {
        ErrorsOf<E1, E2, E3> union = new E3();
        Assert.IsTrue(union.IsT3);
    }

    [TestMethod]
    public void ImplicitConversion_T8_To_ErrorsOf8()
    {
        ErrorsOf<E1, E2, E3, E4, E5, E6, E7, E8> union = new E8();
        Assert.IsTrue(union.IsT8);
    }

    [TestMethod]
    public void ImplicitConversion_ErrorUsableAsResultError()
    {
        // Verifies implicit T → ErrorsOf<T1,T2> works inside Result.Fail
        var e2 = new E2();
        Result<int, ErrorsOf<E1, E2>> r = Result<int, ErrorsOf<E1, E2>>.Fail(e2);
        Assert.IsTrue(r.IsFailure);
        Assert.IsTrue(r.Error.IsT2);
        Assert.AreSame(e2, r.Error.AsT2);
    }

    #endregion

    // =========================================================================
    // Ensure (sync)
    // =========================================================================

    #region Ensure 1→2

    [TestMethod]
    public void Ensure_1to2_PredicatePass_ReturnsOkWidened()
    {
        var r = Result<int, E1>.Ok(10)
            .Ensure(v => v > 0, new E2());

        Assert.IsTrue(r.IsSuccess);
        Assert.AreEqual(10, r.Value);
    }

    [TestMethod]
    public void Ensure_1to2_PredicateFail_ReturnsNewErrorInSlot2()
    {
        var e2 = new E2();
        var r = Result<int, E1>.Ok(10)
            .Ensure(v => v > 100, e2);

        Assert.IsTrue(r.IsFailure);
        Assert.IsTrue(r.Error.IsT2);
        Assert.AreSame(e2, r.Error.AsT2);
    }

    [TestMethod]
    public void Ensure_1to2_AlreadyFailed_OriginalErrorWidenedToSlot1()
    {
        var e1 = new E1();
        var r = Result<int, E1>.Fail(e1)
            .Ensure(v => v > 0, new E2());

        Assert.IsTrue(r.IsFailure);
        Assert.IsTrue(r.Error.IsT1);
        Assert.AreSame(e1, r.Error.AsT1);
    }

    [TestMethod]
    public void Ensure_1to2_NullPredicate_Throws()
    {
        var r = Result<int, E1>.Ok(1);
        Assert.ThrowsExactly<ArgumentNullException>(
            () => r.Ensure((Func<int, bool>)null!, new E2()));
    }

    #endregion

    #region Ensure 2→3

    [TestMethod]
    public void Ensure_2to3_PredicatePass_ReturnsOk()
    {
        var r = Result<int, E1>.Ok(5)
            .Ensure(v => v > 0, new E2())
            .Ensure(v => v < 100, new E3());

        Assert.IsTrue(r.IsSuccess);
        Assert.AreEqual(5, r.Value);
    }

    [TestMethod]
    public void Ensure_2to3_SecondPredicateFail_NewErrorInSlot3()
    {
        var e3 = new E3();
        var r = Result<int, E1>.Ok(5)
            .Ensure(v => v > 0, new E2())
            .Ensure(v => v > 100, e3);

        Assert.IsTrue(r.IsFailure);
        Assert.IsTrue(r.Error.IsT3);
        Assert.AreSame(e3, r.Error.AsT3);
    }

    [TestMethod]
    public void Ensure_2to3_FirstSlotFailure_WidenedToSlot1()
    {
        var e1 = new E1();
        var r = Result<int, E1>.Fail(e1)
            .Ensure(v => v > 0, new E2())
            .Ensure(v => v < 100, new E3());

        Assert.IsTrue(r.IsFailure);
        Assert.IsTrue(r.Error.IsT1);
        Assert.AreSame(e1, r.Error.AsT1);
    }

    [TestMethod]
    public void Ensure_2to3_SecondSlotFailure_WidenedToSlot2()
    {
        var e2 = new E2();
        var r = Result<int, E1>.Ok(5)
            .Ensure(v => v > 100, e2)   // first guard fails
            .Ensure(v => v < 100, new E3());

        Assert.IsTrue(r.IsFailure);
        Assert.IsTrue(r.Error.IsT2);
        Assert.AreSame(e2, r.Error.AsT2);
    }

    #endregion

    #region Ensure 3→4 (spot check)

    [TestMethod]
    public void Ensure_3to4_PredicateFail_NewErrorInSlot4()
    {
        var e4 = new E4();
        var r = Result<int, E1>.Ok(5)
            .Ensure(v => v > 0, new E2())
            .Ensure(v => v < 100, new E3())
            .Ensure(v => v % 2 == 0, e4);  // 5 is odd → fail

        Assert.IsTrue(r.IsFailure);
        Assert.IsTrue(r.Error.IsT4);
        Assert.AreSame(e4, r.Error.AsT4);
    }

    #endregion

    #region Ensure 7→8 (boundary check)

    [TestMethod]
    public void Ensure_7to8_PredicateFail_NewErrorInSlot8()
    {
        var e8 = new E8();
        var r = Result<int, E1>.Ok(1)
            .Ensure(v => v > 0, new E2())
            .Ensure(v => v > 0, new E3())
            .Ensure(v => v > 0, new E4())
            .Ensure(v => v > 0, new E5())
            .Ensure(v => v > 0, new E6())
            .Ensure(v => v > 0, new E7())
            .Ensure(v => v > 100, e8);   // fails

        Assert.IsTrue(r.IsFailure);
        Assert.IsTrue(r.Error.IsT8);
        Assert.AreSame(e8, r.Error.AsT8);
    }

    [TestMethod]
    public void Ensure_7to8_AllPass_ReturnsOk()
    {
        var r = Result<int, E1>.Ok(1)
            .Ensure(v => v > 0, new E2())
            .Ensure(v => v > 0, new E3())
            .Ensure(v => v > 0, new E4())
            .Ensure(v => v > 0, new E5())
            .Ensure(v => v > 0, new E6())
            .Ensure(v => v > 0, new E7())
            .Ensure(v => v > 0, new E8());

        Assert.IsTrue(r.IsSuccess);
        Assert.AreEqual(1, r.Value);
    }

    #endregion

    // =========================================================================
    // EnsureAsync
    // =========================================================================

    #region EnsureAsync 1→2

    [TestMethod]
    public async Task EnsureAsync_1to2_PredicatePass_ReturnsOk()
    {
        var r = await Result<int, E1>.Ok(10)
            .EnsureAsync(v => Task.FromResult(v > 0), new E2());

        Assert.IsTrue(r.IsSuccess);
        Assert.AreEqual(10, r.Value);
    }

    [TestMethod]
    public async Task EnsureAsync_1to2_PredicateFail_NewErrorInSlot2()
    {
        var e2 = new E2();
        var r = await Result<int, E1>.Ok(10)
            .EnsureAsync(v => Task.FromResult(v > 100), e2);

        Assert.IsTrue(r.IsFailure);
        Assert.IsTrue(r.Error.IsT2);
        Assert.AreSame(e2, r.Error.AsT2);
    }

    [TestMethod]
    public async Task EnsureAsync_1to2_AlreadyFailed_OriginalErrorWidened()
    {
        var e1 = new E1();
        var r = await Result<int, E1>.Fail(e1)
            .EnsureAsync(v => Task.FromResult(v > 0), new E2());

        Assert.IsTrue(r.IsFailure);
        Assert.IsTrue(r.Error.IsT1);
        Assert.AreSame(e1, r.Error.AsT1);
    }

    [TestMethod]
    public async Task EnsureAsync_1to2_NullPredicate_Throws()
    {
        var r = Result<int, E1>.Ok(1);
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(
            () => r.EnsureAsync((Func<int, Task<bool>>)null!, new E2()));
    }

    #endregion

    #region EnsureAsync 2→3 (spot check)

    [TestMethod]
    public async Task EnsureAsync_2to3_SecondPredicateFail_NewErrorInSlot3()
    {
        var e3 = new E3();
        var intermediate = Result<int, E1>.Ok(5)
            .Ensure(v => v > 0, new E2());

        var r = await intermediate
            .EnsureAsync(v => Task.FromResult(v > 100), e3);

        Assert.IsTrue(r.IsFailure);
        Assert.IsTrue(r.Error.IsT3);
        Assert.AreSame(e3, r.Error.AsT3);
    }

    #endregion

    // =========================================================================
    // MapError
    // =========================================================================

    #region MapError

    [TestMethod]
    public void MapError_OnFailure_TranslatesError()
    {
        var e1 = new E1();
        var r = Result<int, E1>.Fail(e1)
            .MapError(e => new E2());

        Assert.IsTrue(r.IsFailure);
        Assert.IsInstanceOfType<E2>(r.Error);
    }

    [TestMethod]
    public void MapError_OnSuccess_ForwardsValue()
    {
        var r = Result<int, E1>.Ok(42)
            .MapError(e => new E2());

        Assert.IsTrue(r.IsSuccess);
        Assert.AreEqual(42, r.Value);
    }

    [TestMethod]
    public void MapError_CollapseUnion_SingleError()
    {
        // Collapse ErrorsOf<E1, E2> → E1 by mapping both arms
        var union = ErrorsOf<E1, E2>.FromT2(new E2());
        var r = Result<int, ErrorsOf<E1, E2>>.Fail(union)
            .MapError(e => e.Match(
                v => (IError)new E1(),
                v => new E1()));

        Assert.IsTrue(r.IsFailure);
        Assert.IsInstanceOfType<E1>(r.Error);
    }

    [TestMethod]
    public void MapError_NullMapper_Throws()
    {
        var r = Result<int, E1>.Fail(new E1());
        Assert.ThrowsExactly<ArgumentNullException>(
            () => r.MapError((Func<E1, E2>)null!));
    }

    #endregion

    // =========================================================================
    // Full pipeline smoke test
    // =========================================================================

    #region Pipeline

    [TestMethod]
    public void Pipeline_AllSuccess_ReturnsOkWithFinalValue()
    {
        static Result<int, E1> Step1(int v)    => Result<int, E1>.Ok(v + 10);
        static Result<int, E2> Step2(int v)    => Result<int, E2>.Ok(v + 10);
        static Result<string, E3> Step3(int v) => Result<string, E3>.Ok($"done={v}");

        var result = Step1(0).Bind(Step2).Bind(Step3);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("done=20", result.Value);
    }

    [TestMethod]
    public void Pipeline_FailInMiddle_SkipsRemainingSteps()
    {
        var step2Called = false;
        var e1 = new E1();

        Result<int, E1> Step1(int v) => Result<int, E1>.Fail(e1);
        Result<int, E2> Step2(int v) { step2Called = true; return Result<int, E2>.Ok(v); }
        Result<string, E3> Step3(int v) => Result<string, E3>.Ok("ok");

        var result = Step1(0).Bind(Step2).Bind(Step3);

        Assert.IsFalse(step2Called);
        Assert.IsTrue(result.IsFailure);
        Assert.IsTrue(result.Error.IsT1);
        Assert.AreSame(e1, result.Error.AsT1);
    }

    [TestMethod]
    public void Pipeline_MapAndTap_IntegrateCorrectly()
    {
        var tapped = 0;

        var result = Result<int, E1>.Ok(5)
            .Tap(v => tapped = v)
            .Map(v => v * 2)
            .Bind(v => Result<string, E2>.Ok($"result={v}"));

        Assert.AreEqual(5, tapped);
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("result=10", result.Value);
    }

    #endregion
}
