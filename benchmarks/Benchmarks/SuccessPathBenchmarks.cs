using BenchmarkDotNet.Attributes;
using REslava.Result;
using REslava.Result.Extensions;

namespace REslava.Result.Benchmarks;

/// <summary>
/// Measures hot path overhead — how much does wrapping in Result cost on the success path?
/// Expected: ~5–20 ns per operation. Negligible for real business logic.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
public class SuccessPathBenchmarks
{
    private const int Value = 42;

    [Benchmark(Baseline = true)]
    public int DirectCall() => Value * 2;

    [Benchmark]
    public int Result_Map() =>
        Result<int>.Ok(Value)
            .Map(x => x * 2)
            .GetValueOr(0);

    [Benchmark]
    public int Result_Ensure_Map() =>
        Result<int>.Ok(Value)
            .Ensure(x => x > 0, "Must be positive")
            .Map(x => x * 2)
            .GetValueOr(0);

    [Benchmark]
    public int Result_Match() =>
        Result<int>.Ok(Value)
            .Map(x => x * 2)
            .Match(
                onSuccess: v => v,
                onFailure: _ => 0);

    [Benchmark]
    public int FluentResults_Map() =>
        FluentResults.Result.Ok(Value)
            .Map(x => x * 2)
            .ValueOrDefault;
}
