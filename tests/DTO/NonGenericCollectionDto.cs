namespace tests.DTO;

/// <summary>
/// Non-generic subclass of a generic collection: the type itself has no generic arguments,
/// so the element type must be taken from the implemented IEnumerable&lt;T&gt;.
/// </summary>
public class CustomStringList : List<string>;

public class NonGenericCollectionDto
{
    public CustomStringList Items { get; set; } = [];
}
