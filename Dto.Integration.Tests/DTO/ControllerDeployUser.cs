namespace Dto.Integration.Tests.DTO;

public class ControllerDeployUser
{
    public int Id { get; init; }
    public string ProjectName { get; set; }
    public string Account { get; init; }
    public string DisplayName { get; init; }
    public string? Data { get; set; }
}