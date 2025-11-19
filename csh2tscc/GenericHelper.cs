using System.Globalization;

namespace csh2tscc;

internal class GenericHelper
{
    internal static int LocalGenericParameterCount(Type t) => t.IsNested ? GenericParameterCount(t) - GenericParameterCount(t.DeclaringType) : GenericParameterCount(t);

    private static int GenericParameterCount(Type type)
    {
        if (!type.IsGenericType)
            return 0;

        var genericTypeDefName = type.GetGenericTypeDefinition().Name;
        var tickIndex = genericTypeDefName.LastIndexOf('`');
        return tickIndex == -1 ? 0 : int.Parse(genericTypeDefName[(tickIndex + 1)..], NumberStyles.None);
    }
}