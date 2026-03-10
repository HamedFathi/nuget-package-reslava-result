using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using REslava.ResultFlow.Generators.ResultFlow.Models;
using System.Collections.Generic;

namespace REslava.ResultFlow.Generators.ResultFlow.CodeGeneration
{
    internal static class ResultFlowChainExtractor
    {
        private static readonly Dictionary<string, NodeKind> MethodKindMap = new Dictionary<string, NodeKind>
        {
            // ── REslava.Result ────────────────────────────────────────────────
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
            { "MapError",            NodeKind.SideEffectFailure },
            { "MapErrorAsync",       NodeKind.SideEffectFailure },
            { "Or",                  NodeKind.TransformWithRisk },
            { "OrElse",              NodeKind.TransformWithRisk },
            { "OrElseAsync",         NodeKind.TransformWithRisk },
            { "Match",               NodeKind.Terminal },
            { "MatchAsync",          NodeKind.Terminal },
            { "WithSuccess",         NodeKind.Invisible },
            { "WithSuccessAsync",    NodeKind.Invisible },
            { "WithError",           NodeKind.Invisible },
            { "WithSuccessIf",       NodeKind.Invisible },

            // ── ErrorOr ───────────────────────────────────────────────────────
            { "Then",                NodeKind.TransformWithRisk },
            { "ThenAsync",           NodeKind.TransformWithRisk },
            { "Switch",              NodeKind.Terminal },
            { "SwitchAsync",         NodeKind.Terminal },

            // ── LanguageExt ───────────────────────────────────────────────────
            { "Filter",              NodeKind.Gatekeeper },
            { "Do",                  NodeKind.SideEffectSuccess },
            { "DoAsync",             NodeKind.SideEffectSuccess },
            { "DoLeft",              NodeKind.SideEffectFailure },
            { "DoLeftAsync",         NodeKind.SideEffectFailure },
        };

        /// <summary>
        /// Walks the fluent chain in <paramref name="method"/> and returns an ordered list
        /// of <see cref="PipelineNode"/>. Returns <c>null</c> if the body cannot be parsed
        /// as a fluent chain (triggers REF001 diagnostic).
        /// </summary>
        /// <param name="method">The method declaration to analyse.</param>
        /// <param name="semanticModel">
        /// Optional semantic model used to extract generic type arguments from each step's return type.
        /// When provided, nodes are populated with <see cref="PipelineNode.InputType"/> and
        /// <see cref="PipelineNode.OutputType"/> for inline type-travel labels in the Mermaid diagram.
        /// </param>
        /// <param name="customMappings">
        /// Optional entries loaded from <c>resultflow.json</c>.
        /// These <b>override</b> the built-in convention dictionary — allowing full substitution
        /// of any built-in classification for custom or non-standard libraries.
        /// </param>
        public static IReadOnlyList<PipelineNode>? Extract(
            MethodDeclarationSyntax method,
            SemanticModel? semanticModel = null,
            IReadOnlyDictionary<string, NodeKind>? customMappings = null)
        {
            var rootExpr = GetRootExpression(method);
            if (rootExpr == null) return null;

            rootExpr = Unwrap(rootExpr);

            if (!(rootExpr is InvocationExpressionSyntax))
                return null;

            // Walk the chain bottom-up, collecting (method name, invocation node) pairs
            var collected = new List<(string name, InvocationExpressionSyntax invocationNode)>();
            ExpressionSyntax? current = rootExpr;

            while (current is InvocationExpressionSyntax invocation)
            {
                if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    collected.Add((memberAccess.Name.Identifier.ValueText, invocation));
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

            // Seed prevOutputType from the root expression (the call before the chain begins)
            // e.g. CreateUser() in  CreateUser().Bind(SaveUser).Map(ToDto)
            string? prevOutputType = null;
            if (semanticModel != null && current is InvocationExpressionSyntax rootCall)
                prevOutputType = GenericTypeExtractor.GetFirstTypeArgument(rootCall, semanticModel);

            var nodes = new List<PipelineNode>(collected.Count);

            foreach (var (name, invocationNode) in collected)
            {
                // customMappings override built-ins (explicit user config wins)
                NodeKind kind;
                if (customMappings != null && customMappings.TryGetValue(name, out var custom))
                    kind = custom;
                else if (MethodKindMap.TryGetValue(name, out var builtin))
                    kind = builtin;
                else
                    kind = NodeKind.Unknown;

                string? outputType = null;
                string? errorType = null;
                if (semanticModel != null)
                {
                    outputType = GenericTypeExtractor.GetFirstTypeArgument(invocationNode, semanticModel);
                    errorType  = GenericTypeExtractor.GetErrorTypeArgument(invocationNode, semanticModel);
                }

                var node = new PipelineNode(name, kind)
                {
                    InputType = prevOutputType,
                    OutputType = outputType,
                    ErrorType  = errorType
                };

                nodes.Add(node);
                prevOutputType = outputType;
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
