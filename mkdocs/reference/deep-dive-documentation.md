---
title: Deep Dive Documentation
---

### 🎯 **Navigate by Goal**

| I'm building a... | 📖 Start Here | 🎯 What You'll Learn |
|------------------|---------------|---------------------|
| **Web API** | [🌐 ASP.NET Integration](#-the-transformation-70-90-less-code) | Auto-conversion, OneOf extensions, error mapping |
| **Library/Service** | [📐 Core Library](#-reslavaresult-core-library) | Result pattern, validation, error handling |
| **Custom Generator** | [📖 Custom Generator Guide](https://github.com/reslava/nuget-package-reslava-result/blob/main/docs/how-to-create-custom-generator.md) | Build your own source generators |
| **Advanced App** | [🧠 Advanced Patterns](#-advanced-patterns) | Maybe, OneOf, validation rules |
| **Testing** | [🧪 Testing & Quality](#-testing--quality-assurance) | 2,825+ tests, CI/CD, test strategies |
| **Curious About Magic** | [📐 Complete Architecture](#-complete-architecture) | How generators work, SOLID design |

### 📚 **Complete Reference**

- **📖 [Getting Started Guide](https://github.com/reslava/nuget-package-reslava-result/blob/main/docs/getting-started.md)** - Learn the basics
- **🌐 [ASP.NET Integration](https://github.com/reslava/nuget-package-reslava-result/blob/main/docs/aspnet-integration.md)** - HTTP mapping details
- **🚀 [OneOf Extensions](https://github.com/reslava/nuget-package-reslava-result/blob/main/docs/oneof-extensions.md)** - 🆕 External library support
- **⚡ [Source Generator](https://github.com/reslava/nuget-package-reslava-result/blob/main/docs/source-generator.md)** - Smart auto-detection magic
- **🧠 [Functional Programming](https://github.com/reslava/nuget-package-reslava-result/blob/main/docs/functional-programming.md)** - Complete ROP methodology
- **📖 [Custom Generator Guide](https://github.com/reslava/nuget-package-reslava-result/blob/main/docs/how-to-create-custom-generator.md)** - 🆕 Build your own generators
- **📚 [API Reference](docs/api/)** - Complete technical documentation

### 🎯 **Hands-On Samples**

- **🚀 [FastMinimalAPI Demo](https://github.com/reslava/nuget-package-reslava-result/blob/main/samples/FastMinimalAPI.REslava.Result.Demo/README.md)** - Production-ready .NET 10 Minimal API showcase
  - **SmartEndpoints vs Manual** - Side-by-side comparison (~85% less code)
  - **OpenAPI 3.0 + Scalar UI** - Modern API documentation
  - **REslava.Result patterns** - Result<T> and OneOf<T1,T2,T3,T4> discriminated unions
  - **Real-world scenarios** - Users, Products, Orders with full CRUD operations
  - **Zero exception-based control flow** - Type-safe error handling

- **🎯 [FastMvcAPI Demo](https://github.com/reslava/nuget-package-reslava-result/tree/main/samples/FastMvcAPI.REslava.Result.Demo)** - MVC Controller demo showcasing `ToActionResult()` (v1.21.0+)
  - **Result<T>.ToActionResult()** - Convention-based HTTP mapping for MVC controllers
  - **OneOf<>.ToActionResult()** - Domain error auto-mapping for discriminated unions (v1.22.0)
  - **ToPostActionResult(), ToDeleteActionResult()** - HTTP verb variants
  - **Explicit overload escape hatch** - `ToActionResult(onSuccess, onFailure)` for full control
  - **Match() escape hatch** - Full control over custom response bodies when needed
  - **Port 5001** - Runs alongside Minimal API demo (port 5000) for side-by-side comparison

- **📚 [Console Samples](https://github.com/reslava/nuget-package-reslava-result/blob/main/samples/REslava.Result.Samples.Console/README.md)** - 13 progressive examples from basic to advanced
  - **Level 1**: Core Result<T> patterns, validation pipelines, error handling
  - **Level 2**: Async operations, LINQ syntax, custom errors
  - **Level 3**: Maybe<T>, OneOf patterns, Result↔OneOf conversions

- **🔄 [ASP.NET Integration Samples](https://github.com/reslava/nuget-package-reslava-result/blob/main/samples/ASP.NET/README.md)** - Compare pure .NET 10 vs REslava.Result with source generators
  - **MinimalApi.Net10.Reference** - Pure .NET 10 implementation (baseline)
  - **MinimalApi.Net10.REslava.Result.v1.7.3** - REslava.Result + source generators (70-90% less code)

---