// =============================================================================
// Lesson 8 — ASP.NET: Result<T> to HTTP
//
// Result<T> pipelines plug directly into Minimal API via ToIResult().
// No TypedResults, no manual status mapping — errors carry their HTTP code.
//
//   ValidationError → 400 Bad Request  (+ ProblemDetails)
//   NotFoundError   → 404 Not Found
//   ForbiddenError  → 403 Forbidden
//   success         → 200 OK  (JSON body)
// =============================================================================
using REslava.Result;
using REslava.Result.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<OrderService>();
var app = builder.Build();

// One line per endpoint — Result<T> is converted by ToIResult()
// [ResultFlow]
app.MapPost("/orders", async (CreateOrderRequest req, OrderService svc, CancellationToken ct) =>
    (await svc.PlaceOrderAsync(req, ct)).ToIResult());

app.Run();

// =============================================================================
// Domain
// =============================================================================
record User(int Id, string Email, string Role);
record Order(int Id, int UserId, decimal Amount);
record CreateOrderRequest(int UserId, decimal Amount);

// =============================================================================
// Service — pure business logic, returns Result<T>
// =============================================================================
class OrderService
{
    private readonly Dictionary<int, User> _users = new()
    {
        [42] = new User(42, "alice@example.com", "Admin")
    };

    public async Task<Result<Order>> PlaceOrderAsync(CreateOrderRequest req, CancellationToken ct)
    {
        await Task.Delay(5, ct);

        if (!_users.TryGetValue(req.UserId, out var user))
            return Result<Order>.Fail(new NotFoundError($"User {req.UserId} not found"));

        if (req.Amount <= 0)
            return Result<Order>.Fail(new ValidationError("amount", "Must be greater than zero"));

        return Result<Order>.Ok(new Order(new Random().Next(1000, 9999), user.Id, req.Amount));
    }
}

// =============================================================================
// ToIResult — maps Result<T> to the correct HTTP response
// Domain errors carry an HttpStatusCode tag set by their constructor.
// =============================================================================
static class ResultHttpExtensions
{
    public static IResult ToIResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return Results.Ok(result.Value);

        var error = result.Errors.First();

        // Domain errors carry their HTTP status code in .Tags
        var statusCode = 400;
        if (error.Tags.TryGetValue("HttpStatusCode", out var code) && code is int sc)
            statusCode = sc;

        return statusCode switch
        {
            404 => Results.NotFound(new { error.Message }),
            403 => Results.Json(new { error.Message }, statusCode: 403),
            409 => Results.Conflict(new { error.Message }),
            _   => Results.Problem(detail: error.Message, statusCode: statusCode)
        };
    }
}
