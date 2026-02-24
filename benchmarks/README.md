# REslava.Result Benchmarks

BenchmarkDotNet benchmarks for `REslava.Result` — measures allocation cost, hot-path overhead,
and failure-path performance vs exception-based error handling.

## Scenarios

| Class | What it measures |
|-------|-----------------|
| `ResultCreationBenchmarks` | Raw `Ok`/`Fail` instantiation cost vs FluentResults |
| `FailurePathBenchmarks` | Result vs `throw`/`catch` on both failure and success paths |
| `SuccessPathBenchmarks` | Hot-path overhead: `Map`, `Ensure`, `Match` |
| `RailwayChainBenchmarks` | 3-step and 5-step pipeline chains |
| `WhenAllBenchmarks` | `Combine` (sync) and `WhenAll` (async) aggregation |

## Running

Requires .NET 9 SDK. Run from the repo root:

```bash
# All scenarios
cd benchmarks
dotnet run -c Release -- --exporters github markdown

# Single class (faster during development)
dotnet run -c Release -- --filter *FailurePath* --exporters github markdown
dotnet run -c Release -- --filter *ResultCreation* --exporters github markdown
```

Results land in `benchmarks/BenchmarkDotNet.Artifacts/results/` (gitignored).

## Publishing Results

After a full run, copy the `*-report-github.md` output to
`mkdocs/core-concepts/advanced-patterns/performance-benchmarks.md` and commit.
The MkDocs pipeline picks it up automatically.

## Notes

- Always run with `-c Release` — Debug builds are not representative.
- Close other applications before a full run for consistent numbers.
- The most important benchmark is `FailurePathBenchmarks` — it shows that `Result`
  has near-zero allocation on failure vs `throw`/`catch` stack unwinding.
