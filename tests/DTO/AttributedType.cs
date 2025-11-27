using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace tests.DTO;

public class AttributedType
{
    public int Id { get; set; }
    [JsonIgnore]
    public int? Number { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Number2 { get; set; }
    public string Name { get; set; }
    [Required]
    public string? Description { get; set; }
    [JsonPropertyName("UUID")]
    public Guid Guid { get; set; }

}