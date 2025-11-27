namespace Dto.Integration.Tests.DTO.Extensions;

public class CustomNameAttribute(string customName) : Attribute
{
    public string CustomName { get; private set; } = customName;
}