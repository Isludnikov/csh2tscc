namespace Dto.Integration.Tests.DTO;

public record ControllerDeployGroup
{
    public int? Key { get; init; }
    public required string GroupName { get; init; }
    public required string Environment { get; init; }
    public required int[] DeployGroupServers { get; init; }
    public string? Description { get; init; }
}