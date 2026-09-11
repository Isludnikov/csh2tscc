using System.Text.Json.Serialization;
using Dto.Integration.Tests.DTO.Extensions;

namespace tests.DTO;

/// <summary>
/// Enum member names that need escaping in a single-quoted literal, and a naming attribute that
/// yields null — for an enum member that falls back to the field name instead of failing.
/// </summary>
public enum EscapedEnum
{
    [JsonStringEnumMemberName(@"back\slash")]
    Backslash,

    [CustomName(null!)]
    NullNamed,

    Plain
}
