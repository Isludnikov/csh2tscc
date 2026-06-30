// Declared in the global namespace on purpose (no namespace declaration): this is the only way
// to produce a Type whose Namespace is null, which exercises the TypeDiscovery branch that
// excludes such types from the affected-types list.
public sealed class GlobalNamespaceProbe
{
    public int Value { get; set; }
}
