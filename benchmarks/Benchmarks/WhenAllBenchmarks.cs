using BenchmarkDotNet.Attributes;
using REslava.Result;

namespace REslava.Result.Benchmarks;

/// <summary>
/// Measures aggregating multiple results into one.
/// Combine: synchronous aggregation.
/// WhenAll: async aggregation (concurrent tasks).
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
public class WhenAllBenchmarks
{
    private Result<int>[] _allSuccess = null!;
    private Result<int>[] _withFailures = null!;
    private Task<Result<int>>[] _allSuccessAsync = null!;
    private Task<Result<int>>[] _withFailuresAsync = null!;

    [GlobalSetup]
    public void Setup()
    {
        _allSuccess = Enumerable.Range(1, 10)
            .Select(i => Result<int>.Ok(i))
            .ToArray();

        _withFailures = Enumerable.Range(1, 10)
            .Select(i => i % 3 == 0
                ? Result<int>.Fail(new Error($"Step {i} failed"))
                : Result<int>.Ok(i))
            .ToArray();

        _allSuccessAsync = _allSuccess.Select(Task.FromResult).ToArray();
        _withFailuresAsync = _withFailures.Select(Task.FromResult).ToArray();
    }

    // -------------------------------------------------------------------------
    // Synchronous aggregation via Combine
    // -------------------------------------------------------------------------

    [Benchmark(Baseline = true)]
    public Result<IEnumerable<int>> Combine_10_AllSuccess() =>
        Result<int>.Combine(_allSuccess);

    [Benchmark]
    public Result<IEnumerable<int>> Combine_10_WithFailures() =>
        Result<int>.Combine(_withFailures);

    // -------------------------------------------------------------------------
    // Async aggregation via WhenAll
    // -------------------------------------------------------------------------

    [Benchmark]
    public Task<Result<System.Collections.Immutable.ImmutableList<int>>> WhenAll_10_AllSuccess() =>
        Result.WhenAll(_allSuccessAsync);

    [Benchmark]
    public Task<Result<System.Collections.Immutable.ImmutableList<int>>> WhenAll_10_WithFailures() =>
        Result.WhenAll(_withFailuresAsync);
}
