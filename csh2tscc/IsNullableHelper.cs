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
            return Nullable.GetUnderlyingType(memberType) != null;

        var nullableContext = classType.CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == NullableContextAttributeName);
        if (nullableContext == null) return true; //Если контекст не установлен все ссылочные типы по умолчанию обнуляемы

        var defaultNullable = (byte)nullableContext.ConstructorArguments[0].Value == 2;

        var nullable = property.CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == NullableAttributeName);
        if (nullable is { ConstructorArguments.Count: 1 })
        {
            var attributeArgument = nullable.ConstructorArguments[0];
            if (attributeArgument.ArgumentType == typeof(byte[]))
            {
                var args = (ReadOnlyCollection<CustomAttributeTypedArgument>?)attributeArgument.Value;
                if (args?.Count > 0 && args[0].ArgumentType == typeof(byte)) return (byte?)args[0].Value == 2;
            }
            else if (attributeArgument.ArgumentType == typeof(byte))
            {
                return (byte?)attributeArgument.Value == 2;
            }
        }

        return defaultNullable;
    }
    public static List<bool> IsNullableArray(Type classType, PropertyInfo property)
    {
        var memberType = property.PropertyType;
        if (memberType.IsValueType)
            return Nullable.GetUnderlyingType(memberType) != null ? [true] : [false];

        var nullableContext = classType.CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == NullableContextAttributeName);

        var defaultNullable = nullableContext != null ? (byte)nullableContext.ConstructorArguments[0].Value == 2 : true;

        var nullable = property.CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == NullableAttributeName);
        if (nullable is { ConstructorArguments.Count: 1 })
        {
            var attributeArgument = nullable.ConstructorArguments[0];
            if (attributeArgument.ArgumentType == typeof(byte[]))
            {
                var args = (ReadOnlyCollection<CustomAttributeTypedArgument>?)attributeArgument.Value;
                if (args?.Count > 0 && args[0].ArgumentType == typeof(byte))
                    return args.Select(x => (byte?)x.Value == 2).ToList();
            }
            else if (attributeArgument.ArgumentType == typeof(byte))
            {
                return (byte?)attributeArgument.Value == 2 ? [true] : [false];
            }
        }

        return defaultNullable ? [true] : [false];
    }
}