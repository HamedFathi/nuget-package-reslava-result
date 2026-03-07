// =============================================================================
// Lesson 2 — Map: Transform the Value
//
// Map applies a function to the success value.
// If the result is Fail, the function is never called — failure passes through.
// =============================================================================
using REslava.Result;

Result<User> FindUser(int id) =>
    id > 0 && id <= 1000
        ? Result<User>.Ok(new User(id, "alice@example.com", "Admin"))
        : Result<User>.Fail(new Error($"User {id} not found"));

// --- Map: transform User → string without unwrapping ---
// [ResultFlow]
Result<string> GetDisplayName(int id) =>
    FindUser(id)
        .Map(user => user.Email.ToUpperInvariant())
        .Map(email => $"Display: {email}");

Console.WriteLine("> Success case");
var ok = GetDisplayName(42);
Console.WriteLine($"  IsSuccess: {ok.IsSuccess}");
Console.WriteLine($"  Value:     {ok.Value}");

Console.WriteLine("\n> Failure case — Map is never called");
var fail = GetDisplayName(9999);
Console.WriteLine($"  IsSuccess: {fail.IsSuccess}");
Console.WriteLine($"  Error:     {fail.Errors.First().Message}");

Console.WriteLine("""

  Key rule:
  - Map(fn) — fn runs only when IsSuccess; failure bypasses it untouched
  - Use Map when the transformation itself CANNOT fail
  - Use Bind (Lesson 4) when the transformation CAN fail

  See Lesson 3 for Ensure — adding guard conditions to a pipeline.
""");

record User(int Id, string Email, string Role);
