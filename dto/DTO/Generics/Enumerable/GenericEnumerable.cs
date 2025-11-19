namespace dto.DTO.Generics.Enumerable;

public class GenericEnumerable<T>
{
    public string StrNN { get; set; }
    public string? Str { get; set; }
    public T? First { get; set; }
    public List<T?>? List { get; set; }
    public List<int?> List2 { get; set; }
    public Dictionary<string?, string?>? Dictionary { get; set; }
    public Dictionary<int?, T>? Dictionary2 { get; set; }
    public Dictionary<string?, T>? Dictionary3 { get; set; }
    public T[] Arr { get; set; }
}