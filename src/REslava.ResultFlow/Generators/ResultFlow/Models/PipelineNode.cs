namespace REslava.ResultFlow.Generators.ResultFlow.Models
{
    internal sealed class PipelineNode
    {
        public string MethodName { get; }
        public NodeKind Kind { get; }
        public bool IsAsync => MethodName.EndsWith("Async");

        /// <summary>
        /// The success type flowing INTO this step (output type of the previous step).
        /// Null when type could not be resolved or this is the first step.
        /// </summary>
        public string? InputType { get; set; }

        /// <summary>
        /// The success type flowing OUT of this step (T in the generic return type of this call).
        /// Null when type could not be resolved.
        /// </summary>
        public string? OutputType { get; set; }

        public PipelineNode(string methodName, NodeKind kind)
        {
            MethodName = methodName;
            Kind = kind;
        }
    }
}
