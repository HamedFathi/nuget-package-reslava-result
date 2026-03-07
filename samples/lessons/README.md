# REslava.Result — Lesson Series

Progressive lessons teaching functional programming and railway-oriented programming through REslava.Result.
Each lesson is a standalone runnable console project (~40 lines).

## Lessons

| # | Topic | Concepts |
|---|-------|----------|
| [00](lesson-00/README.md) | The Problem | Traditional nullable returns + exceptions |
| [01](lesson-01/README.md) | Your First Result | `Ok` / `Fail` · `IsSuccess` · `Value` · `Errors` |
| [02](lesson-02/README.md) | Map | Transform without unwrapping · two-track model |
| [03](lesson-03/README.md) | Ensure | Guard conditions · short-circuit on first failure |
| [04](lesson-04/README.md) | Bind | Chain fallible operations · Map vs Bind |
| [05](lesson-05/README.md) | Match | Exhaustive terminal · both branches required |
| [06](lesson-06/README.md) | Domain Errors | `ValidationError` · `NotFoundError` · type-switch |
| [07](lesson-07/README.md) | Async Pipelines | `BindAsync` · `MapAsync` · `CancellationToken` |
| [08](lesson-08/README.md) | ASP.NET | SmartEndpoints · zero-boilerplate HTTP mapping |

## Run any lesson

```bash
cd lesson-01
dotnet run
```

## Shared domain

All lessons use the same three records:

```csharp
record User(int Id, string Email, string Role);
record Order(int Id, int UserId, decimal Amount);
record Product(int Id, string Name, decimal Price, int Stock);
```
