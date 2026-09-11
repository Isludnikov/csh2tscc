using System.Collections.Frozen;
using System.Reflection;

namespace csh2tscc;

public static class CommonHelper
{
    private static readonly FrozenSet<string> PreventNullAttributes = ["System.ComponentModel.DataAnnotations.RequiredAttribute", "System.Diagnostics.CodeAnalysis.NotNullAttribute"];
    internal static string GetPropertyTypeWithNullable(string strType, bool isNullable) => strType + (isNullable ? TypeScriptConstants.NullableUnion : string.Empty);

    /// <summary>
    /// Public instance properties that take part in serialization: static properties and
    /// indexers are never serialized to JSON and must not appear in generated interfaces.
    /// </summary>
    /// <remarks>
    /// Reflection lists a property hidden with <c>new</c> twice (once per declaring type); only
    /// the most derived one is serialized, so only it is kept. An interface does not report the
    /// members of the interfaces it extends, so those are gathered explicitly.
    /// </remarks>
    internal static PropertyInfo[] GetSerializableProperties(Type type)
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
        IEnumerable<PropertyInfo> properties = type.GetProperties(flags);
        if (type.IsInterface)
        {
            properties = properties.Concat(type.GetInterfaces().SelectMany(i => i.GetProperties(flags)));
        }

        return properties
            .Where(property => property.GetIndexParameters().Length == 0)
            .GroupBy(property => property.Name)
            .Select(group => group.OrderByDescending(property => InheritanceDepth(property.DeclaringType)).First())
            .ToArray();
    }

    private static int InheritanceDepth(Type? type)
    {
        var depth = 0;
        for (var current = type; current != null; current = current.BaseType)
        {
            ++depth;
        }

        return depth;
    }
    internal static bool HasNonNullableAttribute(PropertyInfo property)
    {
        var preventAttributeExists =
            property.GetCustomAttributes().Any(x => PreventNullAttributes.Contains(x.GetType().FullName ?? string.Empty));
        if (preventAttributeExists)
        {
            return true;
        }

        var getMethod = property.GetGetMethod();
        return getMethod != null && getMethod.ReturnParameter.GetCustomAttributes().Any(x => PreventNullAttributes.Contains(x.GetType().FullName ?? string.Empty));
    }
}