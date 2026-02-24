# v1.26.0 — RESL1005 Analyzer + SmartEndpoints Auto-Validation

## RESL1005: Suggest Domain-Specific Error Type (New Analyzer)

A new **Info-level analyzer** that suggests switching from generic `new Error("...")` to a
domain-specific error type when the message implies a known HTTP error category:

| Keyword match | Suggested type |
|--------------|----------------|
| `not found`, `missing` | `NotFoundError` |
| `conflict`, `already exists` | `ConflictError` |
| `unauthorized` | `UnauthorizedError` |
| `forbidden`, `access denied` | `ForbiddenError` |
| `invalid`, `validation` | `ValidationError` |

Domain-specific types carry HTTP status context and integrate automatically with `ToIResult()`.

## SmartEndpoints Auto-Validation

When a controller method's body parameter type is decorated with `[Validate]`, the
SmartEndpoints generator now **automatically injects** a validation block into the generated
endpoint lambda — no manual wiring required:

```csharp
// Generated output (before: only the service call; now: validation block injected)
catalogGroup.MapPost("/", async (CreateProductRequest req, CatalogController svc) =>
{
    var validation = req.Validate();
    if (!validation.IsSuccess) return validation.ToIResult();

    var result = await svc.CreateProduct(req);
    return result.ToIResult();
})
```

Detection rules:
- Only `ParameterSource.Body` params (POST/PUT) are auto-validated
- GET query params are skipped even if the type has `[Validate]`
- `using Generated.ValidationExtensions;` is added only when needed

## Test Suite

- 2,862 tests passing across net8.0, net9.0, net10.0 + generator (106) + analyzer (68) tests

## NuGet Packages

- [REslava.Result 1.26.0](https://www.nuget.org/packages/REslava.Result/1.26.0)
- [REslava.Result.SourceGenerators 1.26.0](https://www.nuget.org/packages/REslava.Result.SourceGenerators/1.26.0)
- [REslava.Result.Analyzers 1.26.0](https://www.nuget.org/packages/REslava.Result.Analyzers/1.26.0)
