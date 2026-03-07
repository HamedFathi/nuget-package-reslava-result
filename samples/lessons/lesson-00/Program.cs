// =============================================================================
// Lesson 0 — The Problem (Before REslava.Result)
//
// Traditional error handling: nullable returns + exceptions.
// Three outcomes, three different mechanisms, one confusing caller.
// =============================================================================

// --- Method with three possible outcomes, only one visible in the signature ---
User? FindUser(int id)
{
    if (id <= 0) throw new ArgumentException("Must be positive", nameof(id));
    if (id > 1000) return null;  // not found
    return new User(id, "alice@example.com", "Admin");
}

// --- Caller must handle null, valid value, AND exception separately ---
void TryFindUser(int id)
{
    Console.WriteLine($"\n> FindUser({id})");
    try
    {
        var user = FindUser(id);
        if (user == null)
            Console.WriteLine("  User not found");
        else
            Console.WriteLine($"  Found: {user.Email}");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"  Bad input: {ex.Message}");
    }
}

TryFindUser(42);    // success
TryFindUser(9999);  // null — not found
TryFindUser(-1);    // exception — bad input

Console.WriteLine("""

  Problems:
  - Return type User? hides the "bad input" case entirely
  - Two failure modes (null vs exception) need two different handling paths
  - Every caller must remember to null-check AND wrap in try/catch

  See Lesson 1 for the same code using Result<T>.
""");

// --- Domain (type declarations must follow top-level statements) ---
record User(int Id, string Email, string Role);
