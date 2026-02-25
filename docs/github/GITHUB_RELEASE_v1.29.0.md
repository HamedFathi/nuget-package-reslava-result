# v1.29.0 — IsFailure Rename + Samples Update

## ⚠️ Breaking Change: `IsFailed` → `IsFailure`

`IsSuccess` / `IsFailure` is the correct symmetric pair. `IsFailed` was past-tense verb form — semantically inconsistent with `IsSuccess`. No alias or `[Obsolete]` shim is provided.

**Migration:** find-and-replace across your codebase.

```csharp
// Before
if (result.IsFailed) ...

// After
if (result.IsFailure) ...
```

## Console Samples — 3 New Examples

Three new examples covering every feature added in v1.27.0–v1.28.0:

### `14_ValidationDSL.cs`
All 19 native DSL rules with field-name auto-inference and a real-world order validator:

```csharp
private static ValidatorRuleSet<CreateOrderRequest> BuildOrderValidator() =>
    new ValidatorRuleBuilder<CreateOrderRequest>()
        .NotEmpty(r => r.CustomerId)
        .MinLength(r => r.CustomerId, 3)
        .Positive(r => r.Amount)
        .NotEmpty(r => r.Items)
        .MinCount(r => r.Items, 1)
        .Build();

var result = validator.Validate(request);
// result.IsFailure → result.ValidationErrors (list of ValidationError with FieldName)
```

### `15_OneOf5_OneOf6.cs`
5/6-way discriminated unions, chain extensions, and a checkout pipeline:

```csharp
// 5-way match
OneOf<PaymentSuccess, InsufficientFunds, CardDeclined, FraudAlert, NetworkError> outcome = ...;
outcome.Match(
    success   => $"Paid {success.Amount:C}",
    funds     => $"Insufficient: need {funds.Required:C}",
    declined  => $"Declined: {declined.Reason}",
    fraud     => $"Fraud alert: {fraud.RiskScore}",
    network   => $"Network error: {network.Message}");

// Chain up from TwoWay → SixWay
var six = twoWay.ToThreeWay(defaultT3).ToFourWay(defaultT4).ToFiveWay(defaultT5).ToSixWay(defaultT6);
```

### `16_AsyncPatterns_Advanced.cs`
WhenAll, Retry with backoff, Timeout, TapOnFailure, OkIf/FailIf, Try/TryAsync — and a combined pipeline:

```csharp
// Retry + Timeout + TapOnFailure in one expression
var result = await Result.Retry(
    operation: async () =>
        (await SlowOperation().Timeout(TimeSpan.FromMilliseconds(150)))
            .TapOnFailure((IError err) => logger.Warn(err.Message)),
    maxRetries: 3,
    delay: TimeSpan.FromMilliseconds(5));
```

## Validation Showcase in FastMinimalAPI

New endpoints demonstrate all three validation approaches side-by-side:

| Endpoint | Approach |
|---|---|
| `POST /api/smart/validation/annotated` | DataAnnotations + `[Validate]` auto-guard |
| `POST /api/smart/validation/dsl` | Native Validation DSL inside service |
| `POST /api/smart/fluent-validation/product` | `[FluentValidate]` bridge (optional) |

## FastMvcAPI Parity

MVC controllers now match MinimalAPI: explicit `request.Validate()` guard + `CancellationToken` in all actions.

```csharp
[HttpPost]
public async Task<IActionResult> Create(
    [FromBody] CreateProductRequest request,
    CancellationToken cancellationToken)
{
    var validation = request.Validate();
    if (!validation.IsSuccess)
        return validation.ToActionResult();

    var result = await _productService.CreateProductAsync(request, cancellationToken);
    return result.ToActionResult();
}
```

## Test Suite

- 3,339 tests passing across net8.0, net9.0, net10.0 + generator (131) + analyzer (68) + FluentValidation bridge (26)

## NuGet Packages

- [REslava.Result 1.29.0](https://www.nuget.org/packages/REslava.Result/1.29.0)
- [REslava.Result.SourceGenerators 1.29.0](https://www.nuget.org/packages/REslava.Result.SourceGenerators/1.29.0)
- [REslava.Result.Analyzers 1.29.0](https://www.nuget.org/packages/REslava.Result.Analyzers/1.29.0)
- [REslava.Result.FluentValidation 1.29.0](https://www.nuget.org/packages/REslava.Result.FluentValidation/1.29.0)
