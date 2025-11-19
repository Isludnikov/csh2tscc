using System.Text.Json.Serialization;

namespace dto.DTO.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PrincipalPermission
{
    [JsonStringEnumMemberName("Read")] Read,
    [JsonStringEnumMemberName("Write")] Write,
    [JsonStringEnumMemberName("Delete")] Delete
}