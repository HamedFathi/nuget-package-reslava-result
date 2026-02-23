---
hide:
  - navigation
---

# ASP.NET Integration

Plug REslava.Result into your web stack – Minimal API, MVC, and everything in between.

<div class="grid cards" markdown>

-   :material-code-json: __Minimal API__  
    `Result.ToIResult()`, `OneOf.ToIResult()` – automatic HTTP responses.
    [](asp.net-integration.md#resulttoiresult-extensions)

-   :material-view-dashboard: __MVC__  
    `Result.ToActionResult()`, `OneOf.ToActionResult()` – convention‑based MVC controllers.
    [](asp.net-integration.md#resulttoactionresult-extensions-mvc-support--v1210)

-   :material-lightbulb-on: __SmartEndpoints__  
    Zero‑boilerplate API generation from controllers.
    [](asp.net-integration.md#smartendpoints---zero-boilerplate-fast-apis)

-   :material-file-document: __OpenAPI__  
    Auto‑generated metadata for Swagger / Scalar.
    [](asp.net-integration.md#enhanced-smartendpoints--openapi-metadata-new)

-   :material-shield-lock: __Authorization__  
    Built‑in support for policies, roles, and `[AllowAnonymous]`.
    [](asp.net-integration.md#smartendpoints-authorization--policy-support)

-   :material-alert-box: __Problem Details__  
    RFC 7807 compliant error responses.
    [](asp.net-integration.md#problem-details-integration)

</div>