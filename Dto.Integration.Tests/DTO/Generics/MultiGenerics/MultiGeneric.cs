namespace Dto.Integration.Tests.DTO.Generics.MultiGenerics;

public class MultiGeneric<T, TK>
{
    public T First { get; set; }
    public TK? Second { get; set; }
}