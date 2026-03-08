namespace REslava.Result.Flow.Generators.ResultFlow.Models
{
    internal enum NodeKind
    {
        /// <summary>Ensure, EnsureAsync — validates a condition, can fail.</summary>
        Gatekeeper,

        /// <summary>Bind, BindAsync — transforms T to Result&lt;TOut&gt;, can fail.</summary>
        TransformWithRisk,

        /// <summary>Map, MapAsync — pure transformation, cannot fail.</summary>
        PureTransform,

        /// <summary>Tap, TapAsync — side-effect on success path.</summary>
        SideEffectSuccess,

        /// <summary>TapOnFailure, TapOnFailureAsync — side-effect on failure path.</summary>
        SideEffectFailure,

        /// <summary>TapBoth — side-effect on both paths.</summary>
        SideEffectBoth,

        /// <summary>Match, MatchAsync — terminal consumer, no outbound edges.</summary>
        Terminal,

        /// <summary>WithSuccess, WithSuccessAsync, WithError — traversed but not rendered.</summary>
        Invisible,

        /// <summary>Unrecognised method — rendered as generic operation node.</summary>
        Unknown,
    }
}
