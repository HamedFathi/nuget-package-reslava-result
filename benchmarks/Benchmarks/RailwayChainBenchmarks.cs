using BenchmarkDotNet.Attributes;
using REslava.Result;
using REslava.Result.Extensions;

namespace REslava.Result.Benchmarks;

/// <summary>
/// Measures the cost of chaining operations (railway-oriented programming).
/// Simulates a realistic service method: validate → load → transform → respond.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
public class RailwayChainBenchmarks
{
    private const int OrderId = 42;

    // -------------------------------------------------------------------------
    // 3-step Map chain
    // -------------------------------------------------------------------------

    [Benchmark(Baseline = true)]
    public Result<string> Chain_3_Map() =>
        Result<int>.Ok(OrderId)
            .Ensure(id => id > 0, "Invalid order ID")
            .Map(id => $"ORDER-{id:D6}")
            .Map(code => code.ToUpperInvariant());

    // -------------------------------------------------------------------------
    // 5-step chain with both Map and Bind
    // -------------------------------------------------------------------------

    [Benchmark]
    public Result<OrderSummary> Chain_5_Steps() =>
        Result<int>.Ok(OrderId)
            .Ensure(id => id > 0, "Invalid order ID")
            .Map(id => new OrderRequest(id, 99.99m))
            .Ensure(req => req.Amount > 0, "Amount must be positive")
            .Bind(req => ApplyDiscount(req))
            .Map(req => new OrderSummary(req.Id, req.Amount, $"ORDER-{req.Id:D6}"));

    // -------------------------------------------------------------------------
    // 5-step Bind chain (each step returns Result)
    // -------------------------------------------------------------------------

    [Benchmark]
    public Result<string> Chain_5_Bind() =>
        Result<int>.Ok(OrderId)
            .Bind(id => id > 0
                ? Result<int>.Ok(id)
                : Result<int>.Fail(new Error("Invalid order ID")))
            .Bind(id => id < 1_000_000
                ? Result<decimal>.Ok(id * 1.5m)
                : Result<decimal>.Fail(new Error("Order total too large")))
            .Bind(total => total > 0
                ? Result<string>.Ok($"ORDER-{OrderId:D6}:${total:F2}")
                : Result<string>.Fail(new Error("Calculation error")))
            .Bind(summary => Result<string>.Ok(summary.Trim()));

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static Result<OrderRequest> ApplyDiscount(OrderRequest req)
    {
        var discounted = req.Amount > 50m ? req with { Amount = req.Amount * 0.9m } : req;
        return Result<OrderRequest>.Ok(discounted);
    }
}

public record OrderRequest(int Id, decimal Amount);
public record OrderSummary(int Id, decimal Amount, string Code);
