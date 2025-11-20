using System.Collections.Frozen;
using System.Reflection;

namespace csh2tscc;

public static class CommonHelper
{
    private static readonly FrozenSet<string> PreventNullAttributes = ["System.ComponentModel.DataAnnotations.RequiredAttribute", "System.Diagnostics.CodeAnalysis.NotNullAttribute"];
    internal static string GetPropertyTypeWithNullable(string strType, bool isNullable) => strType + (isNullable ? " | null" : string.Empty);
    internal static bool HasNonNullableAttribute(PropertyInfo property)
    {
        var preventAttributeExists =
            property.GetCustomAttributes().Any(x => PreventNullAttributes.Contains(x.GetType().FullName ?? string.Empty));
        if (preventAttributeExists)
        {
            return true;
        }

        var getMethod = property.GetGetMethod();
        if (getMethod == null)
        {
            return false;
        }

        var returnParameter = getMethod.ReturnParameter;
        return returnParameter.GetCustomAttributes().Any(x => PreventNullAttributes.Contains(x.GetType().FullName ?? string.Empty));
    }
}