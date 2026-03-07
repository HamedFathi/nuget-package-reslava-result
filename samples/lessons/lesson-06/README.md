# Lesson 6 — Domain Errors: Be Specific

**Goal**: Replace generic `Error` strings with typed domain errors. Use `ValidationError.FieldName`, `NotFoundError`, `ForbiddenError` — type-switch in Match, no string matching.

**Concepts**: `ValidationError(fieldName, message)` · `NotFoundError` · `ForbiddenError` · type-switch in `Match` · `IReason` vs typed errors

## Run

```bash
dotnet run
```

## Previous / Next

[← Lesson 5 — Match](../lesson-05/README.md) | [Lesson 7 — Async Pipelines →](../lesson-07/README.md)
