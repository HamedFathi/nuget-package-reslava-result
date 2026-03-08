using Microsoft.CodeAnalysis;
using REslava.Result.Flow.Core.Interfaces;
using REslava.Result.Flow.Generators.ResultFlow.Orchestration;

namespace REslava.Result.Flow.Generators.ResultFlow
{
    /// <summary>
    /// REslava.Result-native source generator.
    /// Uses IResultBase and IError as Roslyn anchors to infer success types and
    /// scan method bodies for possible error types in pipeline diagrams.
    /// </summary>
    [Generator]
    public class ResultFlowGenerator : IIncrementalGenerator
    {
        private readonly IGeneratorOrchestrator _orchestrator = new ResultFlowOrchestrator();

        /// <inheritdoc/>
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            _orchestrator.Initialize(context);
        }
    }
}
