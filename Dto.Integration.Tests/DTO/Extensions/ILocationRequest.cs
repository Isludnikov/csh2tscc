namespace Dto.Integration.Tests.DTO.Extensions;

public interface ILocationRequest
{
    string Enviroment { get; set; }
    string? WhiteList { get; init; }
}