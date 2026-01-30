namespace csh2tscc;

internal static class TypeNameHelper
{
    internal static string NormalizeClassName(string className) => className.Contains(TypeScriptConstants.GenericAritySeparator)
        ? className[..className.LastIndexOf(TypeScriptConstants.GenericAritySeparator)]
        : className;

    internal static string GetTypeScriptName(Type type, bool useFullNames) =>
        (useFullNames ? type.FullName ?? type.Name : type.Name).Replace('.', '_');
}
