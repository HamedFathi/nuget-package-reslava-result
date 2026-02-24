using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REslava.Result.Analyzers.Analyzers;
using REslava.Result.Analyzers.Tests.Helpers;

namespace REslava.Result.Analyzers.Tests.Analyzers;

[TestClass]
public class RESL1005_SuggestDomainErrorTests
{
    private static Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<SuggestDomainErrorAnalyzer, DefaultVerifier>
        CreateTest(string testCode)
    {
        var test = AnalyzerTestHelper.CreateAnalyzerTest<SuggestDomainErrorAnalyzer>(
            testCode, AnalyzerTestHelper.ErrorStubSource);
        return test;
    }

    // ── Positive cases ──────────────────────────────────────────────────────

    [TestMethod]
    public async Task NotFoundMessage_SuggestsNotFoundError()
    {
        var test = CreateTest(@"
using REslava.Result;
class C
{
    IError M() => {|RESL1005:new Error(""item not found"")|};
}");
        await test.RunAsync();
    }

    [TestMethod]
    public async Task MissingMessage_SuggestsNotFoundError()
    {
        var test = CreateTest(@"
using REslava.Result;
class C
{
    IError M() => {|RESL1005:new Error(""user missing"")|};
}");
        await test.RunAsync();
    }

    [TestMethod]
    public async Task ConflictMessage_SuggestsConflictError()
    {
        var test = CreateTest(@"
using REslava.Result;
class C
{
    IError M() => {|RESL1005:new Error(""email conflict"")|};
}");
        await test.RunAsync();
    }

    [TestMethod]
    public async Task AlreadyExistsMessage_SuggestsConflictError()
    {
        var test = CreateTest(@"
using REslava.Result;
class C
{
    IError M() => {|RESL1005:new Error(""record already exists"")|};
}");
        await test.RunAsync();
    }

    [TestMethod]
    public async Task UnauthorizedMessage_SuggestsUnauthorizedError()
    {
        var test = CreateTest(@"
using REslava.Result;
class C
{
    IError M() => {|RESL1005:new Error(""user is unauthorized"")|};
}");
        await test.RunAsync();
    }

    [TestMethod]
    public async Task ForbiddenMessage_SuggestsForbiddenError()
    {
        var test = CreateTest(@"
using REslava.Result;
class C
{
    IError M() => {|RESL1005:new Error(""forbidden operation"")|};
}");
        await test.RunAsync();
    }

    [TestMethod]
    public async Task AccessDeniedMessage_SuggestsForbiddenError()
    {
        var test = CreateTest(@"
using REslava.Result;
class C
{
    IError M() => {|RESL1005:new Error(""access denied"")|};
}");
        await test.RunAsync();
    }

    [TestMethod]
    public async Task InvalidMessage_SuggestsValidationError()
    {
        var test = CreateTest(@"
using REslava.Result;
class C
{
    IError M() => {|RESL1005:new Error(""invalid email address"")|};
}");
        await test.RunAsync();
    }

    [TestMethod]
    public async Task ValidationMessage_SuggestsValidationError()
    {
        var test = CreateTest(@"
using REslava.Result;
class C
{
    IError M() => {|RESL1005:new Error(""validation failed"")|};
}");
        await test.RunAsync();
    }

    // ── Negative cases ──────────────────────────────────────────────────────

    [TestMethod]
    public async Task GenericMessage_NoReport()
    {
        var test = CreateTest(@"
using REslava.Result;
class C
{
    IError M() => new Error(""something went wrong"");
}");
        await test.RunAsync();
    }

    [TestMethod]
    public async Task NotFoundError_NoReport()
    {
        var test = CreateTest(@"
using REslava.Result;
class C
{
    IError M() => new NotFoundError(""item not found"");
}");
        await test.RunAsync();
    }

    [TestMethod]
    public async Task ValidationError_NoReport()
    {
        var test = CreateTest(@"
using REslava.Result;
class C
{
    IError M() => new ValidationError(""invalid input"");
}");
        await test.RunAsync();
    }

    [TestMethod]
    public async Task ErrorWithNoArguments_NoReport()
    {
        // Error() with no args won't match any keyword — should not crash
        var test = CreateTest(@"
using REslava.Result;
class C
{
    IError M() => new Error(""generic error"");
}");
        await test.RunAsync();
    }

    [TestMethod]
    public async Task ErrorWithInterpolatedString_NoReport()
    {
        // Non-literal first argument — analyzer must not crash
        var test = CreateTest(@"
using REslava.Result;
class C
{
    IError M(string msg) => new Error(msg);
}");
        await test.RunAsync();
    }
}
