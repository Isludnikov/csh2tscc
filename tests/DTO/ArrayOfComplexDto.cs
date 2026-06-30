namespace tests.DTO;

/// <summary>An array of a DTO type must emit an element array plus an import for the element type.</summary>
public class ArrayOfComplexDto
{
    public SimpleObject[] Items { get; set; } = [];
}
