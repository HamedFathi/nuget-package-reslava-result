// =============================================================================
// Lesson 6 — Domain Errors: Be Specific
//
// Replace generic Error strings with typed domain errors.
// Each type carries its own meaning — no string-matching on messages.
// ValidationError adds FieldName so callers know which field failed.
// =============================================================================
using REslava.Result;
using REslava.Result.Extensions;

// --- Typed errors in factory methods ---
Result<User> FindUser(int id)
{
    if (id <= 0)     return Result<User>.Fail(new ValidationError("id", "Must be a positive number"));
    if (id > 1000)   return Result<User>.Fail(new NotFoundError($"User {id} does not exist"));
    return Result<User>.Ok(new User(id, "alice@example.com", "Admin"));
}

Result<Order> PlaceOrder(int userId, decimal amount) =>
    FindUser(userId)
        .Bind(u => u.Role == "Admin"
            ? Result<User>.Ok(u)
            : Result<User>.Fail(new ForbiddenError("Only admins can place orders")))
        .Bind(u =>
            amount > 0
                ? Result<Order>.Ok(new Order(1, u.Id, amount))
                : Result<Order>.Fail(new ValidationError("amount", "Must be greater than zero")));

// --- Type-switch on errors in Match ---
// [ResultFlow]
void PrintResult(string label, Result<Order> result) =>
    result.Match(
        onSuccess: o  => Console.WriteLine($"  {label}: Order #{o.Id} — ${o.Amount:F2}"),
        onFailure: errors =>
        {
            foreach (var e in errors)
            {
                var detail = e switch
                {
                    ValidationError ve => $"Validation — field '{ve.FieldName}': {ve.Message}",
                    NotFoundError      => $"Not found: {e.Message}",
                    ForbiddenError     => $"Forbidden: {e.Message}",
                    _                  => $"Error: {e.Message}"
                };
                Console.WriteLine($"  {label}: {detail}");
            }
        });

Console.WriteLine("> Domain errors");
PrintResult("Valid          ", PlaceOrder(42, 99.99m));
PrintResult("Bad user ID    ", PlaceOrder(-1, 99.99m));
PrintResult("User not found ", PlaceOrder(9999, 99.99m));
PrintResult("Bad amount     ", PlaceOrder(42, -5m));

Console.WriteLine("""

  Key rule:
  - ValidationError(fieldName, message) — carries the field name for form validation
  - NotFoundError / ForbiddenError / ConflictError — semantic HTTP-aware types
  - Type-switch in Match — no string matching on messages, no magic strings
  - IReason has only .Message + .Tags — cast to the typed error to access extra fields

  See Lesson 7 for async pipelines — same pattern, with await.
""");

record User(int Id, string Email, string Role);
record Order(int Id, int UserId, decimal Amount);
