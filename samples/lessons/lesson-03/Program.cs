// =============================================================================
// Lesson 3 — Ensure: Guard Conditions
//
// Ensure checks a business rule against the current value.
// If the rule fails, the result becomes Fail — no if/else needed.
// Multiple Ensure calls chain: first failure short-circuits.
// =============================================================================
using REslava.Result;
using REslava.Result.Extensions;

// [ResultFlow]
Result<Order> ValidateOrder(Order order) =>
    Result<Order>.Ok(order)
        .Ensure(o => o.Amount > 0,       "Amount must be positive")
        .Ensure(o => o.Amount < 10_000,  "Amount exceeds limit")
        .Ensure(o => o.UserId > 0,       "Invalid user ID");

void Print(string label, Result<Order> result) =>
    Console.WriteLine(result.IsSuccess
        ? $"  {label}: OK — ${result.Value.Amount:F2}"
        : $"  {label}: FAIL — {result.Errors.First().Message}");

Console.WriteLine("> Guard conditions");
Print("Valid order     ", ValidateOrder(new Order(1, 42, 99.99m)));
Print("Negative amount ", ValidateOrder(new Order(2, 42, -5m)));
Print("Amount too large", ValidateOrder(new Order(3, 42, 15_000m)));
Print("Bad user ID     ", ValidateOrder(new Order(4, -1, 50m)));

Console.WriteLine("""

  Key rule:
  - Ensure(predicate, message) — stays Ok if predicate passes; becomes Fail if not
  - First failure short-circuits: subsequent Ensure calls are skipped
  - No if/else, no early return — the pipeline expresses the rules declaratively

  See Lesson 4 for Bind — chaining operations that can themselves fail.
""");

record Order(int Id, int UserId, decimal Amount);
