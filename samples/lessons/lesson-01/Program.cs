// =============================================================================
// Lesson 1 — Your First Result
//
// Same scenario as Lesson 0, rewritten with Result<T>.
// One return type. All outcomes explicit. No exceptions for control flow.
// =============================================================================
using REslava.Result;

// --- All three outcomes now live in the return type ---
Result<User> FindUser(int id)
{
    if (id <= 0) return Result<User>.Fail(new Error("Must be positive"));
    if (id > 1000) return Result<User>.Fail(new Error($"User {id} not found"));
    return Result<User>.Ok(new User(id, "alice@example.com", "Admin"));
}

// --- Caller handles one type, two branches, no try/catch ---
void TryFindUser(int id)
{
    Console.WriteLine($"\n> FindUser({id})");
    var result = FindUser(id);

    if (result.IsSuccess)
        Console.WriteLine($"  Found: {result.Value.Email}");
    else
        Console.WriteLine($"  Error: {result.Errors.First().Message}");
}

TryFindUser(42);    // success
TryFindUser(9999);  // failure — not found
TryFindUser(-1);    // failure — bad input

Console.WriteLine("""

  What changed:
  - Return type Result<User> makes ALL outcomes visible in the signature
  - Both failure cases use the same mechanism — no special null or exception handling
  - result.IsSuccess / result.IsFailure — symmetric, always explicit

  See Lesson 2 for Map — transforming the value without unwrapping it.
""");

record User(int Id, string Email, string Role);
