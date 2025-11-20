namespace dto.DTO.Generics.Enumerable;

public class UsingGenericEnumerable
{
    public string Name { get; set; }
    public GenericEnumerable<List<long>> Description { get; set; }
    public Dictionary<string, List<string?>>? TestDictionary { get; set; }
    public List<string?>? TestList { get; set; }
}