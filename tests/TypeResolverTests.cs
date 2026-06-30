using csh2tscc;
using tests.DTO;
using tests.TestSupport;

namespace tests;

public class TypeResolverTests
{
    private static string Resolve(
        Type type,
        TypesGeneratorParameters? config = null,
        bool isNullable = false,
        bool suppressNullable = false,
        BooleanContainer? container = null)
    {
        var resolver = new TypeResolver(config ?? ParametersBuilder.ForLocalDto().Build());
        var context = new PropertyTypeExtractionContext
        {
            ClassToWrite = typeof(object),
            PropInfo = null,
            PropertyType = type,
            IsNullable = isNullable,
            SuppressNullable = suppressNullable,
            BooleanContainer = container
        };
        return resolver.ResolveTypeToTypeScript(context);
    }

    [Theory]
    [InlineData(typeof(int))]
    [InlineData(typeof(uint))]
    [InlineData(typeof(short))]
    [InlineData(typeof(ushort))]
    [InlineData(typeof(byte))]
    [InlineData(typeof(sbyte))]
    [InlineData(typeof(long))]
    [InlineData(typeof(ulong))]
    [InlineData(typeof(float))]
    [InlineData(typeof(double))]
    [InlineData(typeof(decimal))]
    public void NumberTypes_ResolveToNumber(Type type)
    {
        Assert.Equal("number", Resolve(type));
    }

    [Fact]
    public void Bool_ResolvesToBoolean() => Assert.Equal("boolean", Resolve(typeof(bool)));

    [Fact]
    public void String_ResolvesToString() => Assert.Equal("string", Resolve(typeof(string)));

    [Theory]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(Uri))]
    [InlineData(typeof(TimeSpan))]
    public void ToStringTypes_ResolveToString(Type type)
    {
        Assert.Equal("string", Resolve(type));
    }

    [Fact]
    public void Object_ResolvesToUnknown() => Assert.Equal("unknown", Resolve(typeof(object)));

    [Fact]
    public void Enum_ResolvesToEnumName() => Assert.Equal("SimpleEnum", Resolve(typeof(SimpleEnum)));

    [Fact]
    public void IsNullable_AppendsNullUnion() => Assert.Equal("number | null", Resolve(typeof(int), isNullable: true));

    [Fact]
    public void SuppressNullable_OverridesIsNullable() =>
        Assert.Equal("number", Resolve(typeof(int), isNullable: true, suppressNullable: true));

    [Fact]
    public void CustomMap_ByShortName_Wins()
    {
        var config = ParametersBuilder.ForLocalDto().WithCustomMap(("SimpleObject", "MyShort")).Build();
        Assert.Equal("MyShort", Resolve(typeof(SimpleObject), config));
    }

    [Fact]
    public void CustomMap_ByFullName_Wins()
    {
        // Short name is not mapped — resolution must fall through to the full-name lookup.
        var config = ParametersBuilder.ForLocalDto()
            .WithCustomMap(("tests.DTO.SimpleObject", "MyFull"))
            .Build();
        Assert.Equal("MyFull", Resolve(typeof(SimpleObject), config));
    }

    [Fact]
    public void Array_ResolvesWithSuffix() =>
        Assert.Equal("number[]", Resolve(typeof(int[]), container: BooleanContainer.CreateFalse()));

    [Fact]
    public void Enumerable_ResolvesToElementArray() =>
        Assert.Equal("string[]", Resolve(typeof(List<string>), container: BooleanContainer.CreateFalse()));

    [Fact]
    public void Dictionary_ResolvesToMap() =>
        Assert.Equal("Map<string, number>", Resolve(typeof(Dictionary<string, int>), container: BooleanContainer.CreateFalse()));

    [Fact]
    public void UnsupportedType_Throws_WhenUnknownToStringDisabled()
    {
        Assert.Throws<UnsupportedTypeException>(() =>
            Resolve(typeof(Stream), container: BooleanContainer.CreateFalse()));
    }

    [Fact]
    public void UnsupportedType_ResolvesToString_WhenUnknownToStringEnabled()
    {
        var config = ParametersBuilder.ForLocalDto().WithUnknownTypesToString().Build();
        Assert.Equal("string", Resolve(typeof(Stream), config, container: BooleanContainer.CreateFalse()));
    }
}
