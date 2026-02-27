# v1.30.0 — Exception Handling + OpenTelemetry

## `Result.Catch<TException>()` — Inline Exception Handling

Handle typed exceptions directly inside the railway pipeline — no try/catch at the call site:

```csharp
Result<User> result = await Result.TryAsync(() => httpClient.GetUserAsync(id))
    .Catch<User, HttpRequestException>(ex => new NotFoundError("User", id));
```

Both cases are covered automatically:
- The result contains an `ExceptionError` wrapping `TException` → replaced in-place with the handler's error
- The source task throws `TException` directly → caught and converted

The replacement preserves the error's original position in the reasons list (important when a result carries multiple errors).

```csharp
// Async handler variant
Result<Order> result = await service.PlaceOrderAsync(request)
    .CatchAsync<Order, DbException>(async ex =>
    {
        await alertService.NotifyAsync(ex.Message);
        return new ConflictError("Order", "duplicate");
    });
```

## `Result.WithActivity(Activity?)` — OTel Span Enrichment

Enrich an existing OpenTelemetry `Activity` span with result outcome metadata — one line, zero boilerplate:

```csharp
using var activity = ActivitySource.StartActivity("GetUser");

Result<User> result = await service.GetUser(id)
    .WithActivity(activity);

// Tags set automatically on the span:
// result.outcome      = "failure"
// result.error.type   = "NotFoundError"
// result.error.message = "User '42' was not found."
// Activity.Status     = ActivityStatusCode.Error
```

Tap-style — the result is returned unchanged, safe to insert anywhere in a pipeline. Passing `Activity.Current` (which may be null) is always safe — it's a no-op when there's no active span.

**No new NuGet dependency** — `System.Diagnostics.Activity` is part of the .NET 8/9/10 BCL.

| Tag | Success | Failure |
|-----|---------|---------|
| `result.outcome` | `"success"` | `"failure"` |
| `result.error.type` | — | First error type name |
| `result.error.message` | — | First error message |
| `result.error.count` | — | Set when > 1 error |
| `Activity.Status` | `Ok` | `Error` |

## Test Suite

- 3,432 tests passing across net8.0, net9.0, net10.0 (1,069×3) + generator (131) + analyzer (68) + FluentValidation bridge (26)

## NuGet Packages

- [View on NuGet — REslava.Result 1.30.0](https://www.nuget.org/packages/REslava.Result/1.30.0)
- [View on NuGet — REslava.Result.SourceGenerators 1.30.0](https://www.nuget.org/packages/REslava.Result.SourceGenerators/1.30.0)
- [View on NuGet — REslava.Result.Analyzers 1.30.0](https://www.nuget.org/packages/REslava.Result.Analyzers/1.30.0)
- [View on NuGet — REslava.Result.FluentValidation 1.30.0](https://www.nuget.org/packages/REslava.Result.FluentValidation/1.30.0)
