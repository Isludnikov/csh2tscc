using System.Runtime.Serialization;

namespace dto.DTO.Enums;

public enum DeploySource
{
    [EnumMember(Value = "Gitlab")] Gitlab,
    [EnumMember(Value = "Teamcity")] Teamcity
}