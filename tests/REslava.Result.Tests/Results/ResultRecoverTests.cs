using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REslava.Result;

namespace REslava.Result.Tests.Results;

[TestClass]
public sealed class ResultRecoverTests
{
    private static readonly CancellationToken CancelledToken =
        new CancellationToken(canceled: true);

    #region Result<T>.Recover — success path

    [TestMethod]
    public void Recover_SuccessResult_ReturnsUnchangedWithoutCallingRecover()
    {
        // Arrange
        var result = Result<int>.Ok(42);
        var called = false;

        // Act
        var recovered = result.Recover(errors => { called = true; return Result<int>.Fail("fallback"); });

        // Assert
        Assert.IsFalse(called);
        Assert.IsTrue(recovered.IsSuccess);
        Assert.AreEqual(42, recovered.Value);
    }

    #endregion

    #region Result<T>.Recover — failure path

    [TestMethod]
    public void Recover_FailureResult_RecoverFuncCalledWithErrors()
    {
        // Arrange
        var result = Result<int>.Fail(new Error("primary failure"));
        ImmutableList<IError>? capturedErrors = null;

        // Act
        result.Recover(errors => { capturedErrors = errors; return Result<int>.Ok(0); });

        // Assert
        Assert.IsNotNull(capturedErrors);
        Assert.HasCount(1, capturedErrors);
        Assert.AreEqual("primary failure", capturedErrors[0].Message);
    }

    [TestMethod]
    public void Recover_FailureResult_RecoverReturnsSuccess_ResultIsSuccess()
    {
        // Arrange
        var result = Result<int>.Fail("primary failed");

        // Act
        var recovered = result.Recover(_ => Result<int>.Ok(99));

        // Assert
        Assert.IsTrue(recovered.IsSuccess);
        Assert.AreEqual(99, recovered.Value);
    }

    [TestMethod]
    public void Recover_FailureResult_RecoverReturnsDifferentFailure_CascadingFailure()
    {
        // Arrange
        var result = Result<int>.Fail("primary failure");

        // Act
        var recovered = result.Recover(_ => Result<int>.Fail(new Error("fallback also failed")));

        // Assert
        Assert.IsTrue(recovered.IsFailure);
        Assert.HasCount(1, recovered.Errors);
        Assert.AreEqual("fallback also failed", recovered.Errors[0].Message);
    }

    [TestMethod]
    public void Recover_NullFunc_ThrowsArgumentNullException()
    {
        // Arrange
        var result = Result<int>.Fail("error");

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(
            () => result.Recover(null!));
    }

    #endregion

    #region Result<T>.RecoverAsync

    [TestMethod]
    public async Task RecoverAsync_SuccessResult_ReturnsUnchangedWithoutCallingRecover()
    {
        // Arrange
        var result = Result<string>.Ok("hello");
        var called = false;

        // Act
        var recovered = await result.RecoverAsync(async _ =>
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
    public async Task RecoverAsync_FailureResult_RecoverReturnsSuccess()
    {
        // Arrange
        var result = Result<string>.Fail("primary failure");

        // Act
        var recovered = await result.RecoverAsync(_ => Task.FromResult(Result<string>.Ok("fallback value")));

        // Assert
        Assert.IsTrue(recovered.IsSuccess);
        Assert.AreEqual("fallback value", recovered.Value);
    }

    [TestMethod]
    public void RecoverAsync_CancellationAlreadyRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var result = Result<int>.Fail("error");

        // Act & Assert
        Assert.ThrowsExactly<OperationCanceledException>(
            () => result.RecoverAsync(_ => Task.FromResult(Result<int>.Ok(0)), CancelledToken)
                        .GetAwaiter().GetResult());
    }

    #endregion

    #region Non-generic Result.Recover

    [TestMethod]
    public void Recover_NonGeneric_SuccessResult_ReturnsUnchanged()
    {
        // Arrange
        var result = Result.Ok();
        var called = false;

        // Act
        var recovered = result.Recover(_ => { called = true; return Result.Fail("fallback"); });

        // Assert
        Assert.IsFalse(called);
        Assert.IsTrue(recovered.IsSuccess);
    }

    [TestMethod]
    public void Recover_NonGeneric_FailureResult_RecoverReturnsSuccess()
    {
        // Arrange
        var result = Result.Fail("primary failure");

        // Act
        var recovered = result.Recover(_ => Result.Ok());

        // Assert
        Assert.IsTrue(recovered.IsSuccess);
    }

    [TestMethod]
    public void Recover_NonGeneric_FailureResult_RecoverReturnsDifferentFailure()
    {
        // Arrange
        var result = Result.Fail("primary failure");

        // Act
        var recovered = result.Recover(_ => Result.Fail("fallback also failed"));

        // Assert
        Assert.IsTrue(recovered.IsFailure);
        Assert.AreEqual("fallback also failed", recovered.Errors[0].Message);
    }

    #endregion
}
