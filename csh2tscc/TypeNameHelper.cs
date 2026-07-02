namespace csh2tscc;

internal static class TypeNameHelper
{
    internal static string NormalizeClassName(string className) => className.Contains(TypeScriptConstants.GenericAritySeparator)
        ? className[..className.LastIndexOf(TypeScriptConstants.GenericAritySeparator)]
        : className;

    internal static string GetTypeScriptName(Type type, bool useFullNames) =>
        (useFullNames ? type.FullName ?? type.Name : type.Name).Replace('.', '_');

    /// <summary>
    /// TypeScript identifier of a type as it appears in generated code and file names:
    /// the (optionally full) name with dots replaced and the generic arity suffix stripped.
    /// </summary>
    internal static string GetNormalizedTypeScriptName(Type type, bool useFullNames) =>
        NormalizeClassName(GetTypeScriptName(type, useFullNames));

    /// <summary>
    /// Lower-cases the first character when <paramref name="camelCase"/> is enabled.
    /// Safe for empty strings. Pure function — extracted so it can be unit-tested directly.
    /// </summary>
    internal static string ToCamelCase(string name, bool camelCase) =>
        camelCase && name.Length > 0 ? char.ToLowerInvariant(name[0]) + name[1..] : name;
}
