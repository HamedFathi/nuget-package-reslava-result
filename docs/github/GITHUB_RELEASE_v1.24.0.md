# v1.24.0 — Compile-Time Validation Generator

## `[Validate]` Source Generator

Decorate any record or class with `[Validate]` and get a zero-boilerplate `.Validate()` extension
method that runs all `System.ComponentModel.DataAnnotations` constraints and returns `Result<T>`.

```csharp
[Validate]
public record CreateProductRequest(
    [Required] string Name,
    [Range(0.01, double.MaxValue)] decimal Price,
    [StringLength(500)] string? Description
);

// Minimal API — one-liner validation + response
app.MapPost("/products", (CreateProductRequest req) =>
    req.Validate().ToIResult());

// With async business logic
app.MapPost("/products", async (CreateProductRequest req, IProductService svc) =>
    (await req.Validate().BindAsync(r => svc.CreateAsync(r))).ToIResult());
```

**What it generates** (`Generated.ValidationExtensions` namespace):

- Calls `Validator.TryValidateObject` — works with all 20+ annotation types automatically
- On success: returns `Result<T>.Ok(instance)`
- On failure: returns `Result<T>.Fail(IEnumerable<ValidationError>)` with field names populated from `MemberNames`
- Invalid requests automatically produce a 422 response via `.ToIResult()`

Fully composable with the existing pipeline: `.Validate()` → `.Bind()` / `.BindAsync()` → `.ToIResult()` / `.ToActionResult()`.

## Test Suite

- 2,843 tests passing across net8.0, net9.0, net10.0

## NuGet Packages

- [REslava.Result 1.24.0](https://www.nuget.org/packages/REslava.Result/1.24.0)
- [REslava.Result.SourceGenerators 1.24.0](https://www.nuget.org/packages/REslava.Result.SourceGenerators/1.24.0)
- [REslava.Result.Analyzers 1.24.0](https://www.nuget.org/packages/REslava.Result.Analyzers/1.24.0)
