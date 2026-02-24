---
hide:
  - navigation
---

# ASP.NET Integration

Plug REslava.Result into your web stack – Minimal API, MVC, and everything in between.

<div class="grid cards" markdown>

-   :material-lightbulb-on: __SmartEndpoints__
    Zero‑boilerplate Minimal API generation — routes, HTTP verbs, OpenAPI, auth, filters, caching.
    [](smartendpoints.md)
    {: .is-featured }

-   :material-swap-horizontal: __OneOf → IResult__
    Convert `OneOf<T1,...>` to HTTP responses in one call — status codes from error types.
    [](oneof-to-iresult.md)

-   :material-code-json: __Minimal API__
    `Result.ToIResult()`, HTTP method variants (`ToPostResult`, `ToPutResult`, `ToDeleteResult`).
    [](asp.net-integration.md#resulttoiresult-extensions)

-   :material-view-dashboard: __MVC__
    `Result.ToActionResult()`, `OneOf.ToActionResult()` – convention‑based MVC controllers.
    [](asp.net-integration.md#resulttoactionresult-extensions-mvc-support--v1210)

-   :material-shield-lock: __Authorization__
    `RequiresAuth`, `Policies`, `Roles` at class level; `[SmartAllowAnonymous]` per method.
    [](smartendpoints.md#authorization)

-   :material-filter: __Endpoint Filters__
    `[SmartFilter(typeof(T))]` — stack `IEndpointFilter` implementations on any method.
    [](smartendpoints.md#endpoint-filters--smartfilter)

-   :material-timer: __Output Caching & Rate Limiting__
    `CacheSeconds` and `RateLimitPolicy` at class and method level.
    [](smartendpoints.md#output-caching--rate-limiting)

-   :material-alert-box: __Problem Details__
    RFC 7807 compliant error responses via `[MapToProblemDetails]`.
    [](asp.net-integration.md#problem-details-integration)

</div>
