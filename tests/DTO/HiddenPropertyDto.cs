namespace tests.DTO;

public class HiddenPropertyBase
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// A property hidden with <c>new</c> is reported twice by reflection; only the most derived one
/// is serialized and only it may appear in the interface.
/// </summary>
public class HiddenPropertyDto : HiddenPropertyBase
{
    public new string Id { get; set; } = string.Empty;
}
