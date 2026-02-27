using System;
using System.Collections.Immutable;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REslava.Result;
using REslava.Result.Extensions;

namespace REslava.Result.Tests.Results;

[TestClass]
public sealed class ResultCatchTests
{
    #region Instance method — Catch<TException>

    [TestMethod]
    public void Catch_SuccessResult_ReturnsUnchanged()
    {
        // Arrange
        var result = new Result<int>(42, new Success("ok"));

        // Act
        var caught = result.Catch<InvalidOperationException>(_ => new Error("should not run"));

        // Assert
        Assert.IsTrue(caught.IsSuccess);
        Assert.AreEqual(42, caught.Value);
        Assert.HasCount(1, caught.Successes);
    }

    [TestMethod]
    public void Catch_FailureWithMatchingExceptionError_ReplacesError()
    {
        // Arrange
        var ex = new InvalidOperationException("boom");
        var result = new Result<int>(default, ImmutableList.Create<IReason>(new ExceptionError(ex)));

        // Act
        var caught = result.Catch<InvalidOperationException>(_ => new Error("converted"));

        // Assert
        Assert.IsTrue(caught.IsFailure);
        Assert.HasCount(1, caught.Errors);
        Assert.IsInstanceOfType<Error>(caught.Errors[0]);
        Assert.AreEqual("converted", caught.Errors[0].Message);
    }

    [TestMethod]
    public void Catch_HandlerReceivesOriginalException()
    {
        // Arrange
        var original = new InvalidOperationException("original message");
        var result = new Result<int>(default, ImmutableList.Create<IReason>(new ExceptionError(original)));
        InvalidOperationException? received = null;

        // Act
        result.Catch<InvalidOperationException>(ex => { received = ex; return new Error("x"); });

        // Assert
        Assert.IsNotNull(received);
        Assert.AreSame(original, received);
    }

    [TestMethod]
    public void Catch_FailureWithDifferentExceptionType_ReturnsUnchanged()
    {
        // Arrange
        var result = new Result<int>(default, ImmutableList.Create<IReason>(
            new ExceptionError(new InvalidOperationException("wrong type"))));

        // Act
        var caught = result.Catch<ArgumentException>(_ => new Error("should not run"));

        // Assert
        Assert.IsTrue(caught.IsFailure);
        Assert.IsInstanceOfType<ExceptionError>(caught.Errors[0]);
    }

    [TestMethod]
    public void Catch_FailureWithNonExceptionError_ReturnsUnchanged()
    {
        // Arrange
        var result = new Result<int>(default, ImmutableList.Create<IReason>(new Error("plain error")));

        // Act
        var caught = result.Catch<InvalidOperationException>(_ => new Error("should not run"));

        // Assert
        Assert.IsTrue(caught.IsFailure);
        Assert.HasCount(1, caught.Errors);
        Assert.AreEqual("plain error", caught.Errors[0].Message);
    }

    [TestMethod]
    public void Catch_MultipleErrors_OnlyMatchingExceptionErrorReplaced()
    {
        // Arrange
        var matching = new ExceptionError(new InvalidOperationException("matching"));
        var other = new Error("unrelated");
        var result = new Result<int>(default, ImmutableList.Create<IReason>(matching, other));

        // Act
        var caught = result.Catch<InvalidOperationException>(_ => new Error("replaced"));

        // Assert
        Assert.IsTrue(caught.IsFailure);
        Assert.HasCount(2, caught.Errors);
        Assert.AreEqual("replaced", caught.Errors[0].Message);
        Assert.AreEqual("unrelated", caught.Errors[1].Message);
    }

    [TestMethod]
    public void Catch_SubclassException_MatchedByBaseType()
    {
        // Arrange — FileNotFoundException is a subclass of IOException
        var result = new Result<int>(default, ImmutableList.Create<IReason>(
            new ExceptionError(new FileNotFoundException("file missing"))));

        // Act
        var caught = result.Catch<IOException>(_ => new Error("io converted"));

        // Assert
        Assert.IsTrue(caught.IsFailure);
        Assert.AreEqual("io converted", caught.Errors[0].Message);
    }

    [TestMethod]
    public void Catch_SuccessReasonsPreservedOnMatchingFailure()
    {
        // Arrange — a result that has both a success reason and an exception error
        var reasons = ImmutableList.Create<IReason>(
            new Success("step 1"),
            new ExceptionError(new InvalidOperationException("boom")));
        var result = new Result<int>(default, reasons);

        // Act
        var caught = result.Catch<InvalidOperationException>(_ => new Error("converted"));

        // Assert
        Assert.IsTrue(caught.IsFailure);
        Assert.HasCount(1, caught.Successes);
        Assert.HasCount(1, caught.Errors);
        Assert.AreEqual("converted", caught.Errors[0].Message);
    }

    #endregion

    #region Instance method — CatchAsync<TException>

    [TestMethod]
    public async Task CatchAsync_SuccessResult_ReturnsUnchanged()
    {
        var result = new Result<int>(42, new Success("ok"));

        var caught = await result.CatchAsync<InvalidOperationException>(
            _ => Task.FromResult<IError>(new Error("x")));

        Assert.IsTrue(caught.IsSuccess);
        Assert.AreEqual(42, caught.Value);
    }

    [TestMethod]
    public async Task CatchAsync_FailureWithMatchingException_ReplacesError()
    {
        var result = new Result<int>(default, ImmutableList.Create<IReason>(
            new ExceptionError(new InvalidOperationException("boom"))));

        var caught = await result.CatchAsync<InvalidOperationException>(
            _ => Task.FromResult<IError>(new Error("async converted")));

        Assert.IsTrue(caught.IsFailure);
        Assert.AreEqual("async converted", caught.Errors[0].Message);
    }

    #endregion

    #region Task<Result<T>> extensions — Catch<T, TException>

    [TestMethod]
    public async Task CatchExtension_SuccessResult_ReturnsUnchanged()
    {
        var task = Task.FromResult(new Result<int>(42, new Success("ok")));

        var caught = await task.Catch<int, InvalidOperationException>(_ => new Error("x"));

        Assert.IsTrue(caught.IsSuccess);
        Assert.AreEqual(42, caught.Value);
    }

    [TestMethod]
    public async Task CatchExtension_FailureWithMatchingExceptionError_ReplacesError()
    {
        var task = Task.FromResult(
            new Result<int>(default, ImmutableList.Create<IReason>(
                new ExceptionError(new InvalidOperationException("boom")))));

        var caught = await task.Catch<int, InvalidOperationException>(_ => new Error("converted"));

        Assert.IsTrue(caught.IsFailure);
        Assert.AreEqual("converted", caught.Errors[0].Message);
    }

    [TestMethod]
    public async Task CatchExtension_TaskThrowsMatchingException_CatchesAndReturnsFailure()
    {
        // Arrange — task throws directly, no Result.Try wrapping
        static async Task<Result<int>> ThrowingTask()
        {
            await Task.Yield();
            throw new InvalidOperationException("direct throw");
        }

        // Act
        var caught = await ThrowingTask().Catch<int, InvalidOperationException>(
            ex => new Error($"caught: {ex.Message}"));

        // Assert
        Assert.IsTrue(caught.IsFailure);
        Assert.AreEqual("caught: direct throw", caught.Errors[0].Message);
    }

    [TestMethod]
    public async Task CatchExtension_TaskThrowsDifferentException_Propagates()
    {
        // Arrange
        static async Task<Result<int>> ThrowingTask()
        {
            await Task.Yield();
            throw new ArgumentException("different exception");
        }

        // Act & Assert — different exception type should propagate
        var propagated = false;
        try
        {
            await ThrowingTask().Catch<int, InvalidOperationException>(_ => new Error("x"));
        }
        catch (ArgumentException)
        {
            propagated = true;
        }
        Assert.IsTrue(propagated, "ArgumentException should propagate when Catch targets a different type.");
    }

    [TestMethod]
    public async Task CatchExtension_FailureWithDifferentExceptionType_ReturnsUnchanged()
    {
        var task = Task.FromResult(
            new Result<int>(default, ImmutableList.Create<IReason>(
                new ExceptionError(new InvalidOperationException("wrong type")))));

        var caught = await task.Catch<int, ArgumentException>(_ => new Error("x"));

        Assert.IsTrue(caught.IsFailure);
        Assert.IsInstanceOfType<ExceptionError>(caught.Errors[0]);
    }

    #endregion

    #region Task<Result<T>> extensions — CatchAsync<T, TException>

    [TestMethod]
    public async Task CatchAsyncExtension_FailureWithMatchingException_ReplacesErrorViaAsyncHandler()
    {
        var task = Task.FromResult(
            new Result<int>(default, ImmutableList.Create<IReason>(
                new ExceptionError(new InvalidOperationException("boom")))));

        var caught = await task.CatchAsync<int, InvalidOperationException>(
            async ex =>
            {
                await Task.Yield();
                return new Error($"async: {ex.Message}");
            });

        Assert.IsTrue(caught.IsFailure);
        Assert.AreEqual("async: boom", caught.Errors[0].Message);
    }

    [TestMethod]
    public async Task CatchAsyncExtension_TaskThrowsMatchingException_CatchesViaAsyncHandler()
    {
        static async Task<Result<int>> ThrowingTask()
        {
            await Task.Yield();
            throw new InvalidOperationException("direct throw");
        }

        var caught = await ThrowingTask().CatchAsync<int, InvalidOperationException>(
            async ex =>
            {
                await Task.Yield();
                return new Error($"async caught: {ex.Message}");
            });

        Assert.IsTrue(caught.IsFailure);
        Assert.AreEqual("async caught: direct throw", caught.Errors[0].Message);
    }

    #endregion
}
