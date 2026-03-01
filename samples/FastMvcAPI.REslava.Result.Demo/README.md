# FastMvcAPI.REslava.Result.Demo

ASP.NET Core **MVC Controller** demo for [REslava.Result](https://github.com/reslava/nuget-package-reslava-result).

Runs on **port 5001**. Compare side-by-side with `FastMinimalAPI` (port 5000) to see the same domain logic expressed through both programming models.

## What it demonstrates

- `Result<T>.ToActionResult()` — one-liner HTTP mapping in controller actions
- `ToPostActionResult()` / `ToDeleteActionResult()` — HTTP verb variants (201 Created, 204 No Content)
- `OneOf<T1, T2, ...>.ToActionResult()` — discriminated union auto-mapping
- Domain errors auto-mapped to HTTP status codes via `HttpStatusCode` tag:
  - `NotFoundError` → 404
  - `ValidationError` → 422
  - `ConflictError` → 409
  - `UnauthorizedError` → 401
- Zero exception-based control flow

## Run

```bash
dotnet run --project samples/FastMvcAPI.REslava.Result.Demo
```

API available at `http://localhost:5001`.

## Explore

| Tool | URL |
|------|-----|
| Scalar UI | http://localhost:5001/scalar |
| OpenAPI JSON | http://localhost:5001/openapi/v1.json |
| Health check | http://localhost:5001/health |

## Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/users` | All users |
| `GET` | `/api/users/{id}` | User by ID — `OneOf<NotFoundError, UserResponse>` |
| `POST` | `/api/users` | Create — `OneOf<ValidationError, ConflictError, UserResponse>` |
| `PUT` | `/api/users/{id}` | Update — `OneOf<ValidationError, NotFoundError, ConflictError, UserResponse>` |
| `DELETE` | `/api/users/{id}` | Delete — `Result<bool>.ToDeleteActionResult()` |
| `GET` | `/api/products` | All products |
| `GET` | `/api/products/{id}` | Product by ID |
| `POST` | `/api/products` | Create product |
| `PUT` | `/api/products/{id}` | Update product |
| `DELETE` | `/api/products/{id}` | Delete product |
| `GET` | `/api/orders` | All orders |
| `GET` | `/api/orders/{id}` | Order by ID |
| `POST` | `/api/orders` | Create order |
| `DELETE` | `/api/orders/{id}` | Cancel order |
| `POST` | `/auth/token` | Generate test JWT (`?role=admin`) |

## Key pattern

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _service;
    public UsersController(UserService service) => _service = service;

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
        => (await _service.GetUserByIdAsync(id)).ToActionResult();
        // OneOf<NotFoundError, UserResponse> → 404 or 200

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        => (await _service.CreateUserAsync(request)).ToActionResult();
        // OneOf<ValidationError, ConflictError, UserResponse> → 422, 409, or 200

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => (await _service.DeleteUserAsync(id)).ToDeleteActionResult();
        // Result<bool> → 204 No Content or error
}
```

## JWT authentication

Some endpoints require a Bearer token. Generate one:

```bash
curl -X POST "http://localhost:5001/auth/token?role=admin"
```

Use the returned token as `Authorization: Bearer <token>` in subsequent requests, or paste it directly into Scalar UI.

## Data

Uses **EF Core In-Memory** database seeded on startup. Data resets each run.

## See also

- [`FastMinimalAPI`](../FastMinimalAPI.REslava.Result.Demo/) — same domain logic, Minimal API style (port 5000)
- [REslava.Result documentation](https://reslava.github.io/nuget-package-reslava-result)
