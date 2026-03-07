# Lesson 8 — ASP.NET: Result&lt;T&gt; to HTTP

**Goal**: Map a `Result<T>` pipeline to an HTTP response — no `TypedResults`, no manual status code mapping.

**Concepts**: `ToIResult()` · domain error HTTP status tags · Minimal API integration

## Run

```bash
dotnet run
```

Then test with curl or your browser:

```bash
# Success — 200 OK
curl -X POST http://localhost:5000/orders \
  -H "Content-Type: application/json" \
  -d '{"userId": 42, "amount": 99.99}'

# Not found — 404
curl -X POST http://localhost:5000/orders \
  -H "Content-Type: application/json" \
  -d '{"userId": 999, "amount": 50}'

# Validation error — 400 ProblemDetails
curl -X POST http://localhost:5000/orders \
  -H "Content-Type: application/json" \
  -d '{"userId": 42, "amount": -5}'
```

## How it works

Domain errors carry their HTTP status code in `.Tags` — set automatically by their constructor:

| Error type | HTTP status |
|------------|-------------|
| `ValidationError` | 400 Bad Request |
| `NotFoundError` | 404 Not Found |
| `ForbiddenError` | 403 Forbidden |
| `ConflictError` | 409 Conflict |

`ToIResult()` reads that tag and returns the correct `IResult`:

```csharp
app.MapPost("/orders", async (CreateOrderRequest req, OrderService svc, CancellationToken ct) =>
    (await svc.PlaceOrderAsync(req, ct)).ToIResult());
```

The service method is pure business logic — it knows nothing about HTTP:

```csharp
public async Task<Result<Order>> PlaceOrderAsync(CreateOrderRequest req, CancellationToken ct)
{
    if (!_users.TryGetValue(req.UserId, out var user))
        return Result<Order>.Fail(new NotFoundError($"User {req.UserId} not found"));
    // ...
}
```

## Previous

[← Lesson 7 — Async Pipelines](../lesson-07/README.md)
