using System.Collections.Immutable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REslava.Result;

namespace REslava.Result.Tests.Results;

[TestClass]
public sealed class ResultDeconstructTests
{
    #region Result<T> — 2-component (value, errors)

    [TestMethod]
    public void Deconstruct_ResultOfT_2Way_Success_ReturnsValueAndEmptyErrors()
    {
        var result = Result<string>.Ok("hello");

        var (value, errors) = result;

        Assert.AreEqual("hello", value);
        Assert.AreEqual(0, errors.Count);
    }

    [TestMethod]
    public void Deconstruct_ResultOfT_2Way_Failure_ReturnsDefaultValueAndErrors()
    {
        var result = Result<string>.Fail("not found");

        var (value, errors) = result;

        Assert.IsNull(value);
        Assert.AreEqual(1, errors.Count);
        Assert.AreEqual("not found", errors[0].Message);
    }

    #endregion

    #region Result<T> — 3-component (isSuccess, value, errors)

    [TestMethod]
    public void Deconstruct_ResultOfT_3Way_Success_ReturnsTrueValueAndEmptyErrors()
    {
        var result = Result<int>.Ok(42);

        var (isSuccess, value, errors) = result;

        Assert.IsTrue(isSuccess);
        Assert.AreEqual(42, value);
        Assert.AreEqual(0, errors.Count);
    }

    [TestMethod]
    public void Deconstruct_ResultOfT_3Way_Failure_ReturnsFalseDefaultAndErrors()
    {
        var result = Result<int>.Fail("invalid");

        var (isSuccess, value, errors) = result;

        Assert.IsFalse(isSuccess);
        Assert.AreEqual(0, value);
        Assert.AreEqual(1, errors.Count);
        Assert.AreEqual("invalid", errors[0].Message);
    }

    #endregion

    #region Result (non-generic) — 2-component (isSuccess, errors)

    [TestMethod]
    public void Deconstruct_Result_2Way_Success_ReturnsTrueAndEmptyErrors()
    {
        var result = Result.Ok();

        var (isSuccess, errors) = result;

        Assert.IsTrue(isSuccess);
        Assert.AreEqual(0, errors.Count);
    }

    [TestMethod]
    public void Deconstruct_Result_2Way_Failure_ReturnsFalseAndErrors()
    {
        var result = Result.Fail("something failed");

        var (isSuccess, errors) = result;

        Assert.IsFalse(isSuccess);
        Assert.AreEqual(1, errors.Count);
        Assert.AreEqual("something failed", errors[0].Message);
    }

    #endregion

    #region Multiple errors

    [TestMethod]
    public void Deconstruct_ResultOfT_3Way_MultipleErrors_ReturnsAllErrors()
    {
        var result = Result<string>.Fail(new[] { "error one", "error two" });

        var (isSuccess, value, errors) = result;

        Assert.IsFalse(isSuccess);
        Assert.IsNull(value);
        Assert.AreEqual(2, errors.Count);
    }

    #endregion
}
