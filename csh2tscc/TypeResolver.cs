namespace csh2tscc;

internal class TypeResolver(TypesGeneratorParameters parameters, TypeDiscovery discovery)
{
    private static readonly Type[] NumberTypes = [
        typeof(int), typeof(uint), typeof(short), typeof(byte), typeof(sbyte), typeof(long), typeof(ulong),
        typeof(float), typeof(double), typeof(ushort), typeof(decimal)
    ];

    // Types that serialize as a JSON string. DateTimeOffset, DateOnly and TimeOnly belong here for
    // the same reason DateTime does: System.Text.Json writes all of them as ISO-8601 text.
    private static readonly Type[] ToStringTypes = [
        typeof(Guid), typeof(DateTime), typeof(DateTimeOffset), typeof(DateOnly), typeof(TimeOnly),
        typeof(Uri), typeof(TimeSpan)
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
            // An enum outside the generated set has no file to import from, and enums are named
            // without an import at all — so emitting the name would leave a dangling reference
            // that surfaces far from its cause, or not at all when the name exists globally
            // (System.DayOfWeek is the textbook case). Refusing here names the actual type.
            // The escape hatch is CustomMap, checked above this.
            if (!discovery.IsGeneratedType(propertyType))
            {
                return parameters.UnknownTypesToString
                    ? CommonHelper.GetPropertyTypeWithNullable(TypeScriptConstants.StringType, nullable)
                    : throw new UnsupportedTypeException(propertyType);
            }

            return CommonHelper.GetPropertyTypeWithNullable(TypeNameHelper.GetTypeScriptName(propertyType, parameters.UseFullNames), nullable);
        }

        // Object type
        if (propertyType == typeof(object))
        {
            return TypeScriptConstants.UnknownType;
        }

        return null;
    }

    private string? TryResolveArrayType(PropertyTypeExtractionContext context, Type propertyType, BooleanContainer nullableList)
    {
        if (!propertyType.IsArray)
        {
            return null;
        }

        // The element is the next node of the nullable flags after the array itself, exactly like a
        // generic argument: string?[] is [1, 2] and string[]? is [2, 1].
        var elementType = propertyType.GetElementType()!;
        var elementContext = context.CreateDerived(elementType, nullableList, GetNullabilityForGenericArg(elementType, nullableList));
        return AsArray(ResolveTypeToTypeScript(elementContext));
    }

    /// <summary>
    /// Appends the array suffix. A union element must be parenthesised: <c>string | null[]</c>
    /// reads as <c>string | (null[])</c> in TypeScript.
    /// </summary>
    private static string AsArray(string elementType) =>
        (HasTopLevelUnion(elementType) ? $"({elementType})" : elementType) + TypeScriptConstants.ArraySuffix;

    /// <summary>
    /// Whether a type expression is a union at its top level: a bar inside brackets or parentheses
    /// (<c>Record&lt;string, string | null&gt;</c>, <c>(string | null)[]</c>) binds tighter than
    /// the array suffix already and needs no wrapping.
    /// </summary>
    private static bool HasTopLevelUnion(string typeExpression)
    {
        var depth = 0;
        foreach (var c in typeExpression)
        {
            switch (c)
            {
                case '(' or '<' or '[':
                    ++depth;
                    break;
                case ')' or '>' or ']':
                    --depth;
                    break;
                case '|' when depth == 0:
                    return true;
            }
        }

        return false;
    }

    private static readonly Type[] DictionaryInterfaces = [typeof(IDictionary<,>), typeof(IReadOnlyDictionary<,>)];

    private string? TryResolveDictionaryType(PropertyTypeExtractionContext context, Type propertyType, BooleanContainer nullableList)
    {
        // The type is a dictionary if it is (or implements) IDictionary<,> or IReadOnlyDictionary<,>;
        // the key/value types come from that interface (the type itself may be a non-generic subclass).
        var dictionaryInterface = DictionaryInterfaces
            .Select(di => propertyType.InstanceOfGenericType(di)
                ? propertyType
                : propertyType.GetInterfaces().FirstOrDefault(x => x.InstanceOfGenericType(di)))
            .FirstOrDefault(x => x != null);

        if (dictionaryInterface == null)
        {
            return null;
        }

        var genericArguments = dictionaryInterface.GetGenericArguments();

        // The nullable flags are laid out depth-first: the key's own flag is followed by the flags
        // of the key's type arguments, and only then comes the value. Each argument must therefore
        // be fully resolved before the next one's flag is read.
        var key = ResolveGenericArgument(context, genericArguments[0], nullableList);
        var value = ResolveGenericArgument(context, genericArguments[1], nullableList);

        // A dictionary serializes to a JSON object, which is Record<K, V> on the TypeScript side;
        // Map is a different runtime thing and never arrives over the wire. Record constrains its
        // key to string | number | symbol, so anything else falls back to Map rather than to code
        // that does not compile.
        var container = IsValidRecordKey(genericArguments[0], key)
            ? TypeScriptConstants.RecordType
            : TypeScriptConstants.MapType;

        return $"{container}{TypeScriptConstants.GenericOpen}{key}{TypeScriptConstants.GenericSeparator}{value}{TypeScriptConstants.GenericClose}";
    }

    // A key that admits null ("string | null") is not a valid Record key either.
    private static bool IsValidRecordKey(Type keyType, string resolvedKey) =>
        !resolvedKey.Contains('|') &&
        (UnwrapNullableType(keyType).IsEnum ||
         resolvedKey == TypeScriptConstants.StringType ||
         resolvedKey == TypeScriptConstants.NumberType);

    private string? TryResolveEnumerableType(PropertyTypeExtractionContext context, Type propertyType, BooleanContainer nullableList)
    {
        if (!propertyType.InstanceOfGenericInterface(typeof(IEnumerable<>)))
        {
            return null;
        }

        // Non-generic subclasses (class CustomList : List<string>) carry no generic arguments
        // of their own — take the element type from the implemented IEnumerable<T> instead.
        var genericArguments = propertyType.IsGenericType
            ? propertyType.GetGenericArguments()
            : propertyType.GetInterfaces().First(i => i.InstanceOfGenericType(typeof(IEnumerable<>))).GetGenericArguments();

        return AsArray(ResolveGenericArgument(context, genericArguments[0], nullableList));
    }

    /// <summary>
    /// Reads the nullable flag of one type argument and resolves it before returning, so that the
    /// flags of nested arguments are consumed in the depth-first order the compiler wrote them.
    /// </summary>
    private string ResolveGenericArgument(PropertyTypeExtractionContext context, Type argument, BooleanContainer nullableList) =>
        ResolveTypeToTypeScript(context.CreateDerived(argument, nullableList, GetNullabilityForGenericArg(argument, nullableList)));

    private string? TryResolveGenericType(PropertyTypeExtractionContext context, Type propertyType, BooleanContainer nullableList)
    {
        if (!propertyType.IsGenericType)
        {
            return null;
        }

        // Same dangling-reference trap as with enums, one level up: a generic type outside the
        // generated set (KeyValuePair<K, V> is the one that actually shows up) would be printed
        // by name with nothing behind it. Declining here lets the affected-types check below have
        // its say, and an unresolved type ends as UnsupportedTypeException rather than as output.
        if (!discovery.IsGeneratedType(propertyType))
        {
            return null;
        }

        // For a nested generic type, GetGenericArguments() lists the declaring (outer) type's
        // parameters first, then the type's own. The generated interface declares all of them
        // (TypeScriptBuilder emits the same list as its header, and the members do use the outer
        // ones), so a reference has to pass all of them as well or the arity will not match.
        // string.Join enumerates lazily in order, which keeps the nullable flags depth-first.
        var typeArgs = string.Join(
            TypeScriptConstants.GenericSeparator,
            propertyType.GetGenericArguments().Select(arg => ResolveGenericArgument(context, arg, nullableList)));

        return $"{TypeNameHelper.GetNormalizedTypeScriptName(propertyType, parameters.UseFullNames)}{TypeScriptConstants.GenericOpen}{typeArgs}{TypeScriptConstants.GenericClose}";
    }

    private string ResolveComplexType(PropertyTypeExtractionContext context, Type propertyType, bool nullable)
    {
        var nullableList = GetNullableContainer(context);

        // Array types
        if (TryResolveArrayType(context, propertyType, nullableList) is { } arrayType)
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
            return CommonHelper.GetPropertyTypeWithNullable(TypeNameHelper.GetNormalizedTypeScriptName(propertyType, parameters.UseFullNames), nullable);
        }

        return parameters.UnknownTypesToString ?
            CommonHelper.GetPropertyTypeWithNullable(TypeScriptConstants.StringType, nullable) :
            throw new UnsupportedTypeException(propertyType);
    }

    // TypeScriptBuilder computes IsNullable for the root property (via IsNullableHelper) and
    // CreateDerived carries it for nested type arguments, so the context value is authoritative.
    private static bool ResolveNullability(PropertyTypeExtractionContext context) =>
        !context.SuppressNullable && context.IsNullable;

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

    // A generic closed over a type parameter (SimpleGenericType<T>) has a null FullName; comparing
    // nulls would make any such type "affected" by any other, so those are matched by identity.
    private static bool IsAffectedOrGenericType(PropertyTypeExtractionContext context, Type propertyType) =>
        context.AffectedTypes.Any(x => x == propertyType || (x.FullName != null && x.FullName == propertyType.FullName)) ||
        context.GenericTypes.Any(x => x.Name == propertyType.Name);
}
