namespace Dto.Integration.Tests.DTO.Generics.SimpleGenerics;

public class SimpleGeneric<T>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public string TypeDescription { get; set; }
    public T? Data { get; set; }
}