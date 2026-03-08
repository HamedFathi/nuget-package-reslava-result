using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using REslava.Result.Flow.Generators.ResultFlow.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace REslava.Result.Flow.Generators.ResultFlow.CodeGeneration
{
    /// <summary>
    /// Extracts pipeline nodes from a [ResultFlow]-decorated method using the IOperation model.
    /// Includes success type travel (IResultBase anchor) and error body scanning (IError).
    /// Falls back to syntax-only extraction when semantic model is unavailable.
    /// </summary>
    internal static class ResultFlowChainExtractor
    {
        private static readonly Dictionary<string, NodeKind> MethodKindMap = new Dictionary<string, NodeKind>
        {
            // ── REslava.Result ──────────────────────────────────────────────
            { "Ensure",            NodeKind.Gatekeeper },
            { "EnsureAsync",       NodeKind.Gatekeeper },
            { "Bind",              NodeKind.TransformWithRisk },
            { "BindAsync",         NodeKind.TransformWithRisk },
            { "Map",               NodeKind.PureTransform },
            { "MapAsync",          NodeKind.PureTransform },
            { "Tap",               NodeKind.SideEffectSuccess },
            { "TapAsync",          NodeKind.SideEffectSuccess },
            { "TapOnFailure",      NodeKind.SideEffectFailure },
            { "TapOnFailureAsync", NodeKind.SideEffectFailure },
            { "TapBoth",           NodeKind.SideEffectBoth },
            { "MapError",          NodeKind.SideEffectFailure },
            { "MapErrorAsync",     NodeKind.SideEffectFailure },
            { "Or",                NodeKind.TransformWithRisk },
            { "OrElse",            NodeKind.TransformWithRisk },
            { "OrElseAsync",       NodeKind.TransformWithRisk },
            { "Match",             NodeKind.Terminal },
            { "MatchAsync",        NodeKind.Terminal },
            { "WithSuccess",       NodeKind.Invisible },
            { "WithSuccessAsync",  NodeKind.Invisible },
            { "WithError",         NodeKind.Invisible },
            { "WithSuccessIf",     NodeKind.Invisible },
        };

        private static readonly IReadOnlyCollection<string> EmptyErrors =
            new ReadOnlyCollection<string>(new List<string>());

        /// <summary>
        /// Extracts an ordered list of pipeline nodes from the method body.
        /// Uses IOperation chain walking (IInvocationOperation.Instance) for semantic richness.
        /// Returns null if the body cannot be parsed as a fluent chain.
        /// </summary>
        public static IReadOnlyList<PipelineNode>? Extract(
            MethodDeclarationSyntax method,
            SemanticModel semanticModel,
            Compilation compilation,
            INamedTypeSymbol? resultBaseSymbol,
            INamedTypeSymbol? iErrorSymbol,
            IReadOnlyDictionary<string, NodeKind>? customMappings = null)
        {
            var rootExpr = GetRootExpression(method);
            if (rootExpr == null) return null;

            rootExpr = UnwrapSyntax(rootExpr);

            if (!(rootExpr is InvocationExpressionSyntax)) return null;

            // Get outermost IInvocationOperation via semantic model
            var outermostOp = semanticModel.GetOperation(rootExpr) as IInvocationOperation;

            if (outermostOp == null)
                return ExtractSyntaxOnly(rootExpr as InvocationExpressionSyntax, customMappings);

            // Walk the chain via IInvocationOperation.Instance (plan §6)
            var ops = new List<IInvocationOperation>();
            var current = outermostOp;
            while (current != null)
            {
                ops.Add(current);
                current = current.Instance as IInvocationOperation;
            }
            ops.Reverse(); // root-first order

            var nodes = new List<PipelineNode>(ops.Count);
            string? previousOutputType = null;

            for (int i = 0; i < ops.Count; i++)
            {
                var step = ops[i];
                var methodName = step.TargetMethod?.Name ?? step.Syntax.ToString();

                // Resolve NodeKind: custom mappings override built-ins
                NodeKind kind;
                if (customMappings != null && customMappings.TryGetValue(methodName, out var custom))
                    kind = custom;
                else if (MethodKindMap.TryGetValue(methodName, out var builtin))
                    kind = builtin;
                else
                    kind = NodeKind.Unknown;

                // Success type via IResultBase anchor
                string? outputType = null;
                if (resultBaseSymbol != null)
                    outputType = ResultTypeExtractor.GetSuccessType(step, resultBaseSymbol);

                // Error types via IError body scanning (skip PureTransform and Invisible — they cannot fail)
                IReadOnlyCollection<string> possibleErrors = EmptyErrors;
                if (kind != NodeKind.PureTransform && kind != NodeKind.Invisible && iErrorSymbol != null)
                    possibleErrors = ResultTypeExtractor.GetPossibleErrors(step, compilation, iErrorSymbol);

                nodes.Add(new PipelineNode(methodName, kind)
                {
                    InputType = previousOutputType,
                    OutputType = outputType,
                    PossibleErrors = possibleErrors,
                });

                previousOutputType = outputType;
            }

            return nodes.Count == 0 ? null : nodes;
        }

        // ── Syntax-only fallback (no type info, no error info) ───────────────

        private static IReadOnlyList<PipelineNode>? ExtractSyntaxOnly(
            InvocationExpressionSyntax? rootInvocation,
            IReadOnlyDictionary<string, NodeKind>? customMappings)
        {
            if (rootInvocation == null) return null;

            var collected = new List<string>();
            ExpressionSyntax? current = rootInvocation;

            while (current is InvocationExpressionSyntax inv)
            {
                if (inv.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    collected.Add(memberAccess.Name.Identifier.ValueText);
                    current = UnwrapSyntax(memberAccess.Expression);
                }
                else
                    break;
            }

            if (collected.Count == 0) return null;
            collected.Reverse();

            var nodes = new List<PipelineNode>(collected.Count);
            foreach (var name in collected)
            {
                NodeKind kind;
                if (customMappings != null && customMappings.TryGetValue(name, out var custom))
                    kind = custom;
                else if (MethodKindMap.TryGetValue(name, out var builtin))
                    kind = builtin;
                else
                    kind = NodeKind.Unknown;

                nodes.Add(new PipelineNode(name, kind));
            }

            return nodes;
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private static ExpressionSyntax? GetRootExpression(MethodDeclarationSyntax method)
        {
            if (method.ExpressionBody != null)
                return method.ExpressionBody.Expression;

            if (method.Body != null)
            {
                ReturnStatementSyntax? lastReturn = null;
                foreach (var stmt in method.Body.Statements)
                    if (stmt is ReturnStatementSyntax ret) lastReturn = ret;
                return lastReturn?.Expression;
            }

            return null;
        }

        private static ExpressionSyntax UnwrapSyntax(ExpressionSyntax expr)
        {
            while (true)
            {
                if (expr is AwaitExpressionSyntax awaitExpr) expr = awaitExpr.Expression;
                else if (expr is ParenthesizedExpressionSyntax parenExpr) expr = parenExpr.Expression;
                else return expr;
            }
        }
    }
}
