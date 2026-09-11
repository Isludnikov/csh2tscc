using System.Collections.ObjectModel;
using System.Reflection;

namespace csh2tscc;

public static class IsNullableHelper
{

    public static bool IsValueTypeNullable(Type t) => t.IsValueType && Nullable.GetUnderlyingType(t) != null;

    /// <summary>
    /// Gets the default nullable setting from the NullableContext attribute of the type that
    /// declares the member or, when it has none, of the nearest declaring type: the compiler emits
    /// the attribute once on the outermost type and lets nested types inherit it. Returns true
    /// (nullable) if no context is specified anywhere up the chain.
    /// </summary>
    /// <remarks>
    /// The declaring type, not the type being converted: an inherited property (from a base class
    /// or an extended interface) was compiled under the context of the type that declares it.
    /// </remarks>
    private static bool GetDefaultNullable(Type classType)
    {
        for (var type = classType; type != null; type = type.DeclaringType)
        {
            var nullableContext = type.CustomAttributes
                .FirstOrDefault(x => x.AttributeType.FullName == WellKnownNames.NullableContextAttributeName);

            if (nullableContext != null)
            {
                return (byte)nullableContext.ConstructorArguments[0].Value! == NullabilityConstants.Nullable;
            }
        }

        return true;
    }

    /// <summary>
    /// Extracts nullable flags from the property's Nullable attribute.
    /// Returns null if no valid nullable attribute is found.
    /// </summary>
    private static bool[]? TryGetNullableFlags(PropertyInfo property)
    {
        var nullable = property.CustomAttributes
            .FirstOrDefault(x => x.AttributeType.FullName == WellKnownNames.NullableAttributeName);

        if (nullable is not { ConstructorArguments.Count: 1 })
            return null;

        var attributeArgument = nullable.ConstructorArguments[0];

        // Handle byte[] argument (multiple nullable flags for generic types)
        if (attributeArgument.ArgumentType == typeof(byte[]))
        {
            var args = (ReadOnlyCollection<CustomAttributeTypedArgument>?)attributeArgument.Value;
            if (args?.Count > 0 && args[0].ArgumentType == typeof(byte))
            {
                return args.Select(x => (byte?)x.Value == NullabilityConstants.Nullable).ToArray();
            }
        }
        // Handle single byte argument (uniform nullability)
        else if (attributeArgument.ArgumentType == typeof(byte))
        {
            var isNullable = (byte?)attributeArgument.Value == NullabilityConstants.Nullable;
            return [isNullable];
        }

        return null;
    }

    public static bool IsNullable(Type classType, PropertyInfo property)
    {
        // Value types use Nullable<T> wrapper
        if (property.PropertyType.IsValueType)
            return Nullable.GetUnderlyingType(property.PropertyType) != null;

        var defaultNullable = GetDefaultNullable(property.DeclaringType ?? classType);
        var flags = TryGetNullableFlags(property);

        // First flag (index 0) indicates the property type's own nullability
        return flags?.Length > 0 ? flags[0] : defaultNullable;
    }

    public static BooleanContainer IsNullableContainer(Type classType, PropertyInfo property)
    {
        var defaultNullable = GetDefaultNullable(property.DeclaringType ?? classType);
        var flags = TryGetNullableFlags(property);

        // The flags describe the type arguments of a value type too: a struct Pair<string?, string>
        // is written as [0, 2, 1] (0 for the struct itself, then its arguments).
        if (flags != null)
            return new BooleanContainer(flags, defaultNullable);

        // Value types use Nullable<T> wrapper
        if (property.PropertyType.IsValueType)
        {
            return Nullable.GetUnderlyingType(property.PropertyType) != null
                ? BooleanContainer.CreateTrue()
                : BooleanContainer.CreateFalse();
        }

        return defaultNullable ? BooleanContainer.CreateTrue() : BooleanContainer.CreateFalse();
    }
}