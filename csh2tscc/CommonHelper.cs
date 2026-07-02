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
    internal static PropertyInfo[] GetSerializableProperties(Type type) =>
        type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.GetIndexParameters().Length == 0)
            .ToArray();
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