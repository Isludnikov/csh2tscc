namespace tests.DTO;

public class InheritedContextBase
{
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
}

/// <summary>
/// Has no reference-type members of its own, hence no NullableContext of its own. The inherited
/// properties were compiled under the base class's context and must keep their annotations.
/// </summary>
public class InheritedContextDto : InheritedContextBase
{
    public int Count { get; set; }
}
