using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REslava.Result;

namespace REslava.Result.Tests.Results;

[TestClass]
public sealed class ResultActivityTests
{
    #region Instance method — WithActivity — success

    [TestMethod]
    public void WithActivity_SuccessResult_SetsOutcomeSuccessAndStatusOk()
    {
        // Arrange
        var result = new Result<int>(42, new Success("ok"));
        using var activity = new Activity("test-op").Start();

        // Act
        result.WithActivity(activity);

        // Assert
        Assert.AreEqual("success", activity.GetTagItem("result.outcome") as string);
        Assert.AreEqual(ActivityStatusCode.Ok, activity.Status);
    }

    [TestMethod]
    public void WithActivity_SuccessResult_NoErrorTagsSet()
    {
        // Arrange
        var result = new Result<int>(42, new Success("ok"));
        using var activity = new Activity("test-op").Start();

        // Act
        result.WithActivity(activity);

        // Assert
        Assert.IsNull(activity.GetTagItem("result.error.type"));
        Assert.IsNull(activity.GetTagItem("result.error.message"));
        Assert.IsNull(activity.GetTagItem("result.error.count"));
    }

    [TestMethod]
    public void WithActivity_SuccessResult_ReturnsOriginalResult()
    {
        // Arrange
        var result = new Result<int>(42, new Success("ok"));
        using var activity = new Activity("test-op").Start();

        // Act
        var returned = result.WithActivity(activity);

        // Assert — side-effect only, same instance
        Assert.IsTrue(ReferenceEquals(result, returned));
    }

    #endregion

    #region Instance method — WithActivity — failure

    [TestMethod]
    public void WithActivity_FailureResult_SetsOutcomeFailureAndStatusError()
    {
        // Arrange
        var result = new Result<int>(default, ImmutableList.Create<IReason>(new Error("something went wrong")));
        using var activity = new Activity("test-op").Start();

        // Act
        result.WithActivity(activity);

        // Assert
        Assert.AreEqual("failure", activity.GetTagItem("result.outcome") as string);
        Assert.AreEqual(ActivityStatusCode.Error, activity.Status);
        Assert.AreEqual("something went wrong", activity.StatusDescription);
    }

    [TestMethod]
    public void WithActivity_FailureResult_SetsErrorTypeAndMessage()
    {
        // Arrange
        var result = new Result<int>(default, ImmutableList.Create<IReason>(new NotFoundError("User", 42)));
        using var activity = new Activity("test-op").Start();

        // Act
        result.WithActivity(activity);

        // Assert
        Assert.AreEqual("NotFoundError", activity.GetTagItem("result.error.type") as string);
        Assert.IsNotNull(activity.GetTagItem("result.error.message") as string);
    }

    [TestMethod]
    public void WithActivity_FailureWithMultipleErrors_SetsErrorCount()
    {
        // Arrange
        var result = new Result<int>(default, ImmutableList.Create<IReason>(
            new Error("first"),
            new Error("second"),
            new Error("third")));
        using var activity = new Activity("test-op").Start();

        // Act
        result.WithActivity(activity);

        // Assert
        Assert.AreEqual("3", activity.GetTagItem("result.error.count") as string);
        Assert.AreEqual("first", activity.GetTagItem("result.error.message") as string); // first error
    }

    [TestMethod]
    public void WithActivity_FailureWithSingleError_NoErrorCountTag()
    {
        // Arrange
        var result = new Result<int>(default, ImmutableList.Create<IReason>(new Error("only one")));
        using var activity = new Activity("test-op").Start();

        // Act
        result.WithActivity(activity);

        // Assert
        Assert.IsNull(activity.GetTagItem("result.error.count"));
    }

    [TestMethod]
    public void WithActivity_FailureResult_ReturnsOriginalResult()
    {
        // Arrange
        var result = new Result<int>(default, ImmutableList.Create<IReason>(new Error("fail")));
        using var activity = new Activity("test-op").Start();

        // Act
        var returned = result.WithActivity(activity);

        // Assert — side-effect only, same instance
        Assert.IsTrue(ReferenceEquals(result, returned));
    }

    #endregion

    #region Instance method — WithActivity — null activity

    [TestMethod]
    public void WithActivity_NullActivity_SuccessResult_ReturnsUnchangedNoException()
    {
        // Arrange
        var result = new Result<int>(42, new Success("ok"));

        // Act — must not throw
        var returned = result.WithActivity(null);

        // Assert
        Assert.IsTrue(returned.IsSuccess);
        Assert.AreEqual(42, returned.Value);
    }

    [TestMethod]
    public void WithActivity_NullActivity_FailureResult_ReturnsUnchangedNoException()
    {
        // Arrange
        var result = new Result<int>(default, ImmutableList.Create<IReason>(new Error("fail")));

        // Act — must not throw
        var returned = result.WithActivity(null);

        // Assert
        Assert.IsTrue(returned.IsFailure);
        Assert.AreEqual("fail", returned.Errors[0].Message);
    }

    #endregion
}
