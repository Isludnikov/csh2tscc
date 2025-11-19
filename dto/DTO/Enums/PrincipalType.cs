using System.Text.Json.Serialization;

namespace dto.DTO.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PrincipalType
{
    [JsonStringEnumMemberName("None")] None,
    [JsonStringEnumMemberName("Token")] Token
}