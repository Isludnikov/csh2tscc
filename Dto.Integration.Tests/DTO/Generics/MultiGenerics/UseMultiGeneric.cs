namespace Dto.Integration.Tests.DTO.Generics.MultiGenerics;

public class UseMultiGeneric
{
    public int Id { get; set; }
    public string Name { get; set; }
    public MultiGeneric<string?, string>? data { get; set; }
    public Dictionary<string?, Dictionary<int?, SimpleDataObject?>>? Dict { get; set; }
}