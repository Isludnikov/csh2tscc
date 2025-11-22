namespace tests.DTO;

public class AffectedType<T>
{
    public Dictionary<string, Dictionary<T, SimpleGenericType<T?>?>?>? Dictionary { get; set; }
}

