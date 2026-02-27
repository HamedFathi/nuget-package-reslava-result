using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REslava.Result;
using REslava.Result.Extensions;

namespace REslava.Result.Tests.Extensions;

[TestClass]
public sealed class ResultRecoverExtensionsTests
{
    private static readonly CancellationToken CancelledToken =
        new CancellationToken(canceled: true);

    #region Task.Recover (sync fallback)

    [TestMethod]
    public async Task Recover_TaskSuccessResult_ReturnsUnchangedWithoutCallingRecover()
    {
        // Arrange
        var resultTask = Task.FromResult(Result<int>.Ok(42));
        var called = false;

        // Act
        var recovered = await resultTask.Recover(errors => { called = true; return Result<int>.Fail("fallback"); });

        // Assert
        Assert.IsFalse(called);
        Assert.IsTrue(recovered.IsSuccess);
        Assert.AreEqual(42, recovered.Value);
    }

    [TestMethod]
    public async Task Recover_TaskFailureResult_RecoverReturnsSuccess()
    {
        // Arrange
        var resultTask = Task.FromResult(Result<int>.Fail("primary failure"));

        // Act
        var recovered = await resultTask.Recover(_ => Result<int>.Ok(99));

        // Assert
        Assert.IsTrue(recovered.IsSuccess);
        Assert.AreEqual(99, recovered.Value);
    }

    [TestMethod]
    public void Recover_CancellationAlreadyRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var resultTask = Task.FromResult(Result<int>.Ok(42));

        // Act & Assert
        Assert.ThrowsExactly<OperationCanceledException>(
            () => resultTask.Recover(_ => Result<int>.Ok(0), CancelledToken)
                            .GetAwaiter().GetResult());
    }

    #endregion

    #region Task.RecoverAsync (async fallback)

    [TestMethod]
    public async Task RecoverAsync_TaskSuccessResult_ReturnsUnchangedWithoutCallingRecover()
    {
        // Arrange
        var resultTask = Task.FromResult(Result<string>.Ok("hello"));
        var called = false;

        // Act
        var recovered = await resultTask.RecoverAsync(async _ =>
        {
            called = true;
            await Task.CompletedTask;
            return Result<string>.Fail("fallback");
        });

        // Assert
        Assert.IsFalse(called);
        Assert.IsTrue(recovered.IsSuccess);
        Assert.AreEqual("hello", recovered.Value);
    }

    [TestMethod]
    public async Task RecoverAsync_TaskFailureResult_RecoverReturnsSuccess()
    {
        // Arrange
        var resultTask = Task.FromResult(Result<string>.Fail("primary failure"));

        // Act
        var recovered = await resultTask.RecoverAsync(_ => Task.FromResult(Result<string>.Ok("fallback value")));

        // Assert
        Assert.IsTrue(recovered.IsSuccess);
        Assert.AreEqual("fallback value", recovered.Value);
    }

    [TestMethod]
    public void RecoverAsync_CancellationAlreadyRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var resultTask = Task.FromResult(Result<int>.Ok(42));

        // Act & Assert
        Assert.ThrowsExactly<OperationCanceledException>(
            () => resultTask.RecoverAsync(_ => Task.FromResult(Result<int>.Ok(0)), CancelledToken)
                            .GetAwaiter().GetResult());
    }

    #endregion
}
