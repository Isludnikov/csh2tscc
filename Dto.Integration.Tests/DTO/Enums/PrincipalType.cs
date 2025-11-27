using System.Text.Json.Serialization;

namespace Dto.Integration.Tests.DTO.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PrincipalType
{
    [JsonStringEnumMemberName("None")] None,
    [JsonStringEnumMemberName("Token")] Token
}