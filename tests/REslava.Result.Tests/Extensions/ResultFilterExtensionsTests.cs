using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REslava.Result;
using REslava.Result.Extensions;

namespace REslava.Result.Tests.Extensions;

[TestClass]
public sealed class ResultFilterExtensionsTests
{
    private static readonly CancellationToken CancelledToken =
        new CancellationToken(canceled: true);

    #region Filter — success path (predicate passes)

    [TestMethod]
    public void Filter_SuccessResult_PredicatePasses_ReturnsUnchanged()
    {
        // Arrange
        var result = Result<int>.Ok(42);

        // Act
        var filtered = result.Filter(x => x > 0, x => new Error($"{x} must be positive"));

        // Assert
        Assert.IsTrue(filtered.IsSuccess);
        Assert.AreEqual(42, filtered.Value);
    }

    #endregion

    #region Filter — success path (predicate fails)

    [TestMethod]
    public void Filter_SuccessResult_PredicateFails_ReturnsFailureWithValueInMessage()
    {
        // Arrange
        var result = Result<int>.Ok(-5);

        // Act
        var filtered = result.Filter(x => x > 0, x => new Error($"Value {x} is not positive"));

        // Assert
        Assert.IsTrue(filtered.IsFailure);
        Assert.HasCount(1, filtered.Errors);
        Assert.AreEqual("Value -5 is not positive", filtered.Errors[0].Message);
    }

    [TestMethod]
    public void Filter_SuccessResult_PredicateFails_ErrorFactoryReceivesValue()
    {
        // Arrange
        var result = Result<string>.Ok("hello");
        string? capturedValue = null;

        // Act
        result.Filter(s => s.Length > 10, s => { capturedValue = s; return new Error("too short"); });

        // Assert
        Assert.AreEqual("hello", capturedValue);
    }

    #endregion

    #region Filter — failure pass-through

    [TestMethod]
    public void Filter_FailureResult_PredicateNotCalled_FailurePropagated()
    {
        // Arrange
        var result = Result<int>.Fail("already failed");
        var called = false;

        // Act
        var filtered = result.Filter(x => { called = true; return x > 0; }, _ => new Error("factory"));

        // Assert
        Assert.IsFalse(called);
        Assert.IsTrue(filtered.IsFailure);
        Assert.AreEqual("already failed", filtered.Errors[0].Message);
    }

    #endregion

    #region Filter — predicate throws

    [TestMethod]
    public void Filter_PredicateThrows_ReturnsExceptionError()
    {
        // Arrange
        var result = Result<int>.Ok(42);

        // Act
        var filtered = result.Filter<int>(
            _ => throw new InvalidOperationException("predicate boom"),
            _ => new Error("unreachable"));

        // Assert
        Assert.IsTrue(filtered.IsFailure);
        Assert.HasCount(1, filtered.Errors);
        Assert.IsInstanceOfType<ExceptionError>(filtered.Errors[0]);
        var exError = (ExceptionError)filtered.Errors[0];
        Assert.IsInstanceOfType<InvalidOperationException>(exError.Exception);
    }

    #endregion

    #region Filter — null guards

    [TestMethod]
    public void Filter_NullPredicate_ThrowsArgumentNullException()
    {
        var result = Result<int>.Ok(1);
        Assert.ThrowsExactly<ArgumentNullException>(
            () => result.Filter(null!, _ => new Error("x")));
    }

    [TestMethod]
    public void Filter_NullErrorFactory_ThrowsArgumentNullException()
    {
        var result = Result<int>.Ok(1);
        Assert.ThrowsExactly<ArgumentNullException>(
            () => result.Filter(_ => true, (Func<int, IError>)null!));
    }

    #endregion

    #region Filter — static error overload

    [TestMethod]
    public void Filter_StaticError_PredicateFails_ReturnsProvidedError()
    {
        // Arrange
        var result = Result<int>.Ok(-1);
        var staticError = new Error("must be positive");

        // Act
        var filtered = result.Filter(x => x > 0, staticError);

        // Assert
        Assert.IsTrue(filtered.IsFailure);
        Assert.AreSame(staticError, filtered.Errors[0]);
    }

    #endregion

    #region Filter — string overload

    [TestMethod]
    public void Filter_StringError_PredicateFails_ReturnsErrorWithMessage()
    {
        // Arrange
        var result = Result<int>.Ok(-1);

        // Act
        var filtered = result.Filter(x => x > 0, "value must be positive");

        // Assert
        Assert.IsTrue(filtered.IsFailure);
        Assert.AreEqual("value must be positive", filtered.Errors[0].Message);
    }

    #endregion

    #region FilterAsync — async predicate

    [TestMethod]
    public async Task FilterAsync_SuccessResult_AsyncPredicatePasses_ReturnsUnchanged()
    {
        // Arrange
        var result = Result<int>.Ok(42);

        // Act
        var filtered = await result.FilterAsync(
            async x => { await Task.CompletedTask; return x > 0; },
            x => new Error($"{x} must be positive"));

        // Assert
        Assert.IsTrue(filtered.IsSuccess);
        Assert.AreEqual(42, filtered.Value);
    }

    [TestMethod]
    public async Task FilterAsync_SuccessResult_AsyncPredicateFails_ReturnsFailure()
    {
        // Arrange
        var result = Result<int>.Ok(-1);

        // Act
        var filtered = await result.FilterAsync(
            async x => { await Task.CompletedTask; return x > 0; },
            x => new Error($"{x} is not positive"));

        // Assert
        Assert.IsTrue(filtered.IsFailure);
        Assert.AreEqual("-1 is not positive", filtered.Errors[0].Message);
    }

    [TestMethod]
    public async Task FilterAsync_FailureResult_PredicateNotCalled()
    {
        // Arrange
        var result = Result<int>.Fail("already failed");
        var called = false;

        // Act
        var filtered = await result.FilterAsync(
            async x => { called = true; await Task.CompletedTask; return x > 0; },
            _ => new Error("factory"));

        // Assert
        Assert.IsFalse(called);
        Assert.IsTrue(filtered.IsFailure);
        Assert.AreEqual("already failed", filtered.Errors[0].Message);
    }

    [TestMethod]
    public void FilterAsync_CancellationAlreadyRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var result = Result<int>.Ok(42);

        // Act & Assert
        Assert.ThrowsExactly<OperationCanceledException>(
            () => result.FilterAsync(
                async x => { await Task.CompletedTask; return x > 0; },
                _ => new Error("x"),
                CancelledToken).GetAwaiter().GetResult());
    }

    #endregion

    #region Task extensions — Filter

    [TestMethod]
    public async Task Filter_TaskSuccessResult_PredicatePasses_ReturnsUnchanged()
    {
        // Arrange
        var resultTask = Task.FromResult(Result<int>.Ok(10));

        // Act
        var filtered = await resultTask.Filter(x => x > 0, x => new Error($"{x}"));

        // Assert
        Assert.IsTrue(filtered.IsSuccess);
        Assert.AreEqual(10, filtered.Value);
    }

    [TestMethod]
    public async Task Filter_TaskSuccessResult_PredicateFails_ReturnsFailure()
    {
        // Arrange
        var resultTask = Task.FromResult(Result<int>.Ok(-3));

        // Act
        var filtered = await resultTask.Filter(x => x > 0, x => new Error($"{x} not positive"));

        // Assert
        Assert.IsTrue(filtered.IsFailure);
        Assert.AreEqual("-3 not positive", filtered.Errors[0].Message);
    }

    [TestMethod]
    public void Filter_Task_CancellationAlreadyRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var resultTask = Task.FromResult(Result<int>.Ok(1));

        // Act & Assert
        Assert.ThrowsExactly<OperationCanceledException>(
            () => resultTask.Filter(x => x > 0, _ => new Error("x"), CancelledToken)
                            .GetAwaiter().GetResult());
    }

    #endregion

    #region Task extensions — FilterAsync

    [TestMethod]
    public async Task FilterAsync_TaskSuccessResult_AsyncPredicateFails_ReturnsFailure()
    {
        // Arrange
        var resultTask = Task.FromResult(Result<int>.Ok(-7));

        // Act
        var filtered = await resultTask.FilterAsync(
            async x => { await Task.CompletedTask; return x > 0; },
            x => new Error($"{x} not positive"));

        // Assert
        Assert.IsTrue(filtered.IsFailure);
        Assert.AreEqual("-7 not positive", filtered.Errors[0].Message);
    }

    [TestMethod]
    public void FilterAsync_Task_CancellationAlreadyRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var resultTask = Task.FromResult(Result<int>.Ok(1));

        // Act & Assert
        Assert.ThrowsExactly<OperationCanceledException>(
            () => resultTask.FilterAsync(
                async x => { await Task.CompletedTask; return x > 0; },
                _ => new Error("x"),
                CancelledToken).GetAwaiter().GetResult());
    }

    #endregion
}
