using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace REslava.Result.Analyzers.Analyzers
{
    /// <summary>
    /// RESL1005: Suggests a domain-specific error type when <c>new Error("...")</c>
    /// is used with a message that implies a well-known HTTP error category.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SuggestDomainErrorAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Descriptors.RESL1005_SuggestDomainError);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(compilationContext =>
            {
                var errorType = compilationContext.Compilation
                    .GetTypeByMetadataName("REslava.Result.Error");

                if (errorType is null)
                    return;

                compilationContext.RegisterSyntaxNodeAction(
                    ctx => AnalyzeObjectCreation(ctx, errorType),
                    SyntaxKind.ObjectCreationExpression);
            });
        }

        private static void AnalyzeObjectCreation(
            SyntaxNodeAnalysisContext context,
            INamedTypeSymbol errorType)
        {
            var creation = (ObjectCreationExpressionSyntax)context.Node;

            // Resolve the constructed type
            var typeInfo = context.SemanticModel.GetTypeInfo(creation, context.CancellationToken);
            if (typeInfo.Type is not INamedTypeSymbol createdType)
                return;

            // Only trigger on exactly Error, not on subclasses (NotFoundError, etc.)
            if (!SymbolEqualityComparer.Default.Equals(createdType, errorType))
                return;

            // Require at least one argument; first must be a string literal
            var args = creation.ArgumentList?.Arguments;
            if (args is null || args.Value.Count == 0)
                return;

            if (args.Value[0].Expression is not LiteralExpressionSyntax literal)
                return;

            if (!literal.IsKind(SyntaxKind.StringLiteralExpression))
                return;

            var message = literal.Token.ValueText.ToLowerInvariant();

            var suggestion = InferSuggestion(message);
            if (suggestion is null)
                return;

            context.ReportDiagnostic(
                Diagnostic.Create(
                    Descriptors.RESL1005_SuggestDomainError,
                    creation.GetLocation(),
                    suggestion));
        }

        private static string? InferSuggestion(string lowerMessage)
        {
            if (Contains(lowerMessage, "not found", "missing"))
                return "NotFoundError";

            if (Contains(lowerMessage, "conflict", "already exists"))
                return "ConflictError";

            if (Contains(lowerMessage, "unauthorized"))
                return "UnauthorizedError";

            if (Contains(lowerMessage, "forbidden", "access denied"))
                return "ForbiddenError";

            if (Contains(lowerMessage, "invalid", "validation"))
                return "ValidationError";

            return null;
        }

        private static bool Contains(string haystack, params string[] needles)
        {
            foreach (var needle in needles)
            {
                if (haystack.Contains(needle))
                    return true;
            }
            return false;
        }
    }
}
