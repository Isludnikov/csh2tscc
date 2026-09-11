namespace tests.DTO;

public struct Pair<TA, TB>
{
    public TA A { get; set; }
    public TB B { get; set; }
}

/// <summary>
/// A generic struct as a root property: the nullable flags are [0, 2, 1] — 0 for the struct
/// itself, then one per reference-type argument — and used to be discarded because the property
/// type is a value type.
/// </summary>
public class GenericStructDto
{
    public Pair<string?, string> Pair { get; set; }
    public Pair<int, string?> PairWithValue { get; set; }
    public Pair<string, string>? NullablePair { get; set; }
}
