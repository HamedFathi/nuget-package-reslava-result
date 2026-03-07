// =============================================================================
// Lesson 4 — Bind: Chain Operations That Can Fail
//
// Bind is like Map, but the function itself returns a Result.
// Use Bind when the next step can independently succeed or fail.
// Any step that returns Fail stops the chain — later steps are skipped.
// =============================================================================
using REslava.Result;
using REslava.Result.Extensions;

// --- Three independent operations, each can fail ---
Result<User> FindUser(int id) =>
    id > 0 && id <= 1000
        ? Result<User>.Ok(new User(id, "alice@example.com", "Admin"))
        : Result<User>.Fail(new Error($"User {id} not found"));

Result<Order> CheckCredit(User user, decimal amount) =>
    amount <= 500
        ? Result<Order>.Ok(new Order(1, user.Id, amount))
        : Result<Order>.Fail(new Error($"Credit limit exceeded (max 500, requested {amount})"));

Result<Order> SaveOrder(Order order) =>
    Result<Order>.Ok(order with { Id = new Random().Next(1000, 9999) });

// --- Bind chains them — any failure stops the pipeline ---
// [ResultFlow]
Result<Order> PlaceOrder(int userId, decimal amount) =>
    FindUser(userId)
        .Bind(user  => CheckCredit(user, amount))
        .Bind(order => SaveOrder(order));

void Print(string label, Result<Order> r) =>
    Console.WriteLine(r.IsSuccess
        ? $"  {label}: Order #{r.Value.Id} — ${r.Value.Amount:F2}"
        : $"  {label}: FAIL — {r.Errors.First().Message}");

Console.WriteLine("> Bind chains");
Print("All steps pass    ", PlaceOrder(42, 99.99m));
Print("User not found    ", PlaceOrder(9999, 99.99m));
Print("Credit exceeded   ", PlaceOrder(42, 999m));

Console.WriteLine("""

  Key rule:
  - Bind(fn) — fn returns Result<TOut>; failure at any step stops the chain
  - Map when the step cannot fail; Bind when it can
  - The pipeline reads like a to-do list — each line is one operation

  See Lesson 5 for Match — consuming the final result exhaustively.
""");

record User(int Id, string Email, string Role);
record Order(int Id, int UserId, decimal Amount);
