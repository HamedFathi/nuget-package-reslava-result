using Microsoft.CodeAnalysis.CSharp.Syntax;
using REslava.Result.SourceGenerators.Generators.ResultFlow.Models;
using System.Collections.Generic;

namespace REslava.Result.SourceGenerators.Generators.ResultFlow.CodeGeneration
{
    internal static class ResultFlowChainExtractor
    {
        private static readonly Dictionary<string, NodeKind> MethodKindMap = new Dictionary<string, NodeKind>
        {
            { "Ensure",              NodeKind.Gatekeeper },
            { "EnsureAsync",         NodeKind.Gatekeeper },
            { "Bind",                NodeKind.TransformWithRisk },
            { "BindAsync",           NodeKind.TransformWithRisk },
            { "Map",                 NodeKind.PureTransform },
            { "MapAsync",            NodeKind.PureTransform },
            { "Tap",                 NodeKind.SideEffectSuccess },
            { "TapAsync",            NodeKind.SideEffectSuccess },
            { "TapOnFailure",        NodeKind.SideEffectFailure },
            { "TapOnFailureAsync",   NodeKind.SideEffectFailure },
            { "TapBoth",             NodeKind.SideEffectBoth },
            { "Match",               NodeKind.Terminal },
            { "MatchAsync",          NodeKind.Terminal },
            { "WithSuccess",         NodeKind.Invisible },
            { "WithSuccessAsync",    NodeKind.Invisible },
            { "WithError",           NodeKind.Invisible },
            { "WithSuccessIf",       NodeKind.Invisible },
        };

        /// <summary>
        /// Walks the fluent chain in <paramref name="method"/> and returns an ordered list
        /// of <see cref="PipelineNode"/>. Returns <c>null</c> if the body cannot be parsed
        /// as a fluent chain (triggers REF001 diagnostic).
        /// </summary>
        public static IReadOnlyList<PipelineNode>? Extract(MethodDeclarationSyntax method)
        {
            var rootExpr = GetRootExpression(method);
            if (rootExpr == null) return null;

            rootExpr = Unwrap(rootExpr);

            if (!(rootExpr is InvocationExpressionSyntax))
                return null;

            // Walk the chain bottom-up, collecting method names
            var collected = new List<string>();
            ExpressionSyntax? current = rootExpr;

            while (current is InvocationExpressionSyntax invocation)
            {
                if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    collected.Add(memberAccess.Name.Identifier.ValueText);
                    current = Unwrap(memberAccess.Expression);
                }
                else
                {
                    break; // reached root call (e.g. CreateUser(cmd) or a variable)
                }
            }

            if (collected.Count == 0) return null;

            // Reverse: bottom-up → top-down order
            collected.Reverse();

            var nodes = new List<PipelineNode>(collected.Count);
            foreach (var name in collected)
            {
                var kind = MethodKindMap.TryGetValue(name, out var k) ? k : NodeKind.Unknown;
                nodes.Add(new PipelineNode(name, kind));
            }

            return nodes;
        }

        private static ExpressionSyntax? GetRootExpression(MethodDeclarationSyntax method)
        {
            // Expression body: method => expr
            if (method.ExpressionBody != null)
                return method.ExpressionBody.Expression;

            // Block body: find the last return statement
            if (method.Body != null)
            {
                ReturnStatementSyntax? lastReturn = null;
                foreach (var stmt in method.Body.Statements)
                {
                    if (stmt is ReturnStatementSyntax ret)
                        lastReturn = ret;
                }
                return lastReturn?.Expression;
            }

            return null;
        }

        private static ExpressionSyntax Unwrap(ExpressionSyntax expr)
        {
            while (true)
            {
                if (expr is AwaitExpressionSyntax awaitExpr)
                    expr = awaitExpr.Expression;
                else if (expr is ParenthesizedExpressionSyntax parenExpr)
                    expr = parenExpr.Expression;
                else
                    return expr;
            }
        }
    }
}
