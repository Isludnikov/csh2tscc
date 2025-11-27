using System.Text.Json.Serialization;

namespace Dto.Integration.Tests.DTO.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserType
{
    Local,
    Ldap
}