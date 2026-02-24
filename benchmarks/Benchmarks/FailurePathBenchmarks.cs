using BenchmarkDotNet.Attributes;
using REslava.Result;
using REslava.Result.Extensions;

namespace REslava.Result.Benchmarks;

/// <summary>
/// The most important benchmark: failure handling.
/// Result has no stack unwinding — dramatically faster than throw/catch on failure paths.
/// Expected: Result_Failure ~50–100× faster with near-zero allocations.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
public class FailurePathBenchmarks
{
    private const int InvalidAge = -1;
    private const int ValidAge = 25;

    // -------------------------------------------------------------------------
    // Failure scenario (most common comparison argument for Result libraries)
    // -------------------------------------------------------------------------

    [Benchmark(Baseline = true)]
    public int Exception_Failure()
    {
        try
        {
            return ValidateWithException(InvalidAge);
        }
        catch
        {
            return -1;
        }
    }

    [Benchmark]
    public int Result_Failure()
    {
        var result = Result<int>.Ok(InvalidAge)
            .Ensure(age => age >= 0, "Age must be non-negative")
            .Ensure(age => age <= 150, "Age is unrealistic");
        return result.IsSuccess ? result.Value : -1;
    }

    [Benchmark]
    public int FluentResults_Failure()
    {
        var result = FluentResults.Result.Ok(InvalidAge)
            .Bind(age => age >= 0
                ? FluentResults.Result.Ok(age)
                : FluentResults.Result.Fail("Age must be non-negative"));
        return result.IsSuccess ? result.Value : -1;
    }

    // -------------------------------------------------------------------------
    // Success scenario (same code paths, no failure)
    // -------------------------------------------------------------------------

    [Benchmark]
    public int Exception_Success()
    {
        try
        {
            return ValidateWithException(ValidAge);
        }
        catch
        {
            return -1;
        }
    }

    [Benchmark]
    public int Result_Success()
    {
        var result = Result<int>.Ok(ValidAge)
            .Ensure(age => age >= 0, "Age must be non-negative")
            .Ensure(age => age <= 150, "Age is unrealistic");
        return result.IsSuccess ? result.Value : -1;
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static int ValidateWithException(int age)
    {
        if (age < 0) throw new ArgumentOutOfRangeException(nameof(age), "Age must be non-negative");
        if (age > 150) throw new ArgumentOutOfRangeException(nameof(age), "Age is unrealistic");
        return age;
    }
}
