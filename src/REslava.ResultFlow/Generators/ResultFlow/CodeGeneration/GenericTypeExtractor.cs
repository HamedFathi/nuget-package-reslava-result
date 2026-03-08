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
    }
}
