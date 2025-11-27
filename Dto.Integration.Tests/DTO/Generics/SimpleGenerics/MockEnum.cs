using System.Text.Json.Serialization;

namespace Dto.Integration.Tests.DTO.Generics.SimpleGenerics;

public enum MockEnum
{
    [JsonStringEnumMemberName("none")] None,
    [JsonStringEnumMemberName("default")] Default
}