using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REslava.Result;
using REslava.Result.Extensions;

namespace REslava.Result.Tests.Extensions;

[TestClass]
public sealed class ResultActivityExtensionsTests
{
    [TestMethod]
    public async Task WithActivity_TaskSuccessResult_SetsOutcomeSuccessAndStatusOk()
    {
        // Arrange
        var task = Task.FromResult(new Result<int>(42, new Success("ok")));
        using var activity = new Activity("test-op").Start();

        // Act
        var result = await task.WithActivity(activity);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("success", activity.GetTagItem("result.outcome") as string);
        Assert.AreEqual(ActivityStatusCode.Ok, activity.Status);
    }

    [TestMethod]
    public async Task WithActivity_TaskFailureResult_SetsFailureTagsAndErrorStatus()
    {
        // Arrange
        var task = Task.FromResult(
            new Result<int>(default, ImmutableList.Create<IReason>(new Error("db timeout"))));
        using var activity = new Activity("test-op").Start();

        // Act
        var result = await task.WithActivity(activity);

        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("failure", activity.GetTagItem("result.outcome") as string);
        Assert.AreEqual(ActivityStatusCode.Error, activity.Status);
        Assert.AreEqual("Error", activity.GetTagItem("result.error.type") as string);
        Assert.AreEqual("db timeout", activity.GetTagItem("result.error.message") as string);
    }

    [TestMethod]
    public async Task WithActivity_NullActivity_ReturnsResultUnchangedNoException()
    {
        // Arrange
        var task = Task.FromResult(new Result<int>(42, new Success("ok")));

        // Act — must not throw
        var result = await task.WithActivity(null);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(42, result.Value);
    }

    [TestMethod]
    public async Task WithActivity_CancellationAlreadyRequested_Throws()
    {
        // Arrange
        var task = Task.FromResult(new Result<int>(42, new Success("ok")));
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        using var activity = new Activity("test-op").Start();

        // Act & Assert
        var thrown = false;
        try
        {
            await task.WithActivity(activity, cts.Token);
        }
        catch (OperationCanceledException)
        {
            thrown = true;
        }
        Assert.IsTrue(thrown, "Expected OperationCanceledException when token is already cancelled.");
    }
}
