using csh2tscc;

namespace tests;

/// <summary>
/// Locks the public contract of the exception hierarchy. These constructors are part of the
/// library's surface but are not all reached through the generation pipeline, so they are
/// covered directly here.
/// </summary>
public class ExceptionsTests
{
    [Fact]
    public void TypeConversionException_WithInnerException_PreservesMessageAndInner()
    {
        var inner = new InvalidOperationException("boom");

        var ex = new TypeConversionException("wrapper", inner);

        Assert.Equal("wrapper", ex.Message);
        Assert.Same(inner, ex.InnerException);
    }

    [Fact]
    public void AttributeProcessingException_ExposesContextProperties()
    {
        var ex = new AttributeProcessingException(
            "bad attribute", attributeName: "JsonPropertyName", propertyName: "Name", parentType: typeof(string));

        Assert.Equal("bad attribute", ex.Message);
        Assert.Equal("JsonPropertyName", ex.AttributeName);
        Assert.Equal("Name", ex.PropertyName);
        Assert.Equal(typeof(string), ex.ParentType);
        Assert.IsAssignableFrom<TypeConversionException>(ex);
    }

    [Fact]
    public void UnsupportedTypeException_MessageAndTypeConstructor_KeepsBoth()
    {
        var ex = new UnsupportedTypeException("custom message", typeof(int));

        Assert.Equal("custom message", ex.Message);
        Assert.Equal(typeof(int), ex.UnsupportedType);
    }

    [Fact]
    public void UnsupportedTypeException_TypeOnlyConstructor_BuildsMessageFromFullName()
    {
        var ex = new UnsupportedTypeException(typeof(int));

        Assert.Contains(typeof(int).FullName!, ex.Message);
        Assert.Equal(typeof(int), ex.UnsupportedType);
    }

    [Fact]
    public void InvalidSerializationConfigException_IsTypeConversionException()
    {
        var ex = new InvalidSerializationConfigException("invalid");

        Assert.Equal("invalid", ex.Message);
        Assert.IsAssignableFrom<TypeConversionException>(ex);
    }
}
