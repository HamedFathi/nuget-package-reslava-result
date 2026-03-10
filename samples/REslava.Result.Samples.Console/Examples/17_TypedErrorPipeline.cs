using System;
using System.Collections.Immutable;
using REslava.Result;
using REslava.Result.AdvancedPatterns;
using REslava.Result.Extensions;

namespace REslava.Result.Samples.Console.Examples;

/// <summary>
/// Demonstrates the typed-error pipeline introduced in v1.39.0.
///
/// Key ideas:
///   • Each step declares a single concrete error type — no error union noise in step signatures.
///   • Result&lt;TValue, TError&gt; + Bind grows the error union one slot at a time.
///   • At the call site, Match is exhaustive — every error type must be handled.
/// </summary>
public static class TypedErrorPipeline
{
    public static async Task Run()
    {
        System.Console.WriteLine("=== Typed Error Pipeline (v1.39.0) ===\n");

        RunHappyPath();
        RunValidationFailure();
        RunInventoryFailure();
        RunPaymentFailure();
        MapAndTapDemo();

        System.Console.WriteLine("\n=== Typed Error Pipeline Complete ===");
        await Task.CompletedTask;
    }

    // -------------------------------------------------------------------------
    // Domain error types — each step owns exactly one
    // -------------------------------------------------------------------------

    private sealed class ValidationError : Error
    {
        public ValidationError(string reason) : base($"Validation failed: {reason}") { }
    }

    private sealed class InventoryError : Error
    {
        public InventoryError(string sku) : base($"Out of stock: {sku}") { }
    }

    private sealed class PaymentError : Error
    {
        public PaymentError(string detail) : base($"Payment declined: {detail}") { }
    }

    private sealed class DatabaseError : Error
    {
        public DatabaseError(string detail) : base($"DB error: {detail}") { }
    }

    // -------------------------------------------------------------------------
    // Domain model
    // -------------------------------------------------------------------------

    private sealed record CheckoutRequest(string Sku, int Quantity, string CardToken);
    private sealed record Order(int Id, string Sku, int Quantity, decimal Total);

    // -------------------------------------------------------------------------
    // Pipeline steps — each returns a single concrete error, no union noise
    // -------------------------------------------------------------------------

    private static Result<CheckoutRequest, ValidationError> Validate(CheckoutRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Sku))
            return Result<CheckoutRequest, ValidationError>.Fail(new ValidationError("SKU is required"));
        if (req.Quantity <= 0)
            return Result<CheckoutRequest, ValidationError>.Fail(new ValidationError("Quantity must be > 0"));
        if (string.IsNullOrWhiteSpace(req.CardToken))
            return Result<CheckoutRequest, ValidationError>.Fail(new ValidationError("Card token is required"));

        return Result<CheckoutRequest, ValidationError>.Ok(req);
    }

    private static Result<Order, InventoryError> ReserveInventory(CheckoutRequest req)
    {
        // Simulate out-of-stock for a specific SKU
        if (req.Sku == "OUT_OF_STOCK")
            return Result<Order, InventoryError>.Fail(new InventoryError(req.Sku));

        return Result<Order, InventoryError>.Ok(new Order(0, req.Sku, req.Quantity, req.Quantity * 9.99m));
    }

    private static Result<Order, PaymentError> ProcessPayment(Order order)
    {
        // Simulate declined card
        if (order.Total > 1000m)
            return Result<Order, PaymentError>.Fail(new PaymentError("Credit limit exceeded"));

        return Result<Order, PaymentError>.Ok(order);
    }

    private static Result<Order, DatabaseError> PersistOrder(Order order)
    {
        // Simulate DB success — assign a new ID
        return Result<Order, DatabaseError>.Ok(order with { Id = 42 });
    }

    // -------------------------------------------------------------------------
    // Checkout pipeline — union grows one slot per Bind
    // -------------------------------------------------------------------------

    /// <summary>
    /// Full return type is inferred by the compiler:
    ///   Result&lt;Order, ErrorsOf&lt;ValidationError, InventoryError, PaymentError, DatabaseError&gt;&gt;
    /// </summary>
    private static Result<Order, ErrorsOf<ValidationError, InventoryError, PaymentError, DatabaseError>>
        Checkout(CheckoutRequest request) =>
            Validate(request)        // Result<CheckoutRequest, ValidationError>
                .Bind(ReserveInventory) // → Result<Order, ErrorsOf<ValidationError, InventoryError>>
                .Bind(ProcessPayment)   // → Result<Order, ErrorsOf<ValidationError, InventoryError, PaymentError>>
                .Bind(PersistOrder);    // → Result<Order, ErrorsOf<V, I, P, DatabaseError>>

    // -------------------------------------------------------------------------
    // Scenarios
    // -------------------------------------------------------------------------

    private static void RunHappyPath()
    {
        System.Console.WriteLine("1. Happy path (all steps succeed):");
        System.Console.WriteLine("------------------------------------");

        var result = Checkout(new CheckoutRequest("SKU-123", 2, "tok_valid"));

        if (result.IsSuccess)
        {
            System.Console.WriteLine($"   ✓ Order #{result.Value.Id} placed — {result.Value.Quantity}× {result.Value.Sku} = ${result.Value.Total:F2}");
        }
        else
        {
            PrintError(result.Error);
        }

        System.Console.WriteLine();
    }

    private static void RunValidationFailure()
    {
        System.Console.WriteLine("2. Validation failure (empty SKU):");
        System.Console.WriteLine("------------------------------------");

        var result = Checkout(new CheckoutRequest("", 1, "tok_valid"));

        System.Console.WriteLine($"   IsSuccess: {result.IsSuccess}");
        PrintError(result.Error);
        System.Console.WriteLine();
    }

    private static void RunInventoryFailure()
    {
        System.Console.WriteLine("3. Inventory failure (out of stock):");
        System.Console.WriteLine("-------------------------------------");

        var result = Checkout(new CheckoutRequest("OUT_OF_STOCK", 1, "tok_valid"));

        System.Console.WriteLine($"   IsSuccess: {result.IsSuccess}");
        PrintError(result.Error);
        System.Console.WriteLine();
    }

    private static void RunPaymentFailure()
    {
        System.Console.WriteLine("4. Payment failure (too expensive):");
        System.Console.WriteLine("------------------------------------");

        // 200 × $9.99 = $1998 → exceeds $1000 limit
        var result = Checkout(new CheckoutRequest("SKU-PRICEY", 200, "tok_valid"));

        System.Console.WriteLine($"   IsSuccess: {result.IsSuccess}");
        PrintError(result.Error);
        System.Console.WriteLine();
    }

    private static void MapAndTapDemo()
    {
        System.Console.WriteLine("5. Map + Tap in a typed pipeline:");
        System.Console.WriteLine("-----------------------------------");

        var logged = false;

        var result = Validate(new CheckoutRequest("SKU-DEMO", 3, "tok_abc"))
            .Tap(req => { logged = true; System.Console.WriteLine($"   [Tap] Validated request for SKU={req.Sku}"); })
            .Map(req => req with { Quantity = req.Quantity * 2 })   // double the quantity
            .Bind(ReserveInventory)
            .Tap(order => System.Console.WriteLine($"   [Tap] Reserved inventory — total=${order.Total:F2}"));

        System.Console.WriteLine($"   Tap was called: {logged}");
        System.Console.WriteLine($"   Final quantity: {result.Value.Quantity}");
        System.Console.WriteLine();
    }

    // -------------------------------------------------------------------------
    // Exhaustive Match — compile-time safe, every error type handled
    // -------------------------------------------------------------------------

    private static void PrintError(
        ErrorsOf<ValidationError, InventoryError, PaymentError, DatabaseError> error)
    {
        var message = error.Match(
            v => $"[ValidationError]  {v.Message}",
            i => $"[InventoryError]   {i.Message}",
            p => $"[PaymentError]     {p.Message}",
            d => $"[DatabaseError]    {d.Message}"
        );
        System.Console.WriteLine($"   ✗ {message}");
    }
}
