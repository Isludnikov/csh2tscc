namespace csh2tscc;

internal static class GenericHelper
{
    /// <summary>
    /// Gets the number of generic parameters declared locally on a type,
    /// excluding any inherited from a declaring (outer) type for nested generics.
    /// </summary>
    /// <remarks>
    /// For nested types like OuterClass&lt;T&gt;.InnerClass&lt;U&gt;:
    /// - InnerClass has 2 total generic arguments (T, U)
    /// - But only 1 is declared locally (U)
    /// - This method returns 1 for InnerClass
    /// </remarks>
    internal static int LocalGenericParameterCount(Type type)
    {
        if (!type.IsGenericType)
        {
            return 0;
        }

        var totalCount = type.GetGenericArguments().Length;

        if (!type.IsNested || type.DeclaringType is not { IsGenericType: true } declaringType)
        {
            return totalCount;
        }

        var parentCount = declaringType.GetGenericArguments().Length;
        return totalCount - parentCount;
    }
}