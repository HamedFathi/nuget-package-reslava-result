---
hide:
  - navigation
---

# Core Concepts

The functional programming foundation of REslava.Result. Learn each piece step by step.

<div class="grid cards" markdown>

-   :material-checkbox-marked-circle: __Result Pattern__
    `Result<T>`, factory methods, pattern matching, and safe value access.
    [](reslava.result-core-library.md#core-operations)

-   :material-link: __Functional Composition__
    Chain operations with `Bind`, `Map`, `Tap`, `Compose`.
    [](reslava.result-core-library.md#functional-composition)

-   :material-timer-sand: __Async Patterns__
    Async variants, `WhenAll`, `Retry`, `Timeout`.
    [](reslava.result-core-library.md#async-patterns-whenall-retry-timeout)

-   :material-code-braces: __LINQ Integration__
    Use query comprehension syntax with `Result<T>`.
    [](reslava.result-core-library.md#linq-integration)

-   :material-swap-horizontal: __Conditional Factories__
    `OkIf` / `FailIf` — create results directly from boolean conditions.
    [](reslava.result-core-library.md#conditional-factories--okif--failif)

-   :material-shield-bug: __Exception Wrapping__
    `Try` / `TryAsync` — catch exceptions and convert to `Result<T>`.
    [](reslava.result-core-library.md#exception-wrapping--try--tryasync)

-   :material-alert: __Error Types__
    Built‑in domain errors (`NotFoundError`, `ValidationError`, `ConflictError`, etc.), custom CRTP errors, and rich tag context.
    [](error-types.md)

-   :material-null: __Maybe&lt;T&gt;__
    Safe null handling with optionals — no null reference exceptions.
    [](maybe.md)

-   :simple-oneplus: __OneOf Unions__
    Discriminated unions for multiple typed outcomes with exhaustive matching.
    [](oneof-unions.md)

-   :material-check-all: __Validation Rules__
    Declarative rule-based validation that accumulates all failures.
    [](validation-rules.md)

-   :material-tag-check: __Validation Attributes__
    `[Validate]` source generator — DataAnnotations → `Result<T>` automatically.
    [](validation-attributes.md)

-   :material-speedometer: __Performance__
    Optimized patterns for high‑throughput scenarios.
    [](advanced-patterns.md#performance-patterns)

</div>
