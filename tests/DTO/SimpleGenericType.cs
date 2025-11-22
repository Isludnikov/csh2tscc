namespace tests.DTO;

public class SimpleGenericType<T>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public T Data { get; set; }
}