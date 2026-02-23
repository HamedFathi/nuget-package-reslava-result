using FastMinimalAPI.REslava.Result.Demo.Filters;
using FastMinimalAPI.REslava.Result.Demo.Models;
using FastMinimalAPI.REslava.Result.Demo.Services;
using REslava.Result;
using REslava.Result.AdvancedPatterns;
using REslava.Result.SourceGenerators.SmartEndpoints;

namespace FastMinimalAPI.REslava.Result.Demo.SmartEndpoints;

/// <summary>
/// SmartEndpoint controller demonstrating Filters, Output Caching, and Rate Limiting.
///
/// Class-level defaults (CacheSeconds = 60, RateLimitPolicy = "api"):
///   GET    /api/smart/catalog        → GetProducts()      cached 60s, rate-limited "api"
///   GET    /api/smart/catalog/{id}   → GetProduct(id)     cached 300s (override), rate-limited "strict" + logged
///   POST   /api/smart/catalog        → CreateProduct()    no cache (POST), rate-limited "api" + logged
///   PUT    /api/smart/catalog/{id}   → UpdateProduct()    no cache (PUT), rate-limited "api"
///   DELETE /api/smart/catalog/{id}   → DeleteProduct()    rate limiting explicitly disabled ("none")
/// </summary>
[AutoGenerateEndpoints(
    RoutePrefix = "/api/smart/catalog",
    CacheSeconds = 60,          // all GETs cached 60s by default
    RateLimitPolicy = "api")]   // all endpoints rate-limited by default
public class SmartCatalogController
{
    private readonly ProductService _service;

    public SmartCatalogController(ProductService service) => _service = service;

    /// <summary>List all catalog products — inherits 60s cache and "api" rate limit.</summary>
    public async Task<Result<List<ProductResponse>>> GetProducts()
        => await _service.GetAllProductsAsync();

    /// <summary>Get catalog product by ID — 300s cache override, stricter "strict" rate limit, request logged.</summary>
    [AutoMapEndpoint("/api/smart/catalog/{id}", CacheSeconds = 300, RateLimitPolicy = "strict")]
    [SmartFilter(typeof(LoggingEndpointFilter))]
    public async Task<OneOf<NotFoundError, ProductResponse>> GetProduct(int id)
        => await _service.GetProductByIdAsync(id);

    /// <summary>Create catalog product — POST is never cached, inherits "api" rate limit, request logged.</summary>
    [SmartFilter(typeof(LoggingEndpointFilter))]
    public async Task<OneOf<ValidationError, ProductResponse>> CreateProduct(CreateProductRequest request)
        => await _service.CreateProductAsync(request);

    /// <summary>Update catalog product — PUT is never cached, inherits "api" rate limit.</summary>
    public async Task<OneOf<ValidationError, NotFoundError, ProductResponse>> UpdateProduct(int id, UpdateProductRequest request)
        => await _service.UpdateProductAsync(id, request);

    /// <summary>Delete catalog product — rate limiting explicitly disabled for this endpoint.</summary>
    [AutoMapEndpoint("/api/smart/catalog/{id}", RateLimitPolicy = "none")]
    public async Task<Result<bool>> DeleteProduct(int id)
        => await _service.DeleteProductAsync(id);
}
