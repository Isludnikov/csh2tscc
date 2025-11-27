using System.Text.Json.Serialization;

namespace Dto.Integration.Tests.DTO.Enums;

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