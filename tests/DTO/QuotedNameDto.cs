using System.Text.Json.Serialization;

namespace tests.DTO;

/// <summary>
/// Serialization names that are not TypeScript identifiers have to be quoted in the interface.
/// </summary>
public class QuotedNameDto
{
    [JsonPropertyName("kebab-name")]
    public string Kebab { get; set; } = string.Empty;

    [JsonPropertyName("with space")]
    public int Spaced { get; set; }

    [JsonPropertyName("123start")]
    public int Digits { get; set; }

    [JsonPropertyName("it's")]
    public int Apostrophe { get; set; }

    [JsonPropertyName("$ok_1")]
    public int Plain { get; set; }
}
