using System.Reflection;

namespace csh2tscc;

public class PropertyTypeExtractionContext
{
    public required Type ClassToWrite { get; init; }
    public PropertyInfo? PropInfo { get; init; }
    public required Type PropertyType { get; init; }
    public IEnumerable<Type> AffectedTypes { get; init; } = [];
    public IEnumerable<Type> GenericTypes { get; init; } = [];
    public bool IsNullable { get; init; }
    public BooleanContainer? BooleanContainer { get; init; }
    public bool SuppressNullable { get; init; }
    public PropertyTypeExtractionContext CreateDerived(Type type, BooleanContainer container, bool isNullable)
    {
        return new PropertyTypeExtractionContext
        {
            ClassToWrite = ClassToWrite,
            PropertyType = type,
            AffectedTypes = AffectedTypes,
            GenericTypes = GenericTypes,
            PropInfo = null,
            BooleanContainer = container,
            SuppressNullable = false,
            IsNullable = isNullable
        };
    }
}


