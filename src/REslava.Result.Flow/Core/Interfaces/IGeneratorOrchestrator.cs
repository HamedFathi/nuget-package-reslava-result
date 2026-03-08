using Microsoft.CodeAnalysis;

namespace REslava.Result.Flow.Core.Interfaces
{
    internal interface IGeneratorOrchestrator
    {
        void Initialize(IncrementalGeneratorInitializationContext context);
    }
}
