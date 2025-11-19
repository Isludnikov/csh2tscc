using System.Text.Json.Serialization;

namespace dto.DTO.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum JobType
{
    [JsonStringEnumMemberName("resetuserpassword")]
    ResetUserPassword,

    [JsonStringEnumMemberName("createrdpsession")]
    CreateRdpSession,

    [JsonStringEnumMemberName("createproxy")]
    CreateProxy
}