# v1.23.0 — SmartEndpoints Production Readiness

## SmartEndpoints: Endpoint Filters

New `[SmartFilter(typeof(T))]` attribute generates `.AddEndpointFilter<T>()` per filter. `AllowMultiple = true` — stack as many as needed, applied in declaration order.

```csharp
[AutoGenerateEndpoints(RoutePrefix = "/api/catalog")]
public class CatalogController
{
    [SmartFilter(typeof(LoggingFilter))]
    [SmartFilter(typeof(ValidationFilter<CreateProductRequest>))]
    public async Task<OneOf<ValidationError, Product>> CreateProduct(CreateProductRequest req)
        => await _service.CreateProductAsync(req);
}
```

Generated:

```csharp
catalogGroup.MapPost("/", async (CreateProductRequest req, CatalogController service) =>
{
    var result = await service.CreateProduct(req);
    return result.ToIResult();
})
    .WithName("Catalog_CreateProduct")
    .Produces<Product>(200)
    .Produces(422)
    .AddEndpointFilter<LoggingFilter>()
    .AddEndpointFilter<ValidationFilter<CreateProductRequest>>();
```

## SmartEndpoints: Output Caching

`CacheSeconds` on `[AutoGenerateEndpoints]` (class default) and `[AutoMapEndpoint]` (method override). Only applied to GET endpoints. Use `-1` to explicitly disable for a method when the class sets a default.

```csharp
[AutoGenerateEndpoints(RoutePrefix = "/api/catalog", CacheSeconds = 60)]
public class CatalogController
{
    // Inherits 60s cache → GET /api/catalog
    public async Task<Result<List<Product>>> GetProducts() => ...;

    // 300s override → GET /api/catalog/{id}
    [AutoMapEndpoint("/api/catalog/{id}", CacheSeconds = 300)]
    public async Task<OneOf<NotFoundError, Product>> GetProduct(int id) => ...;

    // POST: cache never applied regardless of CacheSeconds
    public async Task<OneOf<ValidationError, Product>> CreateProduct(CreateProductRequest req) => ...;
}
```

## SmartEndpoints: Rate Limiting

`RateLimitPolicy` on `[AutoGenerateEndpoints]` (class default) and `[AutoMapEndpoint]` (method override). Use `"none"` to explicitly opt out for a specific endpoint.

```csharp
[AutoGenerateEndpoints(RoutePrefix = "/api/catalog", RateLimitPolicy = "api")]
public class CatalogController
{
    // Inherits "api" rate limit
    public async Task<Result<List<Product>>> GetProducts() => ...;

    // "strict" override for expensive endpoint
    [AutoMapEndpoint("/api/catalog/{id}", RateLimitPolicy = "strict")]
    public async Task<OneOf<NotFoundError, Product>> GetProduct(int id) => ...;

    // Explicitly no rate limit (e.g. health/ping endpoints)
    [AutoMapEndpoint("/api/catalog/ping", RateLimitPolicy = "none")]
    public Task<Result<string>> GetPing() => Task.FromResult(Result<string>.Ok("pong"));
}
```

## Test Suite

- 2,836 tests passing across net8.0, net9.0, net10.0
- 11 new source generator tests for Filters, Caching, and Rate Limiting

## NuGet Packages

- [REslava.Result 1.23.0](https://www.nuget.org/packages/REslava.Result/1.23.0)
- [REslava.Result.SourceGenerators 1.23.0](https://www.nuget.org/packages/REslava.Result.SourceGenerators/1.23.0)
- [REslava.Result.Analyzers 1.23.0](https://www.nuget.org/packages/REslava.Result.Analyzers/1.23.0)
