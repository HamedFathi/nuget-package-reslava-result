using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace REslava.ResultFlow.Generators.ResultFlow.CodeGeneration
{
    /// <summary>
    /// Extracts the first type argument from the return type of an invocation expression.
    /// Works for any single-generic-parameter wrapper (Result&lt;T&gt;, ErrorOr&lt;T&gt;, Fin&lt;T&gt;, etc.).
    /// No dependency on REslava.Result — purely generic Roslyn analysis.
    /// </summary>
    internal static class GenericTypeExtractor
    {
        /// <summary>
        /// Returns the name of the first type argument of the invocation's return type,
        /// or <c>null</c> if the return type is non-generic or cannot be resolved.
        /// Transparently unwraps <c>Task&lt;T&gt;</c>.
        /// </summary>
        public static string? GetFirstTypeArgument(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel)
        {
            var symbol = semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            if (symbol is null) return null;

            ITypeSymbol returnType = symbol.ReturnType;

            // Unwrap Task<T>
            if (returnType is INamedTypeSymbol taskType &&
                taskType.Name == "Task" &&
                taskType.TypeArguments.Length == 1)
            {
                returnType = taskType.TypeArguments[0];
            }

            if (returnType is INamedTypeSymbol named && named.TypeArguments.Length >= 1)
                return named.TypeArguments[0].Name;

            return null;
        }

        /// <summary>
        /// Returns a formatted display string for <c>TError</c> when the invocation's return type
        /// is <c>Result&lt;T, TError&gt;</c> (type-read mode). Returns <c>null</c> for
        /// <c>Result&lt;T&gt;</c> or any non-Result type.
        /// Transparently unwraps <c>Task&lt;T&gt;</c>.
        /// </summary>
        /// <remarks>
        /// When <c>TError</c> is itself generic (e.g. <c>ErrorsOf&lt;ValidationError, InventoryError&gt;</c>),
        /// the returned string includes the type arguments:
        /// <c>ErrorsOf&lt;ValidationError, InventoryError&gt;</c>.
        /// </remarks>
        public static string? GetErrorTypeArgument(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel)
        {
            var symbol = semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            if (symbol is null) return null;

            ITypeSymbol returnType = symbol.ReturnType;

            // Unwrap Task<T>
            if (returnType is INamedTypeSymbol taskType &&
                taskType.Name == "Task" &&
                taskType.TypeArguments.Length == 1)
            {
                returnType = taskType.TypeArguments[0];
            }

            // Type-read mode: Result<T, TError> has exactly 2 type arguments
            if (returnType is INamedTypeSymbol named &&
                named.Name == "Result" &&
                named.TypeArguments.Length == 2)
            {
                return FormatType(named.TypeArguments[1]);
            }

            return null;
        }

        /// <summary>
        /// Formats a type symbol as a readable string, expanding one level of generic arguments.
        /// E.g. <c>ErrorsOf&lt;ValidationError, InventoryError&gt;</c>.
        /// </summary>
        private static string FormatType(ITypeSymbol type)
        {
            if (type is INamedTypeSymbol named && named.TypeArguments.Length > 0)
            {
                var sb = new System.Text.StringBuilder();
                sb.Append(named.Name);
                sb.Append('<');
                for (int i = 0; i < named.TypeArguments.Length; i++)
                {
                    if (i > 0) sb.Append(", ");
                    sb.Append(named.TypeArguments[i].Name);
                }
                sb.Append('>');
                return sb.ToString();
            }

            return type.Name;
        }
    }
}
