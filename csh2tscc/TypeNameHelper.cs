using System.Text.RegularExpressions;

namespace csh2tscc;

internal static partial class TypeNameHelper
{
    /// <summary>
    /// The arity suffix of a generic type name (`1, and ``1 for a generic method). A nested generic
    /// carries one per level ("Outer`1+Inner`1"), so every occurrence is removed, not just the last.
    /// </summary>
    [GeneratedRegex("`+\\d+")]
    private static partial Regex ArityPattern();

    internal static string NormalizeClassName(string className) => ArityPattern().Replace(className, string.Empty);

    /// <summary>
    /// The (optionally full) CLR name of a type with the namespace and nesting separators replaced
    /// by underscores. A constructed generic is named after its definition: its own FullName spells
    /// out every type argument with assembly qualification, which is not a name for anything.
    /// </summary>
    internal static string GetTypeScriptName(Type type, bool useFullNames)
    {
        var subject = type is { IsGenericType: true, IsGenericTypeDefinition: false }
            ? type.GetGenericTypeDefinition()
            : type;

        var name = useFullNames ? subject.FullName ?? subject.Name : subject.Name;
        return name.Replace('.', '_').Replace('+', '_');
    }

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
