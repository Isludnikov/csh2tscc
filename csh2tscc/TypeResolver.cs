namespace csh2tscc;

internal class TypeResolver(TypesGeneratorParameters parameters)
{
    private static readonly Type[] NumberTypes = [
        typeof(int), typeof(uint), typeof(short), typeof(byte), typeof(sbyte), typeof(long), typeof(ulong),
        typeof(float), typeof(double), typeof(ushort), typeof(decimal)
    ];

    private static readonly Type[] ToStringTypes = [
        typeof(Guid), typeof(DateTime), typeof(Uri), typeof(TimeSpan)
    ];

    internal string ResolveTypeToTypeScript(PropertyTypeExtractionContext context)
    {
        var nullable = ResolveNullability(context);
        var propertyType = UnwrapNullableType(context.PropertyType);

        // Try simple type resolution (custom, primitive, enum, object)
        if (TryResolveSimpleType(propertyType, nullable) is { } simpleResult)
        {
            return simpleResult;
        }

        // Complex types require nullable tracking for generic arguments
        return ResolveComplexType(context, propertyType, nullable);
    }

    private string? TryResolveCustomMappedType(Type propertyType)
    {
        if (parameters.CustomMap.TryGetValue(propertyType.Name, out var value))
        {
            return value;
        }

        if (!string.IsNullOrWhiteSpace(propertyType.FullName) &&
            parameters.CustomMap.TryGetValue(propertyType.FullName, out var fullNameValue))
        {
            return fullNameValue;
        }

        return null;
    }

    private string? TryResolvePrimitiveType(Type propertyType)
    {
        if (NumberTypes.Contains(propertyType))
        {
            return TypeScriptConstants.NumberType;
        }

        if (propertyType == typeof(bool))
        {
            return TypeScriptConstants.BooleanType;
        }

        if (propertyType == typeof(string))
        {
            return TypeScriptConstants.StringType;
        }

        if (ToStringTypes.Contains(propertyType))
        {
            return TypeScriptConstants.StringType;
        }

        return null;
    }

    private string? TryResolveSimpleType(Type propertyType, bool nullable)
    {
        // Custom mapped types
        if (TryResolveCustomMappedType(propertyType) is { } customType)
        {
            return CommonHelper.GetPropertyTypeWithNullable(customType, nullable);
        }

        // Primitive types (number, boolean, string)
        if (TryResolvePrimitiveType(propertyType) is { } primitiveType)
        {
            return CommonHelper.GetPropertyTypeWithNullable(primitiveType, nullable);
        }

        // Enum types
        if (propertyType.IsEnum)
        {
            return CommonHelper.GetPropertyTypeWithNullable(TypeNameHelper.GetTypeScriptName(propertyType, parameters.UseFullNames), nullable);
        }

        // Object type
        if (propertyType == typeof(object))
        {
            return TypeScriptConstants.UnknownType;
        }

        return null;
    }

    private string? TryResolveArrayType(PropertyTypeExtractionContext context, Type propertyType, BooleanContainer nullableList, bool nullable)
    {
        if (!propertyType.IsArray)
        {
            return null;
        }

        var elementContext = context.CreateDerived(propertyType.GetElementType()!, nullableList, nullable);
        return ResolveTypeToTypeScript(elementContext) + TypeScriptConstants.ArraySuffix;
    }

    private string? TryResolveDictionaryType(PropertyTypeExtractionContext context, Type propertyType, BooleanContainer nullableList)
    {
        if (!propertyType.InstanceOfGenericInterface(typeof(IDictionary<,>)))
        {
            return null;
        }

        var genericArguments = propertyType.GetInterfaces()
            .SingleOrDefault(x => x.InstanceOfGenericInterface(typeof(IDictionary<,>)))
            ?.GetGenericArguments() ?? propertyType.GetGenericArguments();

        var keyContext = context.CreateDerived(
            genericArguments[0],
            nullableList,
            GetNullabilityForGenericArg(genericArguments[0], nullableList));

        var valueContext = context.CreateDerived(
            genericArguments[1],
            nullableList,
            GetNullabilityForGenericArg(genericArguments[1], nullableList));

        return $"{TypeScriptConstants.MapType}{TypeScriptConstants.GenericOpen}{ResolveTypeToTypeScript(keyContext)}{TypeScriptConstants.GenericSeparator}{ResolveTypeToTypeScript(valueContext)}{TypeScriptConstants.GenericClose}";
    }

    private string? TryResolveEnumerableType(PropertyTypeExtractionContext context, Type propertyType, BooleanContainer nullableList)
    {
        if (!propertyType.InstanceOfGenericInterface(typeof(IEnumerable<>)))
        {
            return null;
        }

        var genericArguments = propertyType.GetGenericArguments();
        var elementContext = context.CreateDerived(
            genericArguments[0],
            nullableList,
            GetNullabilityForGenericArg(genericArguments[0], nullableList));

        return $"{ResolveTypeToTypeScript(elementContext)}{TypeScriptConstants.ArraySuffix}";
    }

    private string? TryResolveGenericType(PropertyTypeExtractionContext context, Type propertyType, BooleanContainer nullableList)
    {
        if (!propertyType.IsGenericType)
        {
            return null;
        }

        var localGenericArguments = propertyType.GetGenericArguments()
            .Take(GenericHelper.LocalGenericParameterCount(propertyType))
            .ToArray();

        var typeArgs = localGenericArguments
            .Select(arg => ResolveTypeToTypeScript(
                context.CreateDerived(arg, nullableList, GetNullabilityForGenericArg(arg, nullableList))))
            .Aggregate((a, b) => a + TypeScriptConstants.GenericSeparator + b);

        return $"{TypeNameHelper.NormalizeClassName(TypeNameHelper.GetTypeScriptName(propertyType, parameters.UseFullNames))}{TypeScriptConstants.GenericOpen}{typeArgs}{TypeScriptConstants.GenericClose}";
    }

    private string ResolveComplexType(PropertyTypeExtractionContext context, Type propertyType, bool nullable)
    {
        var nullableList = GetNullableContainer(context);

        // Array types
        if (TryResolveArrayType(context, propertyType, nullableList, nullable) is { } arrayType)
        {
            return CommonHelper.GetPropertyTypeWithNullable(arrayType, nullable);
        }

        // Dictionary types (must check before IEnumerable since dictionaries implement IEnumerable)
        if (TryResolveDictionaryType(context, propertyType, nullableList) is { } dictType)
        {
            return CommonHelper.GetPropertyTypeWithNullable(dictType, nullable);
        }

        // Enumerable types
        if (TryResolveEnumerableType(context, propertyType, nullableList) is { } enumerableType)
        {
            return CommonHelper.GetPropertyTypeWithNullable(enumerableType, nullable);
        }

        // Generic types
        if (TryResolveGenericType(context, propertyType, nullableList) is { } genericType)
        {
            return CommonHelper.GetPropertyTypeWithNullable(genericType, nullable);
        }

        // Affected types (types that need imports)
        if (IsAffectedOrGenericType(context, propertyType))
        {
            return CommonHelper.GetPropertyTypeWithNullable(TypeNameHelper.NormalizeClassName(TypeNameHelper.GetTypeScriptName(propertyType, parameters.UseFullNames)), nullable);
        }

        return parameters.UnknownTypesToString ?
            TypeScriptConstants.StringType :
            throw new UnsupportedTypeException(propertyType);
    }

    private static bool ResolveNullability(PropertyTypeExtractionContext context)
    {
        if (context.SuppressNullable)
        {
            return false;
        }

        return context.PropInfo != null
            ? IsNullableHelper.IsNullable(context.ClassToWrite, context.PropInfo)
            : context.IsNullable;
    }

    private static Type UnwrapNullableType(Type type) =>
        Nullable.GetUnderlyingType(type) ?? type;

    private static bool GetNullabilityForGenericArg(Type argType, BooleanContainer nullableList)
    {
        return argType.IsValueType
            ? IsNullableHelper.IsValueTypeNullable(argType)
            : nullableList.GetValueAndMoveNext();
    }

    private static BooleanContainer GetNullableContainer(PropertyTypeExtractionContext context) =>
        (context.PropInfo == null
            ? context.BooleanContainer
            : IsNullableHelper.IsNullableContainer(context.ClassToWrite, context.PropInfo))
        ?? throw new InvalidOperationException(
            $"BooleanContainer should not be null for complex property type. Property: {context.PropInfo?.Name}, Type: {context.PropertyType}");

    private static bool IsAffectedOrGenericType(PropertyTypeExtractionContext context, Type propertyType) =>
        context.AffectedTypes.Any(x => x.FullName == propertyType.FullName) ||
        context.GenericTypes.Any(x => x.Name == propertyType.Name);
}
