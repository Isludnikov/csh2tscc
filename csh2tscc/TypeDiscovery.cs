namespace csh2tscc;

internal class TypeDiscovery(TypesGeneratorParameters parameters)
{
    internal List<Type> GetTypes()
    {
        var types = new List<Type>();
        var filePaths = parameters.LibraryFileNames.Select(x => Path.GetDirectoryName(Path.GetFullPath(x))).ToList();
        var context = new CustomAssemblyLoadContext(filePaths);
        foreach (var param in parameters.LibraryFileNames)
        {
            var exactPath = Path.GetFullPath(param);
            var assembly = context.LoadAssembly(exactPath);
            types.AddRange(assembly.GetExportedTypes().Where(IsExportableType));
        }

        return types;
    }

    internal List<Type> ListAffectedTypes(Type type)
    {
        var affected = new List<Type>();
        if (type.IsGenericType)
        {
            AddType(affected, type.GenericTypeArguments);
        }

        var props = CommonHelper.GetSerializableProperties(type);
        foreach (var prop in props)
        {
            if (prop.PropertyType.IsGenericType)
            {
                AddType(affected, prop.PropertyType.GenericTypeArguments);
            }

            AddType(affected, [prop.PropertyType]);
        }

        RemoveTypes(affected);
        return affected;
    }

    private void AddType(List<Type> list, Type[] types)
    {
        foreach (var type in types)
        {
            if (HasCustomMapping(type))
            {
                continue;
            }

            if (type.IsGenericType)
            {
                AddType(list, type.GenericTypeArguments);
            }

            if (type.IsArray)
            {
                AddType(list, [type.GetElementType()!]);
            }
            else if (ShouldIncludeType(type, list))
            {
                list.Add(type);
            }
        }
    }

    private bool ShouldIncludeType(Type type, List<Type> existingTypes)
    {
        // Generic parameters (the T in TreeNode<T>) report the declaring type's namespace
        // but are placeholders, not importable types.
        if (type.IsGenericParameter || type.Namespace == null)
        {
            return false;
        }

        // The full name is tried as well as the namespace so that naming a single type in the
        // selection means the same here as it does in IsExportableType: a file is generated for it
        // AND properties of that type are imported. Matching the namespace alone would generate
        // the file and then refuse to reference it.
        if (!IncludedType(type.Namespace) && !IncludedType(type.FullName) && !TypeHasExportAttribute(type))
        {
            return false;
        }

        return !TypeAlreadyExists(type, existingTypes);
    }

    private int RemoveTypes(List<Type> types) => types.RemoveAll(ShouldFilterType);

    private bool ShouldFilterType(Type type) =>
        HasCustomMapping(type) ||
        ExcludedType(type.FullName) ||
        IsCollectionType(type);

    /// <summary>
    /// Whether a file will be generated for this type, i.e. whether referring to it by name in
    /// the output resolves to something. Asked by <see cref="TypeResolver"/> about types it is
    /// about to name; the selection rules are here because <see cref="GetTypes"/> obeys the same.
    /// </summary>
    internal bool IsGeneratedType(Type type) => IsExportableType(RepresentativeType(type));

    /// <summary>
    /// The type whose selection decides the question. A constructed generic closed over a generic
    /// parameter (SimpleGenericType&lt;T&gt; inside ComplexType&lt;T&gt;) reports a null FullName
    /// and would match no namespace at all; its definition is what a file is generated for.
    /// </summary>
    private static Type RepresentativeType(Type type) =>
        type is { IsGenericType: true, IsGenericTypeDefinition: false, FullName: null }
            ? type.GetGenericTypeDefinition()
            : type;

    private bool IsExportableType(Type type) =>
        (IncludedType(type.FullName) &&
        !IsCompilerGeneratedType(type) &&
        !ExcludedType(type.FullName))
        || TypeHasExportAttribute(type);

    private bool TypeHasExportAttribute(Type type)
    {
        if (parameters.ExportAttributes.Count == 0)
        {
            return false;
        }

        var attributes = type.GetCustomAttributes(true);
        if (attributes.Length == 0)
        {
            return false;
        }

        var extractedAttributes = attributes.Where(x => parameters.ExportAttributes.Contains(x.GetType().Name)).ToArray();
        return extractedAttributes.Length != 0;
    }

    private static bool IsCompilerGeneratedType(Type type) =>
        type.Name.StartsWith(TypeScriptConstants.CompilerGeneratedTypeIndicator);

    private static bool IsCollectionType(Type type)
    {
        var interfaces = type.GetInterfaces();
        return interfaces.Any(i => i.InstanceOfGenericType(typeof(IDictionary<,>))) ||
               interfaces.Any(i => i.InstanceOfGenericType(typeof(IEnumerable<>)));
    }

    private static bool TypeAlreadyExists(Type type, List<Type> existingTypes) =>
        existingTypes.Any(existing => TypeNamesMatch(existing, type));

    private static bool TypeNamesMatch(Type a, Type b) =>
        TypeNameHelper.NormalizeClassName(a.FullName ?? a.Name) == TypeNameHelper.NormalizeClassName(b.FullName ?? b.Name);

    private bool HasCustomMapping(Type type) =>
        parameters.CustomMap.ContainsKey(type.Name) ||
        (!string.IsNullOrWhiteSpace(type.FullName) && parameters.CustomMap.ContainsKey(type.FullName));

    private bool IncludedType(string? needle) => needle != null && parameters.RootNamespaces.Any(ns => IsWithinNamespace(needle, ns));

    private bool ExcludedType(string? needle) => needle != null &&
                                                 parameters.RootNamespacesExcluded.Any(ns => IsWithinNamespace(needle, ns));

    /// <summary>
    /// Prefix match with a dot boundary: "My.DTO" covers "My.DTO" and "My.DTO.Sub.Type",
    /// but not the neighboring namespace "My.DTOther".
    /// </summary>
    private static bool IsWithinNamespace(string needle, string namespacePrefix) =>
        needle == namespacePrefix || needle.StartsWith(namespacePrefix + '.');
}
