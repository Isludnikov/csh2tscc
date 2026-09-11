namespace tests.DTO;

public class Wrapper<T>
{
    public T Value { get; set; } = default!;
}

/// <summary>
/// A generic constructed with another construction of itself: one file, one import — and a name
/// taken from the definition, not from the FullName that spells out every argument.
/// </summary>
public class DeepGenericDto
{
    public Wrapper<Wrapper<string>> Deep { get; set; } = null!;
    public Wrapper<int> Flat { get; set; } = null!;
    public Wrapper<Wrapper<SimpleObject>?> DeepNullable { get; set; } = null!;
}
