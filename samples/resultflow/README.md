# ResultFlow — Pipeline Visualization Sample

> **ResultFlow is library-agnostic.**
> It works with [REslava.Result](https://www.nuget.org/packages/REslava.Result), [ErrorOr](https://github.com/amantinband/error-or), [LanguageExt](https://github.com/louthy/language-ext), or any fluent Result library.
> This sample uses REslava.Result — the diagrams are identical regardless of which library you choose.

---

## What is ResultFlow?

`REslava.ResultFlow` is a **Roslyn source generator** that walks your fluent pipeline chain at compile time and emits a **Mermaid flowchart** as a `const string` — zero runtime cost, zero manual maintenance.

```csharp
[ResultFlow]
public static Result<Order> PlaceOrder(int userId, decimal amount) =>
    FindUser(userId)
        .Bind(u  => CheckCredit(u, amount))
        .Bind(SaveOrder);
```

At compile time, the generator emits:

```csharp
// Generated.ResultFlow.Pipelines_Flows.PlaceOrder
flowchart LR
    N0_Bind["Bind"]:::transform
    N0_Bind -->|ok| N1_Bind
    N0_Bind -->|fail| F0["Failure"]:::failure
    N1_Bind["Bind"]:::transform
    N1_Bind -->|fail| F1["Failure"]:::failure
```

Paste into [mermaid.live](https://mermaid.live) to visualize the pipeline instantly.

---

## Node colours

| Colour | Role | Methods |
|--------|------|---------|
| Lavender | **Gatekeeper** | `Ensure`, `EnsureAsync`, `Filter` |
| Mint | **Transform** | `Bind`, `BindAsync`, `Map`, `MapAsync`, `Or`, `OrElse` |
| Vanilla | **Side effect** | `Tap`, `TapAsync`, `TapOnFailure`, `TapBoth`, `MapError` |
| Pink | **Failure** | failure exit branches |
| White | **Terminal** | `Match`, `MatchAsync`, `Switch` |

---

## Run

```bash
dotnet run
```

Output: all 6 pipeline diagrams printed to the terminal. Copy any diagram into [mermaid.live](https://mermaid.live).

## What this sample covers

| Pipeline | Methods | Demonstrates |
|----------|---------|--------------|
| `ValidateOrder` | `Ensure` × 3 | Guard chain — first failure short-circuits |
| `PlaceOrder` | `Bind` × 2 | Risk chain — each step can independently fail |
| `ProcessCheckout` | `Ensure` + `Bind` + `Tap` + `TapOnFailure` + `TapBoth` + `Map` | All node kinds in one pipeline |
| `PlaceOrderAsync` | `BindAsync` + `EnsureAsync` + `MapAsync` + `BindAsync` | Async — identical shape to sync |
| `WithFallback` | `Or` + `Map` | Recovery — returns a fallback when upstream fails |
| `TranslateErrors` | `Bind` × 2 + `MapError` | Error translation — transforms errors without changing state |

---

## Library-agnostic: side-by-side

The **same diagram** is generated regardless of which library you use. Only the method names in the chain differ:

```csharp
// REslava.Result
[ResultFlow]
Result<Order> PlaceOrder(int userId, decimal amount) =>
    FindUser(userId)
        .Bind(u => CheckCredit(u, amount));

// ErrorOr — same diagram, different method name
[ResultFlow]
ErrorOr<Order> PlaceOrder(int userId, decimal amount) =>
    FindUser(userId)
        .Then(u => CheckCredit(u, amount));   // "Then" → TransformWithRisk (same colour)
```

The built-in convention dictionary covers **REslava.Result**, **ErrorOr**, and **LanguageExt** out of the box. Use `resultflow.json` to add custom library method names.

---

## How the constant is accessed

```csharp
using Generated.ResultFlow;

// Class: Pipelines  →  Generated class: Pipelines_Flows
// Method: PlaceOrder →  Constant:       Pipelines_Flows.PlaceOrder

Console.WriteLine(Pipelines_Flows.PlaceOrder);
```

The constant is a `const string` — it is embedded in the assembly at compile time and has **zero runtime overhead**.

---

## Install

```bash
dotnet add package REslava.ResultFlow
```

No extra `using` for the attribute — just add `using REslava.ResultFlow;` and annotate any fluent method with `[ResultFlow]`.
