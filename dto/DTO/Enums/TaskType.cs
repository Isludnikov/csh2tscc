using System.Text.Json.Serialization;

namespace dto.DTO.Enums;

public enum TaskType
{
    [JsonStringEnumMemberName("agent")] Agent,
    [JsonStringEnumMemberName("legacy")] Legacy
}