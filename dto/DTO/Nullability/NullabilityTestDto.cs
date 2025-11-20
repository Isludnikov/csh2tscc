namespace dto.DTO.Nullability;

public class NullabilityTestDto
{
    public string Name { get; set; }
    public Dictionary<string, List<string?>?>? TestDictionary { get; set; }
    public List<string?> TestList { get; set; }
}