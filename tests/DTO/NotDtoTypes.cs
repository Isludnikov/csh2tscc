namespace tests.DTO;

/// <summary>Types the namespace selection reaches but that cannot be DTOs, and types that can.</summary>
public delegate void NotADtoDelegate(int value);

public static class NotADtoStaticClass
{
    public static int Counter { get; set; }
}

public abstract class AbstractDtoBase
{
    public int Id { get; set; }
}

public record RecordDto(int Id, string? Name);

public struct StructDto
{
    public int X { get; set; }
}
