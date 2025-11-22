namespace tests.DTO;

public class MultiGenericType<T,D>
{
    public string Name { get; set; }
    public T Type { get; set; }
    public D? Value { get; set; }
}