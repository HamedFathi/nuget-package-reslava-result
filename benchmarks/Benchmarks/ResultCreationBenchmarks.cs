using BenchmarkDotNet.Attributes;
using REslava.Result;

namespace REslava.Result.Benchmarks;

/// <summary>
/// Measures the raw instantiation cost of creating Ok and Fail results,
/// compared against FluentResults.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
public class ResultCreationBenchmarks
{
    [Benchmark(Baseline = true)]
    public Result<int> Ok() => Result<int>.Ok(42);

    [Benchmark]
    public Result<int> Fail() => Result<int>.Fail(new Error("something went wrong"));

    [Benchmark]
    public FluentResults.Result<int> FluentResults_Ok() =>
        FluentResults.Result.Ok(42);

    [Benchmark]
    public FluentResults.Result<int> FluentResults_Fail() =>
        FluentResults.Result.Fail("something went wrong");
}
