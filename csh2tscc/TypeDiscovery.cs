namespace csh2tscc;

internal class TypeDiscovery(TypesGeneratorParameters parameters)
{
    internal List<Type> GetTypes()
    {
        var types = new List<Type>();
        var filePaths = parameters.LibraryFileNames.Select(x => Path.GetDirectoryName(Path.GetFullPath(x))).ToList();
        foreach (var param in parameters.LibraryFileNames)
        {
            var exactPath = Path.GetFullPath(param);
            var context = new CustomAssemblyLoadContext(filePaths);
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

        var props = type.GetProperties();
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
        if (type.Namespace == null)
        {
            return false;
        }

        if (!IncludedType(type.Namespace) && !TypeHasExportAttribute(type))
        {
            return false;
        }

        if (type.GUID == Guid.Empty)
        {
            return false;
        }

        return !TypeAlreadyExists(type, existingTypes);
    }

    private int RemoveTypes(List<Type> types) => types.RemoveAll(ShouldFilterType);

    private bool ShouldFilterType(Type type) =>
        HasCustomMapping(type) ||
        IsInExcludedNamespace(type) ||
        IsCollectionType(type);

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

    private bool IsInExcludedNamespace(Type type) =>
        !string.IsNullOrWhiteSpace(type.FullName) &&
        parameters.RootNamespacesExcluded.Any(excluded => type.FullName.Contains(excluded));

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

    private bool IncludedType(string? needle) => needle != null && parameters.RootNamespaces.Any(needle.StartsWith);

    private bool ExcludedType(string? needle) => needle != null &&
                                                 parameters.RootNamespacesExcluded.Count != 0 &&
                                                 parameters.RootNamespacesExcluded.Any(needle.StartsWith);
}
