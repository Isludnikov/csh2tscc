namespace tests.DTO;

/// <summary>A bare <c>object</c> property maps to the TypeScript <c>unknown</c> type.</summary>
public class ObjectPropertyDto
{
    public int Id { get; set; }
    public object Payload { get; set; } = new();
}
