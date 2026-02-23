---
title: Quick Start
---

### Installation

```bash
dotnet add package REslava.Result                      # Core library
dotnet add package REslava.Result.SourceGenerators     # ASP.NET source generators
dotnet add package REslava.Result.Analyzers            # Roslyn safety analyzers
```

### Complete Generator Showcase

#### ⚡ SmartEndpoints - Zero-Boilerplate Fast APIs
Generate complete Minimal APIs from controllers with automatic HTTP mapping!

```csharp
[AutoGenerateEndpoints(RoutePrefix = "/api/users")]
public class UserController {
    private readonly UserService _service;
    public UserController(UserService service) => _service = service;

    // 🚀 DI + async → Automatic REST API with dependency injection!
    public async Task<OneOf<ValidationError, NotFoundError, User>>
        GetUser(int id) => await _service.GetUserByIdAsync(id);

    public async Task<OneOf<ValidationError, ConflictError, User>>
        CreateUser(CreateUserRequest request) => await _service.CreateAsync(request);

    public async Task<Result<List<User>>> GetUsers()
        => await _service.GetAllAsync();
}
```

**🎉 Generated Minimal API (Zero Manual Code!)**
- ✅ `POST /api/users` → 201/400/404/409 (OneOf4 auto-mapping!)
- ✅ `GET /api/users/{id}` → 200/404 (OneOf2 auto-mapping!)
- ✅ **Full OpenAPI metadata** — `.Produces<T>(200)`, `.Produces(404)`, `.WithSummary()`, `.WithTags()` auto-generated from return types
- ✅ **Error handling** automatically configured
- ✅ **HTTP status mapping** automatically applied
- ✅ **Route grouping** via `MapGroup` with automatic tag generation

**🔥 Development Speed: 10x Faster**
- **No manual route setup** - automatic from method names
- **No manual error handling** - automatic from return types
- **No manual status codes** - automatic from error types
- **No manual API docs** - OpenAPI + Scalar UI automatically generated
- **Self-explanatory code** - business logic only

#### 🔄 OneOf Extensions - Intelligent HTTP Mapping
Automatic error detection and HTTP status mapping for OneOf types:

```csharp
// Error Types → HTTP Status Codes
ValidationError → 400 Bad Request
UserNotFoundError → 404 Not Found  
ConflictError → 409 Conflict
UnauthorizedError → 401 Unauthorized
ForbiddenError → 403 Forbidden
ServerError → 500 Internal Server Error
```

**Supported Patterns:**
- **OneOf2ToIResult<T1,T2>** - Two-type error handling
- **OneOf3ToIResult<T1,T2,T3>** - Three-type error handling  
- **🆕 OneOf4ToIResult<T1,T2,T3,T4>** - Four-type error handling (NEW v1.12.0!)
- **SmartEndpoints Integration** - Uses extensions automatically in generated APIs

#### 🚀 Enhanced SmartEndpoints + OpenAPI Metadata (NEW!)

**Feature**: Full OpenAPI metadata auto-generated at compile time from return types
**Benefits**: Scalar/Swagger UI shows typed responses, status codes, summaries, and tags — zero manual configuration
**Use Case**: Production-ready APIs with complete documentation from day one

**🔥 What Makes SmartEndpoints Revolutionary:**

```csharp
// ✅ YOU WRITE: Pure business logic (5 lines)
[AutoGenerateEndpoints(RoutePrefix = "/api/orders")]
public class SmartOrderController {
    public async Task<OneOf<UserNotFoundError, InsufficientStockError, ValidationError, OrderResponse>>
        CreateOrder(CreateOrderRequest request) => await _service.CreateOrderAsync(request);
}

// 🎉 GENERATOR PRODUCES: Complete endpoint with full OpenAPI metadata
var smartOrderGroup = endpoints.MapGroup("/api/orders")
    .WithTags("Smart Order");

smartOrderGroup.MapPost("/", async (CreateOrderRequest request, SmartOrderController service) =>
{
    var result = await service.CreateOrder(request);
    return result.ToIResult();
})
    .WithName("SmartOrder_CreateOrder")
    .WithSummary("Create order")
    .Produces<OrderResponse>(200)
    .Produces(400)   // ← ValidationError
    .Produces(404)   // ← UserNotFoundError
    .Produces(409);  // ← InsufficientStockError
```

**🎯 Everything Auto-Generated from Return Types:**
- **Method name** → HTTP method + `.WithName()` (`CreateOrder` → `POST` + `SmartOrder_CreateOrder`)
- **Class name** → `.WithTags()` + `MapGroup()` (`SmartOrderController` → `"Smart Order"`)
- **PascalCase** → `.WithSummary()` (`CreateOrder` → `"Create order"`)
- **Success type** → `.Produces<T>(200)` (`OrderResponse` → typed 200 response)
- **Error types** → `.Produces(statusCode)` (`UserNotFoundError` → 404, `InsufficientStockError` → 409)
- **Parameters** → Route/body binding (`int id` → `/{id}`, `request` → JSON body)

**⚡ Zero Boilerplate Benefits:**
- **No manual route configuration** - inferred from class/method names
- **No manual error handling** - automatic from OneOf types
- **No manual status codes** - automatic from error type names
- **No manual OpenAPI metadata** - `.Produces()`, `.WithSummary()`, `.WithTags()` all auto-generated
- **No manual endpoint names** - globally unique names from controller + method
- **No manual ProblemDetails** - automatic RFC 7807 compliance

#### 🎯 ResultToIResult Extensions
Convert Result<T> types to proper HTTP responses:

```csharp
public Result<User> GetUser(int id) { /* ... */ }
return GetUser(id).ToIResult(); // Automatic HTTP mapping


app.MapGet("/users/{id}", async (int id, IUserService service) =>
{
    return await service.GetUserAsync(id); // Auto-converts to HTTP response!
});

// 🆕 v1.10.0: OneOf extensions also work!
app.MapGet("/users/oneof/{id}", async (int id) =>
{
    return GetOneOfUser(id); // Auto-converts OneOf<T1,T2,T3> too!
});
```

#### 🛡️ Safety Analyzers — Compile-Time Diagnostics

Catch common Result<T> and OneOf mistakes **at compile time** with 5 diagnostics and 3 code fixes:

```csharp
// RESL1001 — Unsafe .Value access without guard [Warning + Code Fix]
var result = GetUser(id);
var name = result.Value;        // ⚠️ Warning: Access to '.Value' without checking 'IsSuccess'
                                // 💡 Fix A: Wrap in if (result.IsSuccess) { ... }
                                // 💡 Fix B: Replace with result.Match(v => v, e => default)

// ✅ Safe alternatives:
if (result.IsSuccess)
    var name = result.Value;    // No warning — guarded by IsSuccess

var name = result.Match(        // No warning — pattern matching
    onSuccess: u => u.Name,
    onFailure: _ => "Unknown");
```

```csharp
// RESL1002 — Discarded Result<T> return value [Warning]
Save();                         // ⚠️ Warning: Return value of type 'Result<T>' is discarded
await SaveAsync();              // ⚠️ Warning: errors silently swallowed

// ✅ Safe alternatives:
var result = Save();            // No warning — assigned
return Save();                  // No warning — returned
```

```csharp
// RESL1003 — Prefer Match() over if-check [Info suggestion]
if (result.IsSuccess)           // ℹ️ Suggestion: Consider using Match() instead
{
    var x = result.Value;
}
else
{
    var e = result.Errors;
}

// ✅ Cleaner with Match():
var x = result.Match(v => v, e => HandleErrors(e));
```

```csharp
// RESL1004 — Task<Result<T>> assigned without await [Warning + Code Fix]
async Task M()
{
    var result = GetFromDb(id);     // ⚠️ Warning: 'GetFromDb' returns Task<Result<T>> but is not awaited
                                    // 💡 Fix: Add 'await'
}

// ✅ Safe alternatives:
var result = await GetFromDb(id);   // No warning — properly awaited
Task<Result<User>> task = GetFromDb(id); // No warning — explicit Task type (intentional)
```

```csharp
// RESL2001 — Unsafe OneOf.AsT* access without IsT* check [Warning + Code Fix]
var oneOf = GetResult();        // OneOf<User, NotFound, ValidationError>
var user = oneOf.AsT1;          // ⚠️ Warning: Access to '.AsT1' without checking '.IsT1'
                                // 💡 Fix: Replace with oneOf.Match(t1 => t1, t2 => throw ..., t3 => throw ...)

// ✅ Safe alternatives:
if (oneOf.IsT1)
    var user = oneOf.AsT1;      // No warning — guarded

var user = oneOf.Match(         // No warning — exhaustive
    user => user,
    notFound => throw ...,
    error => throw ...);
```

```bash
dotnet add package REslava.Result.Analyzers
```

---