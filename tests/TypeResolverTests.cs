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
        var parameters = config ?? ParametersBuilder.ForLocalDto().Build();
        var resolver = new TypeResolver(parameters, new TypeDiscovery(parameters));
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
    [InlineData(typeof(DateTimeOffset))]
    [InlineData(typeof(DateOnly))]
    [InlineData(typeof(TimeOnly))]
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
    public void NonGenericEnumerableSubclass_ResolvesToElementArray() =>
        Assert.Equal("string[]", Resolve(typeof(CustomStringList), container: BooleanContainer.CreateFalse()));

    [Fact]
    public void Dictionary_ResolvesToRecord() =>
        Assert.Equal("Record<string, number>", Resolve(typeof(Dictionary<string, int>), container: BooleanContainer.CreateFalse()));

    [Fact]
    public void ReadOnlyDictionary_ResolvesToRecord() =>
        Assert.Equal("Record<string, number>", Resolve(typeof(IReadOnlyDictionary<string, int>), container: BooleanContainer.CreateFalse()));

    [Fact]
    public void EnumKeyedDictionary_ResolvesToRecord() =>
        Assert.Equal("Record<SimpleEnum, number>", Resolve(typeof(Dictionary<SimpleEnum, int>), container: BooleanContainer.CreateFalse()));

    [Fact]
    public void DictionaryWithUnusableRecordKey_FallsBackToMap() =>
        // Record constrains its key to string | number | symbol, and "unknown" is none of those.
        Assert.Equal(
            "Map<unknown, number>",
            Resolve(typeof(Dictionary<object, int>), container: BooleanContainer.CreateFalse()));

    [Fact]
    public void EnumOutsideSelection_Throws()
    {
        // System.DayOfWeek is an enum no selection covers: naming it would leave a reference to
        // a file that is never generated.
        Assert.Throws<UnsupportedTypeException>(() => Resolve(typeof(DayOfWeek)));
    }

    [Fact]
    public void EnumOutsideSelection_ResolvesToString_WhenUnknownToStringEnabled()
    {
        var config = ParametersBuilder.ForLocalDto().WithUnknownTypesToString().Build();
        Assert.Equal("string", Resolve(typeof(DayOfWeek), config));
    }

    [Fact]
    public void EnumOutsideSelection_IsStillMappable()
    {
        var config = ParametersBuilder.ForLocalDto().WithCustomMap(("DayOfWeek", "number")).Build();
        Assert.Equal("number", Resolve(typeof(DayOfWeek), config));
    }

    [Fact]
    public void GenericTypeOutsideSelection_Throws()
    {
        Assert.Throws<UnsupportedTypeException>(() =>
            Resolve(typeof(KeyValuePair<string, string>), container: BooleanContainer.CreateFalse()));
    }

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
