namespace tests.DTO;

/// <summary>
/// Static properties and indexers are not serialized to JSON and must not appear
/// in the generated TypeScript interface.
/// </summary>
public class StaticAndIndexerDto
{
    public static string StaticName { get; set; } = string.Empty;

    public string this[int index] => string.Empty;

    public int Id { get; set; }
}
