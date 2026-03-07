// =============================================================================
// ResultFlow — Pipeline Visualization Sample
//
// REslava.ResultFlow is a SOURCE GENERATOR — library-agnostic.
// It works with REslava.Result, ErrorOr, LanguageExt, or any fluent Result library.
//
// Add [ResultFlow] to any fluent pipeline method.
// The generator walks the chain at compile time and emits a Mermaid diagram
// as a const string — zero runtime overhead, zero manual maintenance.
//
// This sample uses REslava.Result as the underlying library.
// The diagrams below are identical to what you would get with ErrorOr or LanguageExt
// — only the method names in the chain differ.
// =============================================================================
using Generated.ResultFlow;
using REslava.Result;
using REslava.Result.Extensions;
using REslava.ResultFlow;
using System.Collections.Immutable;

var sep  = new string('─', 60);
var sep2 = new string('═', 60);

Console.WriteLine(sep2);
Console.WriteLine("  ResultFlow — Compile-Time Pipeline Diagrams");
Console.WriteLine("  Library used here: REslava.Result");
Console.WriteLine("  ResultFlow itself has NO dependency on any Result library.");
Console.WriteLine(sep2);

void Print(string label, string diagram)
{
    Console.WriteLine();
    Console.WriteLine($"  {label}");
    Console.WriteLine(sep);
    Console.WriteLine(diagram);
}

// Each constant is generated at compile time from the [ResultFlow] attribute.
// Paste into https://mermaid.live to see the visual diagram.
Print("1. Guard chain — Ensure × 3",                  Pipelines_Flows.ValidateOrder);
Print("2. Risk chain — Bind × 2",                     Pipelines_Flows.PlaceOrder);
Print("3. Full pipeline — Ensure+Bind+Tap+Map",       Pipelines_Flows.ProcessCheckout);
Print("4. Async pipeline — *Async variants",           Pipelines_Flows.PlaceOrderAsync);
Print("5. Recovery — Or fallback",                     Pipelines_Flows.WithFallback);
Print("6. Error translation — MapError",               Pipelines_Flows.TranslateErrors);

Console.WriteLine();
Console.WriteLine(sep2);
Console.WriteLine("  Runtime verification — pipelines execute normally");
Console.WriteLine(sep2);

// The [ResultFlow] annotation is compile-time only — no runtime effect.
// The pipelines are plain C# methods that run and return Result<T> as usual.
void Run(string label, object result)
{
    var isSuccess = result.GetType().GetProperty("IsSuccess")?.GetValue(result) is true;
    Console.WriteLine($"  {label}: {(isSuccess ? "OK" : "FAIL")}");
}

using var cts = new CancellationTokenSource();
Run("ValidateOrder (valid)       ", Pipelines.ValidateOrder(new Order(1, 42, 99.99m)));
Run("ValidateOrder (bad amount)  ", Pipelines.ValidateOrder(new Order(2, 42, -5m)));
Run("PlaceOrder (success)        ", Pipelines.PlaceOrder(42, 99.99m));
Run("PlaceOrder (user not found) ", Pipelines.PlaceOrder(999, 50m));
Run("WithFallback (missing user) ", Pipelines.WithFallback(999));
Run("PlaceOrderAsync             ", Pipelines.PlaceOrderAsync(42, 7, cts.Token).GetAwaiter().GetResult());

Console.WriteLine();

// =============================================================================
// Domain records — same as the lesson series (User, Order, Product)
// =============================================================================
record User(int Id, string Email, string Role);
record Order(int Id, int UserId, decimal Amount);
record Product(int Id, string Name, decimal Price, int Stock);

// =============================================================================
// Pipelines — [ResultFlow] methods
//
// Each method is annotated with [ResultFlow].
// The source generator walks the fluent chain and emits:
//   Generated.ResultFlow.Pipelines_Flows.<MethodName>   (const string Mermaid)
//
// Node colours in the Mermaid output:
//   lavender  = Ensure / EnsureAsync    (Gatekeeper)
//   mint      = Bind / Map / Or         (TransformWithRisk / PureTransform)
//   vanilla   = Tap / TapBoth           (SideEffect)
//   pink      = TapOnFailure / MapError (SideEffectFailure)
//   terminal  = Match                   (Terminal)
// =============================================================================
static class Pipelines
{
    // ─── 1. Guard chain ─────────────────────────────────────────────────────
    // Three Ensure calls: first failure short-circuits the rest.
    // Diagram: Gatekeeper → Gatekeeper → Gatekeeper
    [ResultFlow]
    public static Result<Order> ValidateOrder(Order order) =>
        Result<Order>.Ok(order)
            .Ensure(o => o.Amount > 0,       "Amount must be positive")
            .Ensure(o => o.Amount < 10_000,  "Amount exceeds limit")
            .Ensure(o => o.UserId > 0,       "Invalid user ID");

    // ─── 2. Risk chain ──────────────────────────────────────────────────────
    // Two Bind calls: each step can independently fail and stop the chain.
    // Diagram: TransformWithRisk → TransformWithRisk
    [ResultFlow]
    public static Result<Order> PlaceOrder(int userId, decimal amount) =>
        FindUser(userId)
            .Bind(u  => CheckCredit(u, amount))
            .Bind(SaveOrder);

    // ─── 3. Full pipeline ───────────────────────────────────────────────────
    // Shows every node kind: Gatekeeper, TransformWithRisk, SideEffect×3, PureTransform.
    // Diagram: Gatekeeper → TransformWithRisk → SideEffect → SideEffectFailure
    //          → SideEffectBoth → PureTransform
    [ResultFlow]
    public static Result<string> ProcessCheckout(int userId, decimal amount) =>
        FindUser(userId)
            .Ensure(u => u.Role == "Admin",    "Admins only")
            .Bind(u  => CheckCredit(u, amount))
            .Tap(o   => Log($"Order #{o.Id} ready — ${o.Amount:F2}"))
            .TapOnFailure(e => Log($"Checkout error: {e.Message}"))
            .TapBoth(r => Log($"Pipeline complete ({(r.IsSuccess ? "success" : "failure")})"))
            .Map(o   => $"Confirmed: Order #{o.Id} — ${o.Amount:F2}");

    // ─── 4. Async pipeline ──────────────────────────────────────────────────
    // *Async variants: identical shape to sync — only the types change.
    // Diagram: TransformWithRisk → Gatekeeper → PureTransform → TransformWithRisk
    [ResultFlow]
    public static async Task<Result<Order>> PlaceOrderAsync(
        int userId, int productId, CancellationToken ct) =>
        await FindUserAsync(userId, ct)
            .BindAsync(u  => FindProductAsync(productId, ct), ct)
            .EnsureAsync(p => p.Stock > 0, "Product out of stock", ct)
            .MapAsync(p   => new Order(0, userId, p.Price), ct)
            .BindAsync(o  => SaveOrderAsync(o, ct), ct);

    // ─── 5. Recovery — Or fallback ──────────────────────────────────────────
    // Or returns a fallback Result<T> when the upstream fails.
    // Diagram: TransformWithRisk → PureTransform
    [ResultFlow]
    public static Result<string> WithFallback(int userId) =>
        FindUser(userId)
            .Or(Result<User>.Ok(new User(0, "guest@example.com", "Guest")))
            .Map(u => $"{u.Email} ({u.Role})");

    // ─── 6. Error translation — MapError ────────────────────────────────────
    // MapError fires only on the failure track: transforms errors without changing state.
    // Diagram: TransformWithRisk → TransformWithRisk → SideEffectFailure
    [ResultFlow]
    public static Result<Order> TranslateErrors(int userId, decimal amount) =>
        FindUser(userId)
            .Bind(u  => CheckCredit(u, amount))
            .Bind(SaveOrder)
            .MapError(errs => errs
                .Select(e => (IError)new ConflictError($"Order failed: {e.Message}"))
                .ToImmutableList());

    // ─── Helpers (no [ResultFlow] — not part of a pipeline diagram) ─────────

    private static readonly Dictionary<int, User> _users = new()
    {
        [42] = new User(42, "alice@example.com", "Admin"),
        [7]  = new User(7,  "bob@example.com",   "User")
    };
    private static readonly Dictionary<int, Product> _products = new()
    {
        [7]  = new Product(7, "Widget", 29.99m, 100)
    };

    private static Result<User> FindUser(int id) =>
        _users.TryGetValue(id, out var u)
            ? Result<User>.Ok(u)
            : Result<User>.Fail(new NotFoundError($"User {id} not found"));

    private static Result<Order> CheckCredit(User user, decimal amount) =>
        amount <= 500
            ? Result<Order>.Ok(new Order(0, user.Id, amount))
            : Result<Order>.Fail(new ValidationError("amount", "Credit limit exceeded (max 500)"));

    private static Result<Order> SaveOrder(Order order) =>
        Result<Order>.Ok(order with { Id = new Random().Next(1000, 9999) });

    private static async Task<Result<User>> FindUserAsync(int id, CancellationToken ct)
    {
        await Task.Delay(5, ct);
        return FindUser(id);
    }

    private static async Task<Result<Product>> FindProductAsync(int id, CancellationToken ct)
    {
        await Task.Delay(5, ct);
        return _products.TryGetValue(id, out var p)
            ? Result<Product>.Ok(p)
            : Result<Product>.Fail(new NotFoundError($"Product {id} not found"));
    }

    private static async Task<Result<Order>> SaveOrderAsync(Order order, CancellationToken ct)
    {
        await Task.Delay(5, ct);
        return SaveOrder(order);
    }

    private static void Log(string msg) => Console.WriteLine($"    [LOG] {msg}");
}
