namespace tests.DTO;

/// <summary>
/// References <see cref="GlobalNamespaceProbe"/> (a global-namespace type) so that affected-type
/// discovery walks into a property whose type has a null <see cref="System.Type.Namespace"/>.
/// </summary>
public class GlobalProbeHostDto
{
    public GlobalNamespaceProbe Probe { get; set; } = new();
}
