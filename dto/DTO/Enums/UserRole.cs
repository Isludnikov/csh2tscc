using System.Text.Json.Serialization;

namespace dto.DTO.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRole
{
    Access,
    Admin
}