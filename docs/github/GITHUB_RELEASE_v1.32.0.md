# REslava.Result v1.32.0

Three DX improvements: applicative validation that accumulates all errors at once, C# tuple unpacking for results, and a bidirectional `Maybe<T>` ↔ `Result<T>` bridge.

---

## ✅ Applicative Validation — `Result.Validate`

Run multiple independent validations and collect **all** errors simultaneously — no short-circuiting:

```csharp
Result<CreateOrderDto> dto = Result.Validate(
    ValidateName(request.Name),
    ValidateEmail(request.Email),
    ValidateAge(request.Age),
    (name, email, age) => new CreateOrderDto(name, email, age));

// If Name and Age fail → dto.Errors contains BOTH errors
// If all succeed → dto.Value = new CreateOrderDto(...)
```

Distinct from existing API:

| Method | Types | Short-circuits? | Mapper? |
|--------|-------|-----------------|---------|
| `Bind` | chained `Result<T>` | ✅ yes | no |
| `Result.Combine` | same-type collection | ❌ no | no — returns `Result<IEnumerable<T>>` |
| `Result.Validate` | **heterogeneous** types | ❌ no | ✅ yes — typed `Result<TResult>` |

2, 3, and 4-way overloads available.

---

## 🔓 Tuple Unpacking — `Result<T>.Deconstruct`

C# 8+ deconstruction support:

```csharp
// 2-component
var (value, errors) = GetUser(id);
if (errors.Count == 0) Console.WriteLine(value!.Name);

// 3-component
var (isSuccess, value, errors) = GetUser(id);
if (isSuccess) Console.WriteLine(value!.Name);

// Non-generic Result
var (isSuccess, errors) = DoSomething();
```

Value is `default` when `IsFailure`. Zero breaking changes.

---

## 🔁 `Maybe<T>` ↔ `Result<T>` Interop

Bidirectional bridge between the two types:

```csharp
// Maybe → Result
Result<User> result = maybe.ToResult(() => new NotFoundError("User", id)); // lazy factory
Result<User> result = maybe.ToResult(new NotFoundError("User", id));       // static error
Result<User> result = maybe.ToResult("User not found");                     // string overload

// Result → Maybe (errors discarded)
Maybe<User> maybe = result.ToMaybe();
```

---

## 📦 NuGet

| Package | Link |
|---------|------|
| REslava.Result | [View on NuGet](https://www.nuget.org/packages/REslava.Result/1.32.0) |
| REslava.Result.SourceGenerators | [View on NuGet](https://www.nuget.org/packages/REslava.Result.SourceGenerators/1.32.0) |
| REslava.Result.Analyzers | [View on NuGet](https://www.nuget.org/packages/REslava.Result.Analyzers/1.32.0) |
| REslava.Result.FluentValidation | [View on NuGet](https://www.nuget.org/packages/REslava.Result.FluentValidation/1.32.0) |

---

## Stats

- 3,696 tests passing across net8.0, net9.0, net10.0 (1,157×3) + generator (131) + analyzer (68) + FluentValidation bridge (26)
- 117 features across 11 categories
