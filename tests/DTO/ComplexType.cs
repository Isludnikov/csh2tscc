namespace tests.DTO;

public class ComplexType<T>
{
    public int Id { get; set; }
    public int? Number { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid Guid { get; set; }
    public T? Type { get; set; }
    public Dictionary<string, Dictionary<T, SimpleGenericType<T?>?>?>? Dictionary { get; set; }
}