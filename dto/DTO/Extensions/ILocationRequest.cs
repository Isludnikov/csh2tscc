namespace dto.DTO.Extensions;

public interface ILocationRequest
{
    public string Enviroment { get; set; }
    public string? WhiteList { get; init; }
}