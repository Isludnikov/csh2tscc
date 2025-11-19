using System.Text.Json.Serialization;

namespace dto.DTO.Generics.SimpleGenerics;

public enum MockEnum
{
    [JsonStringEnumMemberName("none")] None,
    [JsonStringEnumMemberName("default")] Default
}