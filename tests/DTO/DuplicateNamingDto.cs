using System.Text.Json.Serialization;
using Dto.Integration.Tests.DTO.Extensions;

namespace tests.DTO;

public class DuplicateNamingDto
{
    [JsonPropertyName("first")]
    [CustomName("second")]
    public string Name { get; set; } = string.Empty;
}
