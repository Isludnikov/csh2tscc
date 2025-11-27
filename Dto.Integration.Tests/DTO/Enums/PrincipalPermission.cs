using System.Text.Json.Serialization;

namespace Dto.Integration.Tests.DTO.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PrincipalPermission
{
    [JsonStringEnumMemberName("Read")] Read,
    [JsonStringEnumMemberName("Write")] Write,
    [JsonStringEnumMemberName("Delete")] Delete
}