using Microsoft.VisualStudio.TestTools.UnitTesting;
using REslava.Result;

namespace REslava.Result.Tests.Factories;

[TestClass]
public sealed class ResultValidateTests
{
    #region 2-way

    [TestMethod]
    public void Validate_2Way_AllSucceed_ShouldReturnMappedValue()
    {
        var r1 = Result<int>.Ok(10);
        var r2 = Result<string>.Ok("hello");

        var result = Result.Validate(r1, r2, (n, s) => $"{n}-{s}");

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("10-hello", result.Value);
    }

    [TestMethod]
    public void Validate_2Way_OneFails_ShouldReturnErrorsFromFailing()
    {
        var r1 = Result<int>.Ok(10);
        var r2 = Result<string>.Fail("invalid email");

        var result = Result.Validate(r1, r2, (n, s) => $"{n}-{s}");

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.AreEqual("invalid email", result.Errors[0].Message);
    }

    [TestMethod]
    public void Validate_2Way_BothFail_ShouldAccumulateAllErrors()
    {
        var r1 = Result<int>.Fail("invalid name");
        var r2 = Result<string>.Fail("invalid email");

        var result = Result.Validate(r1, r2, (n, s) => $"{n}-{s}");

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual(2, result.Errors.Count);
    }

    [TestMethod]
    public void Validate_2Way_OneFails_MapperNotCalled()
    {
        var r1 = Result<int>.Fail("error");
        var r2 = Result<string>.Ok("hello");
        var mapperCalled = false;

        Result.Validate(r1, r2, (n, s) => { mapperCalled = true; return $"{n}-{s}"; });

        Assert.IsFalse(mapperCalled);
    }

    [TestMethod]
    public void Validate_2Way_NullMapper_ShouldThrowArgumentNullException()
    {
        var r1 = Result<int>.Ok(10);
        var r2 = Result<string>.Ok("hello");

        Assert.ThrowsExactly<ArgumentNullException>(() =>
            Result.Validate<int, string, string>(r1, r2, null!));
    }

    #endregion

    #region 3-way

    [TestMethod]
    public void Validate_3Way_AllSucceed_ShouldReturnMappedValue()
    {
        var r1 = Result<int>.Ok(1);
        var r2 = Result<string>.Ok("two");
        var r3 = Result<int>.Ok(3);

        var result = Result.Validate(r1, r2, r3, (a, b, c) => $"{a}-{b}-{c}");

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("1-two-3", result.Value);
    }

    [TestMethod]
    public void Validate_3Way_OneFails_ShouldReturnErrors()
    {
        var r1 = Result<int>.Ok(1);
        var r2 = Result<string>.Fail("invalid email");
        var r3 = Result<int>.Ok(3);

        var result = Result.Validate(r1, r2, r3, (a, b, c) => $"{a}-{b}-{c}");

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.AreEqual("invalid email", result.Errors[0].Message);
    }

    [TestMethod]
    public void Validate_3Way_MultipleFail_ShouldAccumulateAllErrors()
    {
        var r1 = Result<int>.Fail("error name");
        var r2 = Result<string>.Ok("two");
        var r3 = Result<int>.Fail("error age");

        var result = Result.Validate(r1, r2, r3, (a, b, c) => $"{a}-{b}-{c}");

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual(2, result.Errors.Count);
    }

    [TestMethod]
    public void Validate_3Way_OneFails_MapperNotCalled()
    {
        var r1 = Result<int>.Fail("error");
        var r2 = Result<string>.Ok("two");
        var r3 = Result<int>.Ok(3);
        var mapperCalled = false;

        Result.Validate(r1, r2, r3, (a, b, c) => { mapperCalled = true; return ""; });

        Assert.IsFalse(mapperCalled);
    }

    [TestMethod]
    public void Validate_3Way_NullMapper_ShouldThrowArgumentNullException()
    {
        var r1 = Result<int>.Ok(1);
        var r2 = Result<string>.Ok("two");
        var r3 = Result<int>.Ok(3);

        Assert.ThrowsExactly<ArgumentNullException>(() =>
            Result.Validate<int, string, int, string>(r1, r2, r3, null!));
    }

    #endregion

    #region 4-way

    [TestMethod]
    public void Validate_4Way_AllSucceed_ShouldReturnMappedValue()
    {
        var r1 = Result<int>.Ok(1);
        var r2 = Result<string>.Ok("two");
        var r3 = Result<int>.Ok(3);
        var r4 = Result<int>.Ok(4);

        var result = Result.Validate(r1, r2, r3, r4, (a, b, c, d) => $"{a}-{b}-{c}-{d}");

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("1-two-3-4", result.Value);
    }

    [TestMethod]
    public void Validate_4Way_OneFails_ShouldReturnErrors()
    {
        var r1 = Result<int>.Ok(1);
        var r2 = Result<string>.Fail("invalid email");
        var r3 = Result<int>.Ok(3);
        var r4 = Result<int>.Ok(4);

        var result = Result.Validate(r1, r2, r3, r4, (a, b, c, d) => $"{a}-{b}-{c}-{d}");

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual(1, result.Errors.Count);
    }

    [TestMethod]
    public void Validate_4Way_MultipleFail_ShouldAccumulateAllErrors()
    {
        var r1 = Result<int>.Fail("error name");
        var r2 = Result<string>.Fail("error email");
        var r3 = Result<int>.Ok(3);
        var r4 = Result<int>.Fail("error age");

        var result = Result.Validate(r1, r2, r3, r4, (a, b, c, d) => "");

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual(3, result.Errors.Count);
    }

    [TestMethod]
    public void Validate_4Way_OneFails_MapperNotCalled()
    {
        var r1 = Result<int>.Ok(1);
        var r2 = Result<string>.Ok("two");
        var r3 = Result<int>.Fail("error");
        var r4 = Result<int>.Ok(4);
        var mapperCalled = false;

        Result.Validate(r1, r2, r3, r4, (a, b, c, d) => { mapperCalled = true; return ""; });

        Assert.IsFalse(mapperCalled);
    }

    [TestMethod]
    public void Validate_4Way_NullMapper_ShouldThrowArgumentNullException()
    {
        var r1 = Result<int>.Ok(1);
        var r2 = Result<string>.Ok("two");
        var r3 = Result<int>.Ok(3);
        var r4 = Result<int>.Ok(4);

        Assert.ThrowsExactly<ArgumentNullException>(() =>
            Result.Validate<int, string, int, int, string>(r1, r2, r3, r4, null!));
    }

    #endregion
}
