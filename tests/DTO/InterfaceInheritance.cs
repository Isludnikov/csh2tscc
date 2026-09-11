namespace tests.DTO;

public interface IBaseShape
{
    string Name { get; }
    int Id { get; }
}

/// <summary>
/// Reflection does not list inherited members on an interface; the generated interface must
/// still carry them, or a consumer loses half of what arrives on the wire.
/// </summary>
public interface IDerivedShape : IBaseShape
{
    int Age { get; }
}
