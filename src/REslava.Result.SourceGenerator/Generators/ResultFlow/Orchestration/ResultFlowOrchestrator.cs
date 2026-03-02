using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using REslava.Result.SourceGenerators.Core.Interfaces;
using REslava.Result.SourceGenerators.Generators.ResultFlow.Attributes;
using REslava.Result.SourceGenerators.Generators.ResultFlow.CodeGeneration;
using System.Collections.Generic;
using System.Linq;

namespace REslava.Result.SourceGenerators.Generators.ResultFlow.Orchestration
{
    internal class ResultFlowOrchestrator : IGeneratorOrchestrator
    {
        private const string AttributeShortName = "ResultFlow";

#pragma warning disable RS2008 // Enable analyzer release tracking
        private static readonly DiagnosticDescriptor REF001 = new DiagnosticDescriptor(
            id: "REF001",
            title: "ResultFlow chain not detected",
            messageFormat: "[ResultFlow] could not extract a fluent chain from '{0}'. Only fluent-style return pipelines are supported. Diagram not generated.",
            category: "REslava.Result.Flow",
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true);
#pragma warning restore RS2008

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Stage 1: Always emit [ResultFlow] attribute (available immediately to user code)
            context.RegisterPostInitializationOutput(ctx =>
            {
                ctx.AddSource("ResultFlowAttribute.g.cs", ResultFlowAttributeGenerator.GenerateAttribute());
            });

            // Stage 2: Find method declarations decorated with [ResultFlow]
            var annotatedMethods = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: (node, _) => node is MethodDeclarationSyntax m &&
                        m.AttributeLists.SelectMany(al => al.Attributes)
                            .Any(a => a.Name.ToString().Contains(AttributeShortName)),
                    transform: (ctx, _) => (MethodDeclarationSyntax)ctx.Node)
                .Where(m => m != null);

            var compilationAndMethods = context.CompilationProvider.Combine(annotatedMethods.Collect());

            // Stage 3: Group by containing class, extract chain, render Mermaid, emit constants
            context.RegisterSourceOutput(compilationAndMethods, (spc, source) =>
            {
                var methods = source.Right;
                if (!methods.Any()) return;

                foreach (var group in methods.GroupBy(m => m.Parent))
                {
                    // Both ClassDeclarationSyntax and RecordDeclarationSyntax inherit TypeDeclarationSyntax
                    if (!(group.Key is TypeDeclarationSyntax typeDecl)) continue;

                    var className = typeDecl.Identifier.ValueText;
                    var diagrams = new List<(string methodName, string mermaid)>();

                    foreach (var methodDecl in group)
                    {
                        var chain = ResultFlowChainExtractor.Extract(methodDecl);
                        if (chain == null)
                        {
                            spc.ReportDiagnostic(Diagnostic.Create(
                                REF001,
                                methodDecl.Identifier.GetLocation(),
                                methodDecl.Identifier.ValueText));
                            continue;
                        }

                        var mermaid = ResultFlowMermaidRenderer.Render(chain);
                        diagrams.Add((methodDecl.Identifier.ValueText, mermaid));
                    }

                    if (diagrams.Count > 0)
                    {
                        var code = ResultFlowCodeGenerator.Generate(className, diagrams);
                        spc.AddSource($"{className}_Flows.g.cs", code);
                    }
                }
            });
        }
    }
}
