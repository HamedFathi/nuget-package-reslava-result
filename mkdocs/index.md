---
hide:
  - navigation
title: No exceptions. No nulls. 
description: Result<T> + OneOf + Maybe<T> + SmartEndpoints. Handle errors, auto-generate APIs, compose async pipelines. One package.
tagline: Railway-oriented .NET made simple.
---

# REslava.Result .NET Functional Error Handling & Zero‑Boilerplate APIs
<div align="center" markdown>
![.NET](https://img.shields.io/badge/.NET-512BD4?logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?&logo=csharp&logoColor=white)
![NuGet Version](https://img.shields.io/nuget/v/REslava.Result.AspNetCore?style=flat&logo=nuget)
![License](https://img.shields.io/badge/license-MIT-green)
[![GitHub contributors](https://img.shields.io/github/contributors/reslava/REslava.Result)](https://GitHub.com/reslava/REslava.Result/graphs/contributors/) 
[![GitHub Stars](https://img.shields.io/github/stars/reslava/REslava.Result)](https://github.com/reslava/REslava.Result/stargazers) 
[![NuGet Downloads](https://img.shields.io/nuget/dt/REslava.Result)](https://www.nuget.org/packages/REslava.Result)
![Test Coverage](https://img.shields.io/badge/coverage-95%25-brightgreen)
![Test Suite](https://img.shields.io/badge/tests-3960%20passing-brightgreen)
</div>

**:material-api: The only .NET library that blends functional error handling with compile‑time API generation.**

**:material-source-repository: [nuget-package-reslava-result GitHub repo](https://github.com/reslava/nuget-package-reslava-result)**

!!! tip "🗺️ See how your `Result<T>` flows — before it runs. Pipeline Visualization."
    Annotate any fluent pipeline with `[ResultFlow]` and with **single-click code action** ResultFlow will insert a **Mermaid diagram comment** so you can visualize every success path, failure branch, and side effect in your pipeline — zero runtime overhead, zero maintenance.

    [→ ResultFlow](resultflow){ .md-button }

!!! note "📚 New to functional programming? Start with the progressive tutorial series."
    9 self-contained lessons that teach **functional & railway-oriented programming** step by step — from plain C# exceptions all the way to async pipelines and ASP.NET. Each lesson is a standalone `dotnet run`, no setup required.

    Learn all three packages progressively: **REslava.Result** · **REslava.ResultFlow** · **REslava.Result.AspNetCore**

    YouTube video series — coming soon.

    [→ Tutorial Lessons](https://github.com/reslava/nuget-package-reslava-result/tree/main/samples/lessons/){ .md-button }

---

<div class="grid cards" markdown>

-   :material-rocket-launch: __Getting Started__  
    Installation, quick start, and the transformation (70-90% less code).
    [](getting-started)

-   :material-cube-outline: __Core Concepts__  
    Functional programming foundation: Result, composition, async, Maybe, OneOf, validation, and more.
    [](core-concepts)

-   :material-api: __ASP.NET Integration__
    Minimal API, MVC, SmartEndpoints, OpenAPI, authorization, and problem details.
    [](aspnet)
    {: .is-featured }

-   :material-shield-check: __Safety Analyzers__
    7 Roslyn diagnostics + 3 code fixes — catch `Result<T>` and `OneOf` mistakes at compile time.
    [](safety-analyzers)

-   :material-puzzle: __Architecture & Design__
    How the library is built – SOLID, package structure, and the source generator pipeline.
    [](architecture)

-   :material-school: __Tutorial Series__
    9 progressive lessons — functional & railway-oriented programming from scratch. `REslava.Result` · `REslava.ResultFlow` · `REslava.Result.AspNetCore`. YouTube series coming soon.
    [](https://github.com/reslava/nuget-package-reslava-result/tree/main/samples/lessons/)

-   :material-language-csharp: __Code examples__
    Code examples: Fast APIs, Console and quick code examples.
    [](code-examples)

-   :material-test-tube: __Testing & Quality__
    3,339+ tests, CI/CD, real‑world impact, and production benefits.
    [](testing)

-   :material-book-open-variant: __Reference__  
    Version history, roadmap, and API documentation.
    [](reference)

-   :material-account-group: __Community__  
    Contributing, license, and acknowledgments.
    [](community)

</div>

---

## Why REslava.Result?

> **Zero‑boilerplate APIs, railway‑oriented programming, and source‑generated ASP.NET integration – all in one package.**

⚡ **[Performance benchmarks](reference/performance/)** — `Ok` creation **9.6× faster** than FluentResults · failure handling **6.8× faster** than exceptions · measured on .NET 9 with BenchmarkDotNet.

!!! example "Feature Comparison"
    | | REslava.Result | FluentResults | ErrorOr | LanguageExt |
    |---|:---:|:---:|:---:|:---:|
    | Result&lt;T&gt; pattern | ✅ | ✅ | ✅ | ✅ |
    | OneOf discriminated unions | ✅ (2-6 types) | — | — | ✅ |
    | Maybe&lt;T&gt; | ✅ | — | — | ✅ |
    | **ASP.NET source generators (Minimal API + MVC)** | **✅** | — | — | — |
    | **SmartEndpoints (zero-boilerplate APIs)** | **✅** | — | — | — |
    | **OpenAPI metadata auto-generation** | **✅** | — | — | — |
    | **Authorization & Policy support** | **✅** | — | — | — |
    | **Roslyn safety analyzers** | **✅** | — | — | — |
    | **JSON serialization (System.Text.Json)** | **✅** | — | — | — |
    | **Async patterns (WhenAll, Retry, Timeout)** | **✅** | — | — | — |
    | **Domain error hierarchy (NotFound, Validation, etc.)** | **✅** | — | Partial | — |
    | Validation framework | ✅ | Basic | — | ✅ |
    | **FluentValidation bridge** *(optional, migration only)* | **✅** | — | — | — |
    | **Pipeline visualization (`[ResultFlow]`)** | **✅** | — | — | — |
    | Zero dependencies (core) | ✅ | ✅ | ✅ | — |
    | **[`Ok` creation speed](reference/performance/)** | **5.9 ns / 48 B** | 57 ns / 112 B | — | — |
    | **[Failure path vs exceptions](reference/performance/)** | **6.8× faster** | ~5.8× faster | — | — |

---

## Ready to Transform Your Error Handling?

**📖 [Start with the Getting Started Guide](getting-started)**

---

<div align="center" markdown>

**⭐ Star this REslava.Result repository if you find it useful!**

Made with ❤️ by [Rafa Eslava](https://github.com/reslava) for the developer community

[Report Bug](https://github.com/reslava/nuget-package-reslava-result/issues) • [Request Feature](https://github.com/reslava/nuget-package-reslava-result/issues) • [Discussions](https://github.com/reslava/nuget-package-reslava-result/discussions)

</div>