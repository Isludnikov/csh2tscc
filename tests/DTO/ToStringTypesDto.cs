namespace tests.DTO;

/// <summary>Types that map to a TypeScript <c>string</c> via the resolver's ToStringTypes list.</summary>
public class ToStringTypesDto
{
    public Guid Id { get; set; }
    public DateTime Created { get; set; }
    public Uri Link { get; set; } = new("https://example.com");
    public TimeSpan Duration { get; set; }
}
