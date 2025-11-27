using System.Runtime.Serialization;

namespace Dto.Integration.Tests.DTO.Enums;

public enum DeploySource
{
    [EnumMember(Value = "Gitlab")] Gitlab,
    [EnumMember(Value = "Teamcity")] Teamcity
}