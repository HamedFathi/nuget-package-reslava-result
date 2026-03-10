# REslava.Result v1.39.0

Domain-driven typed error pipelines: `ErrorsOf<T1..T8>` error union + `Result<TValue, TError>` + full pipeline API (`Bind`×7, `Map`, `Tap`, `Ensure`×7+Async, `MapError`). Exact typed failure edges in `Result.Flow` Mermaid diagrams. `OneOf` becomes a `sealed class` for full code-sharing with `ErrorsOf` via `OneOfBase`.

---

## ⚠️ Breaking Change — `OneOf<T1..T8>`: `readonly struct` → `sealed class`

All `OneOf` types are now reference types. The public API is identical; only the memory model changes.

| What changes | Practical impact | Mitigated by |
|---|---|---|
| Copy semantics → reference semantics | Extremely rare to depend on | Code review |
| `default(OneOf<T1,T2>)` → `null` | Code using `default` as sentinel | `<Nullable>enable</Nullable>` already on — compiler flags every unsafe callsite |
| `where T : struct` no longer satisfied | Generic code with `T : struct` | Rare — `OneOf` almost never used in struct-constrained positions |

**Rationale**: `OneOfBase<T1..T8>` (a new abstract class) holds all shared dispatch. Both `OneOf<>` and `ErrorsOf<>` inherit it — the logic is written exactly once. Struct types cannot inherit from classes, so the conversion was necessary.

---

## ✨ `OneOf<T1..T7>` and `OneOf<T1..T8>` — New Arities

Two new arities for full T2–T8 symmetry alongside `ErrorsOf<T1..T8>`.

---

## ✨ `OneOfBase<T1..T8>` — Shared Dispatch

New `internal`-facing abstract class holding all shared logic: `IsT1..T8`, `AsT1..T8`, `Match`, `Switch`, `Equals`, `GetHashCode`, `ToString`. Both `OneOf` and `ErrorsOf` inherit it — dispatch code written once, zero duplication.

---

## ✨ `IOneOf<T1..T8>` — Shared Interface

New interface implemented by both `OneOf<T1,T2>` and `ErrorsOf<T1,T2>`. Enables generic programming over any discriminated union regardless of whether it is a value union or an error union.

---

## ✨ `ErrorsOf<T1..T8>` — Typed Error Union

A discriminated union over error types. Inherits `OneOfBase` (same dispatch as `OneOf`) and additionally implements `IError` — so it is itself a valid error usable as `TError` in `Result<TValue, TError>`.

```csharp
// Implicit conversions — no explicit wrapping needed
ErrorsOf<ValidationError, InventoryError> err = new ValidationError("Amount required");
ErrorsOf<ValidationError, InventoryError> err2 = new InventoryError("Out of stock");

// IError — delegates to active case
Console.WriteLine(err.Message);  // "Amount required"

// Exhaustive Match
string message = err.Match(
    v => v.Message,
    i => i.Message);

// IsT1..T8 / AsT1..T8
if (err.IsT2) { var inv = err.AsT2; /* InventoryError */ }
```

Constraints: `where T1 : IError where T2 : IError ...` — all slots must implement `IError`.

---

## ✨ `Result<TValue, TError>` — Typed Result

A typed result where the error is a concrete, compile-time-known type instead of `IEnumerable<IError>`.

```csharp
// Factory
Result<Order, ValidationError>.Ok(order);
Result<Order, ValidationError>.Fail(new ValidationError("Amount required"));

// Accessors
result.IsSuccess   // bool
result.IsFailure   // bool
result.Value       // TValue — throws InvalidOperationException on failure
result.Error       // TError — throws InvalidOperationException on success
```

---

## ✨ Typed Pipeline — `Bind` ×7

Each `Bind` call grows the error union by exactly one slot. The normalization trick (each step returns a single-error `Result<TOut, Ti>`, implicit conversion widens it) keeps the overload count O(n), not combinatorial.

```csharp
// Steps declare single concrete errors — clean signatures
Result<Order, ValidationError> Validate(CheckoutRequest req) => ...
Result<Order, InventoryError>  ReserveInventory(Order order) => ...
Result<Order, PaymentError>    ProcessPayment(Order order)   => ...
Result<Order, DatabaseError>   CreateOrder(Order order)      => ...

// Pipeline — error union grows: E1 → E1,E2 → E1,E2,E3 → E1,E2,E3,E4
Result<Order, ErrorsOf<ValidationError, InventoryError, PaymentError, DatabaseError>>
Checkout(CheckoutRequest request) =>
    Validate(request)
        .Bind(ReserveInventory)
        .Bind(ProcessPayment)
        .Bind(CreateOrder);

// Callsite — exhaustive match, compile-time safe
result.Error.Match(
    v => HandleValidation(v),
    i => HandleInventory(i),
    p => HandlePayment(p),
    d => HandleDatabase(d));
```

---

## ✨ Typed Pipeline — `Map`, `Tap`, `TapOnFailure`

```csharp
// Map — transform value, error type unchanged
Result<OrderDto, ValidationError> dto = result.Map(order => new OrderDto(order));

// Tap — side effect on success, returns original result
result.Tap(order => logger.LogInformation("Order {Id} validated", order.Id));

// TapOnFailure — side effect on failure, returns original result
result.TapOnFailure(err => logger.LogWarning("Failed: {Msg}", err.Message));
```

---

## ✨ Typed Pipeline — `Ensure` ×7 + `EnsureAsync` ×7

Guard conditions that widen the error union by one slot when the predicate fails:

```csharp
Result<Order, ValidationError> Validate(req) => ...

// First Ensure: Result<Order, ValidationError> → Result<Order, ErrorsOf<ValidationError, CreditLimitError>>
var guarded = Validate(req)
    .Ensure(order => order.Amount > 0,  new CreditLimitError("Amount must be positive"))
    .Ensure(order => order.Amount < 10_000, new CreditLimitError("Amount exceeds limit"));
//   ^ second Ensure: ErrorsOf<ValidationError, CreditLimitError> → ErrorsOf<V, C, C>

// Async variant (predicate only is async)
var asyncGuarded = await result
    .EnsureAsync(order => CheckCreditAsync(order), new CreditLimitError("..."));
```

---

## ✨ Typed Pipeline — `MapError`

Translates the error surface at layer boundaries:

```csharp
// Collapse union into a single domain error
Result<Order, DomainError> adapted = result.MapError(e => e.Match(
    v => new DomainError(v.Message),
    i => new DomainError(i.Message),
    p => new DomainError(p.Message),
    d => new DomainError(d.Message)));
```

---

## ✨ `Result.Flow` — Type-Read Mode

When a `[ResultFlow]`-annotated method returns `Result<T, TError>`, failure edges in the generated Mermaid diagram now show the exact error type. No body scanning — reads directly from the Roslyn return type symbol (`TypeArguments[1]`).

**Before (body-scan mode for `Result<T>`):**
```
N0_Bind["Bind<br/>Order → Order"]:::transform
N0_Bind -->|fail| F0["Failure"]:::failure
```

**After (type-read mode for `Result<T, ErrorsOf<...>>`):**
```
N0_Bind["Bind<br/>Order → Order"]:::transform
N0_Bind -->|"fail: ErrorsOf&lt;ValidationError, InventoryError&gt;"| F0["Failure"]:::failure
```

---

## 📦 NuGet

| Package | Link |
|---------|------|
| REslava.Result | [View on NuGet](https://www.nuget.org/packages/REslava.Result/1.39.0) |
| REslava.Result.AspNetCore | [View on NuGet](https://www.nuget.org/packages/REslava.Result.AspNetCore/1.39.0) |
| REslava.Result.Analyzers | [View on NuGet](https://www.nuget.org/packages/REslava.Result.Analyzers/1.39.0) |
| REslava.Result.FluentValidation | [View on NuGet](https://www.nuget.org/packages/REslava.Result.FluentValidation/1.39.0) |
| REslava.Result.Http | [View on NuGet](https://www.nuget.org/packages/REslava.Result.Http/1.39.0) |
| REslava.ResultFlow | [View on NuGet](https://www.nuget.org/packages/REslava.ResultFlow/1.39.0) |
| REslava.Result.Flow | [View on NuGet](https://www.nuget.org/packages/REslava.Result.Flow/1.39.0) |

---

## Stats

- 4,198 tests passing across net8.0, net9.0, net10.0 (1,280×3) + generator AspNetCore (131) + Result.Flow (22) + ResultFlow (40) + analyzer (79) + FluentValidation bridge (26) + Http (20×3)
- 153 features across 13 categories
