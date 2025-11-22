using System.Text.Json.Serialization;

namespace tests.DTO;

public class SimpleObject
{
    public int Id { get; set; }
    [JsonPropertyName("nameCustom")]
    public string Name { get; set; }

}