using System.Text.Json.Serialization;

namespace tests.DTO;

/// <summary>
/// Enum with a member blocked by [JsonIgnore] (default condition Always) and a renamed member —
/// exercises the blocked-member branch of BuildFromEnum.
/// </summary>
public enum IgnoredEnum
{
    Visible,

    [JsonIgnore]
    Hidden,

    [JsonStringEnumMemberName("renamed")]
    Renamed
}
