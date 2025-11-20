using System.Collections.ObjectModel;
using System.Reflection;

namespace csh2tscc;

public static class IsNullableHelper
{
    private const string NullableAttributeName = "System.Runtime.CompilerServices.NullableAttribute";
    private const string NullableContextAttributeName = "System.Runtime.CompilerServices.NullableContextAttribute";
    public static bool IsValueTypeNullable(Type t) => t.IsValueType && Nullable.GetUnderlyingType(t) != null;
    public static bool IsNullable(Type classType, PropertyInfo property)
    {
        var memberType = property.PropertyType;
        if (memberType.IsValueType)
        {
            return Nullable.GetUnderlyingType(memberType) != null;
        }

        var nullableContext = classType.CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == NullableContextAttributeName);

        var defaultNullable = nullableContext == null ? true : (byte)nullableContext.ConstructorArguments[0].Value == 2;

        var nullable = property.CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == NullableAttributeName);
        if (nullable is { ConstructorArguments.Count: 1 })
        {
            var attributeArgument = nullable.ConstructorArguments[0];
            if (attributeArgument.ArgumentType == typeof(byte[]))
            {
                var args = (ReadOnlyCollection<CustomAttributeTypedArgument>?)attributeArgument.Value;
                if (args?.Count > 0 && args[0].ArgumentType == typeof(byte))
                {
                    return (byte?)args[0].Value == 2;
                }
            }
            else if (attributeArgument.ArgumentType == typeof(byte))
            {
                return (byte?)attributeArgument.Value == 2;
            }
        }

        return defaultNullable;
    }
    public static BooleanContainer IsNullableContainer(Type classType, PropertyInfo property)
    {
        var memberType = property.PropertyType;
        if (memberType.IsValueType)
        {
            return Nullable.GetUnderlyingType(memberType) != null ? BooleanContainer.CreateTrue() : BooleanContainer.CreateFalse();
        }

        var nullableContext = classType.CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == NullableContextAttributeName);

        var defaultNullable = nullableContext == null ? true : (byte)nullableContext.ConstructorArguments[0].Value == 2;

        var nullable = property.CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == NullableAttributeName);
        if (nullable is { ConstructorArguments.Count: 1 })
        {
            var attributeArgument = nullable.ConstructorArguments[0];
            if (attributeArgument.ArgumentType == typeof(byte[]))
            {
                var args = (ReadOnlyCollection<CustomAttributeTypedArgument>?)attributeArgument.Value;
                if (args?.Count > 0 && args[0].ArgumentType == typeof(byte))
                {
                    return new BooleanContainer(args.Select(x => (byte?)x.Value == 2).ToArray(), defaultNullable);
                }
            }
            else if (attributeArgument.ArgumentType == typeof(byte))
            {
                return (byte?)attributeArgument.Value == 2 ? BooleanContainer.CreateTrue() : BooleanContainer.CreateFalse();
            }
        }

        return defaultNullable ? BooleanContainer.CreateTrue() : BooleanContainer.CreateFalse();
    }
}