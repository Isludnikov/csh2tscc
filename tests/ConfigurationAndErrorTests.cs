using csh2tscc;
using Dto.Integration.Tests.DTO.Extensions;
using tests.DTO;
using tests.TestSupport;

namespace tests;

public class ConfigurationAndErrorTests
{
    // === Fix 1: FileExtension is respected ===

    [Theory]
    [InlineData(".ts")]
    [InlineData(".d.ts")]
    [InlineData(".tsx")]
    public void TransformTypes_UsesConfiguredFileExtension(string extension)
    {
        var files = ParametersBuilder.ForIntegrationDll()
            .WithUnknownTypesToString()
            .WithFileExtension(extension)
            .BuildGenerator()
            .TransformTypes();

        Assert.NotEmpty(files);
        Assert.All(files, kv => Assert.EndsWith(extension, kv.Key));
    }

    // === Fix 2: generated TypeScript does not end with a blank line ===

    [Fact]
    public void BuildFileFromType_OutputDoesNotEndWithDoubleNewline()
    {
        var ts = ParametersBuilder.ForLocalDto().BuildGenerator().BuildFileFromType(typeof(SimpleObject));
        Assert.False(ts.EndsWith(Environment.NewLine + Environment.NewLine),
            "Generated TypeScript should not end with a blank line.");
        Assert.EndsWith("}" + Environment.NewLine, ts);
    }

    // === CustomMap path is exercised ===

    [Fact]
    public void CustomMap_ByShortName_OverridesGeneratedType()
    {
        var ts = ParametersBuilder.ForLocalDto()
            .WithCustomMap(("SimpleObject", "MyTsType"))
            .BuildGenerator()
            .BuildFileFromType(typeof(CustomMappedDto));

        Assert.Contains("mapped: MyTsType", ts);
        Assert.DoesNotContain("mapped: SimpleObject", ts);
        Assert.DoesNotContain("import { SimpleObject }", ts);
    }

    // === UseFullNames path is exercised ===

    [Fact]
    public void UseFullNames_True_ProducesUnderscoreSeparatedNames()
    {
        var ts = ParametersBuilder.ForLocalDto()
            .WithUseFullNames()
            .BuildGenerator()
            .BuildFileFromType(typeof(SimpleObject));

        Assert.Contains("export interface tests_DTO_SimpleObject", ts);
    }

    // === Error path: UnsupportedTypeException ===

    [Fact]
    public void UnknownTypesToString_False_ThrowsForUnsupportedType()
    {
        Assert.Throws<UnsupportedTypeException>(() =>
            ParametersBuilder.ForLocalDto().BuildGenerator().BuildFileFromType(typeof(UnknownTypeDto)));
    }

    [Fact]
    public void UnknownTypesToString_True_MapsUnsupportedToString()
    {
        var ts = ParametersBuilder.ForLocalDto()
            .WithUnknownTypesToString()
            .BuildGenerator()
            .BuildFileFromType(typeof(UnknownTypeDto));

        Assert.Contains("unsupportedField: string", ts);
    }

    // === Error path: InvalidSerializationConfigException ===

    [Fact]
    public void DuplicateNamingAttributes_OnOneProperty_Throws()
    {
        var ex = Assert.Throws<InvalidSerializationConfigException>(() =>
            ParametersBuilder.ForLocalDto().BuildGenerator().BuildFileFromType(typeof(DuplicateNamingDto)));
        Assert.Contains("more than one serialization naming attribute", ex.Message);
    }

    // === Error path: AttributeProcessingException (naming attribute resolves to null) ===

    [Fact]
    public void NamingAttribute_WithNullName_ThrowsAttributeProcessingException()
    {
        var ex = Assert.Throws<AttributeProcessingException>(() =>
            ParametersBuilder.ForLocalDto().BuildGenerator().BuildFileFromType(typeof(NullNameDto)));

        Assert.Contains("is null", ex.Message);
        Assert.Equal(nameof(CustomNameAttribute), ex.AttributeName);
    }

    // === Fix 3: ExcludedNamespace uses StartsWith (not Contains) ===

    [Fact]
    public void ExcludedNamespace_IsPrefixMatch_NotSubstring()
    {
        // RootNamespacesExcluded = ["DTO"]. This is a *substring* of "tests.DTO.SimpleGenericType"
        // but not a prefix. Under the old Contains-based check, SimpleGenericType would have
        // been wrongly filtered out of ComplexType's affected types — and the import statement
        // for it would be missing. Under the StartsWith-based check, the substring does not
        // match the prefix, so SimpleGenericType remains in affected types and is imported.
        var ts = new ParametersBuilder()
            .WithCamelCase()
            .WithRootNamespaces("tests.DTO")
            .WithRootNamespacesExcluded("DTO") // substring but not a prefix of "tests.DTO.*"
            .BuildGenerator()
            .BuildFileFromType(typeof(ComplexType<>));

        Assert.Contains("SimpleGenericType", ts);
    }
}
