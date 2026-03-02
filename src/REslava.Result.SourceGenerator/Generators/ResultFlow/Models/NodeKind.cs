namespace REslava.Result.SourceGenerators.Generators.ResultFlow.Models
{
    internal enum NodeKind
    {
        Gatekeeper,        // Ensure, EnsureAsync
        TransformWithRisk, // Bind, BindAsync
        PureTransform,     // Map, MapAsync
        SideEffectSuccess, // Tap, TapAsync
        SideEffectFailure, // TapOnFailure, TapOnFailureAsync
        SideEffectBoth,    // TapBoth
        Terminal,          // Match, MatchAsync
        Invisible,         // WithSuccess, WithSuccessAsync — traversed, not rendered
        Unknown,           // unrecognised method — render as generic "Operation" node
    }
}
