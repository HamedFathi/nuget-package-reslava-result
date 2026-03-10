---
title: Performance Benchmarks
description: BenchmarkDotNet results for REslava.Result — creation cost, railway chains, failure handling, and async aggregation.
---

# Performance Benchmarks

Real measurements produced by [BenchmarkDotNet v0.14.0](https://benchmarkdotnet.org/) on the reference machine below.
All numbers are **per single operation** on .NET 9.

```
BenchmarkDotNet v0.14.0, Windows 11 (10.0.26200.7840)
AMD Ryzen 5 4600G with Radeon Graphics, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.103 — [Host] / DefaultJob: .NET 9.0.13 (9.0.1326.6317), X64 RyuJIT AVX2
```

---

## Key Takeaways

| Finding | Number |
|---------|--------|
| `Result<T>.Ok(value)` creation cost | **5.9 ns / 48 B** |
| **9.6x faster** Ok creation vs FluentResults | 5.9 ns vs 57 ns |
| **20% faster** Fail creation vs FluentResults | 212 ns vs 266 ns |
| Result-based failure path vs exceptions | **6.8x faster** (511 ns vs 3,490 ns) |
| 5-step railway chain (Map + Bind) | ~840 ns / 1,528 B |
| 10-result async aggregation (`WhenAll`, all success) | 697 ns / 1,600 B |

---

## 1. Result Creation

Raw cost of instantiating `Ok` and `Fail` results, compared against FluentResults.

| Method | Mean | Error | StdDev | Ratio | Gen0 | Allocated | Alloc Ratio |
|--- |---:|---:|---:|---:|---:|---:|---:|
| `Ok` *(baseline)* | 5.930 ns | 0.170 ns | 0.175 ns | 1.00 | 0.0229 | 48 B | 1.00 |
| `Fail` | 211.812 ns | 2.117 ns | 1.653 ns | 35.75 | 0.2141 | 448 B | 9.33 |
| `FluentResults_Ok` | 57.030 ns | 1.120 ns | 0.993 ns | 9.63 | 0.0535 | 112 B | 2.33 |
| `FluentResults_Fail` | 266.023 ns | 5.107 ns | 3.987 ns | 44.90 | 0.2484 | 520 B | 10.83 |

**Notes**

- `Ok` is a struct-backed value with an immutable list ref — near-zero overhead.
- `Fail` allocates an `ImmutableList<IError>` and `ImmutableList<ISuccess>`, which accounts for the higher cost. This is a one-time cost at result construction.
- REslava.Result `Ok` is **9.6x faster** and uses **57% less memory** than FluentResults `Ok`.
- REslava.Result `Fail` is **20% faster** and uses **14% less memory** than FluentResults `Fail`.

---

## 2. Success Path — Fluent Chaining

Cost of a single fluent operation on an already-created `Ok` result, including a FluentResults comparison.

!!! note
    The baseline (`DirectCall`) is a raw method call that compiles to a handful of instructions (~0.017 ns). Because it is effectively JIT noise, the **Ratio** column is not meaningful here and is omitted. Focus on **Mean** and **Allocated**.

| Method | Mean | Error | StdDev | Gen0 | Allocated |
|--- |---:|---:|---:|---:|---:|
| `DirectCall` *(baseline — near zero)* | 0.017 ns | 0.003 ns | 0.003 ns | — | — |
| `Result_Map` | 277.98 ns | 5.59 ns | 13.82 ns | 0.2637 | 552 B |
| `Result_Ensure_Map` | 303.93 ns | 5.35 ns | 7.68 ns | 0.2637 | 552 B |
| `Result_Match` | 296.92 ns | 5.97 ns | 10.29 ns | 0.2637 | 552 B |
| `FluentResults_Map` | 215.35 ns | 4.34 ns | 7.13 ns | 0.1760 | 368 B |

**Notes**

- Each operation allocates a **new** `Result<T>` with its own immutable lists. The cost is dominated by that allocation.
- FluentResults uses mutable internal lists, which reduces allocation per operation — at the cost of mutability and thread-safety guarantees.
- REslava.Result trades ~60 ns / 184 B per step for **full immutability** and no accidental sharing between pipeline stages.

---

## 3. Failure Path — Exceptions vs Results

The classic argument for the Result pattern: exceptions are slow on the failure path.

| Method | Mean | Error | StdDev | Ratio | Gen0 | Allocated | Alloc Ratio |
|--- |---:|---:|---:|---:|---:|---:|---:|
| `Exception_Failure` *(baseline)* | 3,490.252 ns | 10.391 ns | 8.677 ns | 1.000 | 0.1602 | 336 B | 1.00 |
| `Result_Failure` | 511.529 ns | 3.371 ns | 2.815 ns | **0.147** | 0.4320 | 904 B | 2.69 |
| `FluentResults_Failure` | 596.979 ns | 10.067 ns | 8.924 ns | 0.171 | 0.4702 | 984 B | 2.93 |
| `Exception_Success` | 0.313 ns | 0.003 ns | 0.003 ns | 0.000 | — | — | 0.00 |
| `Result_Success` | 118.754 ns | 1.891 ns | 2.944 ns | 0.034 | 0.0956 | 200 B | 0.60 |

**Notes**

- `Result_Failure` is **6.8x faster** than throwing and catching an exception for the failure path.
- REslava.Result failure handling is **14% faster** than FluentResults.
- `Exception_Success` confirms why exceptions are cheap when *not thrown* — cost only hits when the exception is raised and caught.
- `Result_Success` allocates more per-operation than an exception-success path, but the failure-path savings dominate at realistic error rates.

---

## 4. Railway Chains

Cost of multi-step pipelines combining `Ensure`, `Map`, and `Bind` operations in sequence.

| Method | Mean | Error | StdDev | Ratio | RatioSD | Gen0 | Allocated | Alloc Ratio |
|--- |---:|---:|---:|---:|---:|---:|---:|---:|
| `Chain_3_Map` *(baseline)* | 478.6 ns | 3.04 ns | 2.99 ns | 1.00 | 0.01 | 0.3824 | 800 B | 1.00 |
| `Chain_5_Steps` | 767.4 ns | 15.23 ns | 20.33 ns | 1.60 | 0.04 | 0.6113 | 1,280 B | 1.60 |
| `Chain_5_Bind` | 839.1 ns | 11.50 ns | 9.60 ns | 1.75 | 0.02 | 0.7305 | 1,528 B | 1.91 |

**Benchmark definitions**

| Name | Steps |
|------|-------|
| `Chain_3_Map` | `Ok` → `Ensure` → `Map` → `Map` |
| `Chain_5_Steps` | `Ok` → `Ensure` → `Map` → `Ensure` → `Bind` → `Map` |
| `Chain_5_Bind` | `Ok` → `Bind` → `Bind` → `Bind` → `Bind` (4 Bind steps each returning `Result`) |

**Notes**

- Pipeline cost scales **linearly** with step count — each additional step adds ~80–120 ns and ~240 B.
- A realistic 5-step service pipeline (`Chain_5_Steps`) runs in **~767 ns** — well under any I/O-bound operation threshold.
- `Bind` steps cost slightly more than `Map` steps because each `Bind` callback returns a new `Result<T>` that must be unwrapped.

---

## 5. Result Aggregation — Combine & WhenAll

Cost of combining 10 results into one, both synchronously (`Combine`) and concurrently (`WhenAll`).

| Method | Mean | Error | StdDev | Ratio | RatioSD | Gen0 | Allocated | Alloc Ratio |
|--- |---:|---:|---:|---:|---:|---:|---:|---:|
| `Combine_10_AllSuccess` *(baseline)* | 508.9 ns | 9.88 ns | 14.16 ns | 1.00 | 0.04 | 0.2365 | 496 B | 1.00 |
| `Combine_10_WithFailures` | 1,110.9 ns | 22.21 ns | 33.92 ns | 2.18 | 0.09 | 0.6847 | 1,432 B | 2.89 |
| `WhenAll_10_AllSuccess` | 696.6 ns | 11.20 ns | 9.93 ns | 1.37 | 0.04 | 0.7648 | 1,600 B | 3.23 |
| `WhenAll_10_WithFailures` | 1,585.1 ns | 31.37 ns | 55.76 ns | 3.12 | 0.14 | 0.9403 | 1,968 B | 3.97 |

**Benchmark definitions**

| Name | Description |
|------|-------------|
| `Combine_10_AllSuccess` | `Result<int>.Combine(array)` — 10 pre-built Ok results |
| `Combine_10_WithFailures` | `Result<int>.Combine(array)` — 10 results, 3 are Fail |
| `WhenAll_10_AllSuccess` | `Result.WhenAll(tasks)` — 10 `Task.FromResult(Ok)` tasks |
| `WhenAll_10_WithFailures` | `Result.WhenAll(tasks)` — 10 tasks, 3 are Fail |

**Notes**

- Failure cases cost more because errors must be **collected and merged** across all results — this is the price of accumulating all failures rather than short-circuiting at the first one.
- `WhenAll` uses `Task.WhenAll` internally and then aggregates. The extra overhead vs `Combine` (~190 ns) is the async machinery.
- All values are well within the noise range of a single database roundtrip (~1–10 ms).

---

## Design Rationale — Why ImmutableList?

Every `Result<T>` in REslava.Result stores its errors and successes in `System.Collections.Immutable.ImmutableList<T>`. This is a deliberate design decision — not a performance oversight.

### What ImmutableList costs

Each fluent operation (`Map`, `Bind`, `Ensure`, `Tap`, …) returns a **brand-new** `Result<T>` with its own immutable lists. It never mutates the original. The allocation cost per step is roughly 60 ns / 184 B more than an equivalent mutable design (see the Success Path benchmark above, where REslava.Result costs ~280–300 ns per step vs FluentResults ~215 ns).

### What you get in return

**Thread safety by construction.** You can pass a `Result<T>` across threads, store it in a field, cache it, or share it between pipeline branches without any locking, defensive copying, or fear of one branch mutating what another branch is reading.

**No accidental sharing bugs.** With a mutable result, doing `var shared = result; shared.Errors.Add(...)` silently corrupts the original. With an immutable result, that code does not compile. The type system enforces the contract.

**Referential transparency.** Each step in a pipeline returns a value. There are no hidden side-effects. This makes the code easy to reason about, test, and refactor — the same input always produces the same output, regardless of call order.

**Safe accumulation.** `Combine`, `WhenAll`, and `Validate` merge multiple error lists into one result. With immutable lists, merging is a pure operation — each source list is unchanged, and the combined list is a new allocation. No defensive copies needed.

### When the tradeoff matters

The 30–40% per-step overhead only matters if you have a **tight hot loop** performing thousands of Result operations per millisecond with **no I/O**. That is not the intended use case. REslava.Result is designed for application-layer error handling — validation pipelines, service method chains, HTTP handler flows — where each operation is I/O-bound and the allocations are negligible noise.

If you are processing millions of items per second in a computational pipeline without I/O, consider using `Span<T>` and struct-based value types instead of a Result library.

### Comparison summary

| Property | REslava.Result (`ImmutableList`) | FluentResults (mutable list) |
|---|---|---|
| Thread-safe sharing | Yes | No |
| Accidental mutation | Impossible | Possible |
| Per-step allocation | ~280–300 ns / 552 B | ~215 ns / 368 B |
| Failure-path speed | 511 ns | 597 ns |
| Ok creation | **5.9 ns / 48 B** | 57 ns / 112 B |

The Ok-creation advantage (9.6×) comes precisely from ImmutableList: an empty immutable list is a singleton (`ImmutableList<T>.Empty`) — no allocation. FluentResults allocates a new mutable `List<T>` for every result, even the happy-path ones.

---

## Methodology

- **Tool**: BenchmarkDotNet v0.14.0 with `[MemoryDiagnoser]` and `[SimpleJob]` (default job)
- **Warmup**: automatic BDN pilot and warmup iterations
- **Measurement**: automatic iteration count per benchmark (14–40 runs each)
- **Allocations**: managed heap only, inclusive; 1 KB = 1,024 B
- **FluentResults comparison**: requires `ConfigOptions.DisableOptimizationsValidator` because the FluentResults NuGet ships a non-optimized assembly — its numbers are a conservative lower bound

To reproduce locally:

```bash
dotnet run --project benchmarks/Benchmarks.csproj -c Release -- --filter "*"
```