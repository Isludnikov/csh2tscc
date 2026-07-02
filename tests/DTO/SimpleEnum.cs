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
    // Apostrophe in the serialized name must be escaped in the single-quoted TS literal.
    [JsonStringEnumMemberName("O'Brien")]
    Four
}