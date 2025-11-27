using System.Text.Json.Serialization;

namespace Dto.Integration.Tests.DTO.Enums;

public enum TaskType
{
    [JsonStringEnumMemberName("agent")] Agent,
    [JsonStringEnumMemberName("legacy")] Legacy
}