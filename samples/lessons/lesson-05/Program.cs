// =============================================================================
// Lesson 5 — Match: Consume the Result
//
// Match is the terminal step — it forces you to handle both tracks.
// It always returns a value, so it can be used inline anywhere.
// The compiler will not let you forget the failure case.
// =============================================================================
using REslava.Result;

Result<User>  FindUser(int id)  => id > 0 && id <= 1000
    ? Result<User>.Ok(new User(id, "alice@example.com", "Admin"))
    : Result<User>.Fail(new Error($"User {id} not found"));

Result<Order> PlaceOrder(int userId, decimal amount) =>
    FindUser(userId).Bind(user =>
        amount <= 500
            ? Result<Order>.Ok(new Order(1, user.Id, amount))
            : Result<Order>.Fail(new Error("Credit limit exceeded")));

// --- Match: both branches must be handled, always returns a value ---
// [ResultFlow]
string Describe(int userId, decimal amount) =>
    PlaceOrder(userId, amount).Match(
        onSuccess: order  => $"Order #{order.Id} placed for ${order.Amount:F2}",
        onFailure: errors => $"Could not place order: {errors.First().Message}"
    );

Console.WriteLine("> Match — exhaustive");
Console.WriteLine($"  {Describe(42, 99.99m)}");
Console.WriteLine($"  {Describe(9999, 50m)}");
Console.WriteLine($"  {Describe(42, 999m)}");

// Match can also dispatch void actions
Console.WriteLine("\n> Match — void dispatch");
PlaceOrder(42, 49.99m).Match(
    onSuccess: order  => Console.WriteLine($"  Saved to DB: #{order.Id}"),
    onFailure: errors => Console.WriteLine($"  Logged error: {errors.First().Message}")
);

Console.WriteLine("""

  Key rule:
  - Match forces you to handle both success and failure — no way to ignore either
  - Returns a TOut value — use it inline in an expression or assignment
  - This is the natural END of a pipeline

  See Lesson 6 for Domain Errors — typed, rich, field-aware error objects.
""");

record User(int Id, string Email, string Role);
record Order(int Id, int UserId, decimal Amount);
