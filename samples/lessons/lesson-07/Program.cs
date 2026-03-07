// =============================================================================
// Lesson 7 — Async Pipelines
//
// The same Map / Bind / Ensure / Match pattern — with async operations.
// *Async variants accept and return Task<Result<T>>.
// Always thread CancellationToken through every async step.
// =============================================================================
using REslava.Result;
using REslava.Result.Extensions;

// --- Async operations that simulate I/O ---
async Task<Result<User>> FindUserAsync(int id, CancellationToken ct)
{
    await Task.Delay(10, ct);
    if (id <= 0)   return Result<User>.Fail(new ValidationError("id", "Must be positive"));
    if (id > 1000) return Result<User>.Fail(new NotFoundError($"User {id} not found"));
    return Result<User>.Ok(new User(id, "alice@example.com", "Admin"));
}

async Task<Result<Product>> FindProductAsync(int productId, CancellationToken ct)
{
    await Task.Delay(10, ct);
    if (productId != 7) return Result<Product>.Fail(new NotFoundError($"Product {productId} not found"));
    return Result<Product>.Ok(new Product(7, "Widget", 29.99m, 100));
}

async Task<Result<Order>> SaveOrderAsync(Order order, CancellationToken ct)
{
    await Task.Delay(10, ct);
    return Result<Order>.Ok(order with { Id = 4242 });
}

// --- Async pipeline — identical shape to the sync version ---
// [ResultFlow]
async Task<Result<Order>> PlaceOrderAsync(int userId, int productId, CancellationToken ct) =>
    await FindUserAsync(userId, ct)
        .BindAsync(user    => FindProductAsync(productId, ct), ct)
        .EnsureAsync(p     => p.Stock > 0, "Product out of stock", ct)
        .MapAsync(product  => new Order(0, userId, product.Price), ct)
        .BindAsync(order   => SaveOrderAsync(order, ct), ct);

using var cts = new CancellationTokenSource();

Console.WriteLine("> Async pipeline");

var ok = await PlaceOrderAsync(42, 7, cts.Token);
ok.Match(
    onSuccess: o  => Console.WriteLine($"  Success: Order #{o.Id} — ${o.Amount:F2}"),
    onFailure: es => Console.WriteLine($"  Failed:  {es.First().Message}"));

var fail = await PlaceOrderAsync(9999, 7, cts.Token);
fail.Match(
    onSuccess: o  => Console.WriteLine($"  Success: Order #{o.Id}"),
    onFailure: es => Console.WriteLine($"  Failed:  {es.First().Message}"));

Console.WriteLine("""

  Key rule:
  - *Async methods (BindAsync, MapAsync, EnsureAsync…) accept Task<Result<T>> or return Task<Result<T>>
  - Always pass CancellationToken through every step — do not ignore it
  - The pipeline shape is identical to sync — only the types change

  See Lesson 8 for ASP.NET — mapping Result<T> to HTTP responses automatically.
""");

record User(int Id, string Email, string Role);
record Order(int Id, int UserId, decimal Amount);
record Product(int Id, string Name, decimal Price, int Stock);
