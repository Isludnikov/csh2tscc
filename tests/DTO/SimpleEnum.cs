using System.Text.Json.Serialization;

namespace tests.DTO;

public enum SimpleEnum
{
    Zero,
    [JsonStringEnumMemberName("Mia")]
    One,
    [JsonStringEnumMemberName("2")]
    Two,
    [JsonStringEnumMemberName("three")]
    Three,
}