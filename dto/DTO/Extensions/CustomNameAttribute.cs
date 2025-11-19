namespace dto.DTO.Extensions;

public class CustomNameAttribute(string customName) : Attribute
{
    public string CustomName { get; private set; } = customName;
}