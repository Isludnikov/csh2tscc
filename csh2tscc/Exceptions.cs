namespace csh2tscc;

/// <summary>
/// Base exception for all type conversion errors.
/// </summary>
public class TypeConversionException : Exception
{
    public TypeConversionException(string message) : base(message) { }
    public TypeConversionException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when attribute processing fails.
/// </summary>
public class AttributeProcessingException : TypeConversionException
{
    public string? AttributeName { get; }
    public string? PropertyName { get; }
    public Type? ParentType { get; }

    public AttributeProcessingException(string message, string? attributeName = null, string? propertyName = null, Type? parentType = null)
        : base(message)
    {
        AttributeName = attributeName;
        PropertyName = propertyName;
        ParentType = parentType;
    }
}

/// <summary>
/// Exception thrown when a type cannot be resolved to TypeScript.
/// </summary>
public class UnsupportedTypeException : TypeConversionException
{
    public Type? UnsupportedType { get; }

    public UnsupportedTypeException(string message, Type? unsupportedType = null)
        : base(message)
    {
        UnsupportedType = unsupportedType;
    }

    public UnsupportedTypeException(Type unsupportedType)
        : base($"Unsupported type: {unsupportedType.FullName ?? unsupportedType.Name}")
    {
        UnsupportedType = unsupportedType;
    }
}

/// <summary>
/// Exception thrown when serialization configuration is invalid.
/// </summary>
public class InvalidSerializationConfigException : TypeConversionException
{
    public InvalidSerializationConfigException(string message) : base(message) { }
}
