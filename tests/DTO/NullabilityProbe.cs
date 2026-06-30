namespace tests.DTO;

/// <summary>
/// Top-level DTO carrying a representative mix of value/reference and nullable/non-nullable
/// members so the C# compiler emits realistic NullableContext/Nullable metadata
/// (a nested helper class does not get the same attributes).
/// </summary>
public class NullabilityProbe
{
    public int Value { get; set; }
    public int? NullableValue { get; set; }
    public string Reference { get; set; } = "";
    public string? NullableReference { get; set; }
    public List<string?> ListOfNullable { get; set; } = [];
    public List<string> ListOfNonNullable { get; set; } = [];
}
