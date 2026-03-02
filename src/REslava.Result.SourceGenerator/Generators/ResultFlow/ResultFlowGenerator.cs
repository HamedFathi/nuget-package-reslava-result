using Microsoft.CodeAnalysis;
using REslava.Result.SourceGenerators.Core.Interfaces;
using REslava.Result.SourceGenerators.Generators.ResultFlow.Orchestration;

namespace REslava.Result.SourceGenerators.Generators.ResultFlow
{
    [Generator]
    public class ResultFlowGenerator : IIncrementalGenerator
    {
        private readonly IGeneratorOrchestrator _orchestrator = new ResultFlowOrchestrator();

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            _orchestrator.Initialize(context);
        }
    }
}
