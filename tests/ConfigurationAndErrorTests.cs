using csh2tscc;
using Dto.Integration.Tests.DTO.Extensions;
using System.Collections.Frozen;
using System.Reflection;
using System.Text.Json.Serialization;
using tests.DTO;

namespace tests;

public class ConfigurationAndErrorTests
{
    private static TypesGeneratorParameters BaseConfig() => new()
    {
        CamelCaseProperties = true,
        LibraryFileNames = [],
        RootNamespaces = ["tests.DTO"],
        RootNamespacesExcluded = ["tests.DTO.Extensions"],
        SerializationNamingAttributes = new Dictionary<string, string>
        {
            { nameof(JsonStringEnumMemberNameAttribute), "Name" },
            { nameof(CustomNameAttribute), "CustomName" },
            { nameof(JsonPropertyNameAttribute), "Name" }
        }.ToFrozenDictionary(),
        NoSerializationAttributes = [nameof(JsonIgnoreAttribute), nameof(NoSerializeAttribute)],
        OutputDirectory = "",
        Verbose = false
    };

    // === Fix 1: FileExtension is respected ===

    [Theory]
    [InlineData(".ts")]
    [InlineData(".d.ts")]
    [InlineData(".tsx")]
    public void TransformTypes_UsesConfiguredFileExtension(string extension)
    {
        var config = new TypesGeneratorParameters
        {
            CamelCaseProperties = true,
            LibraryFileNames = ["Dto.Integration.Tests.dll"],
            RootNamespaces = ["Dto.Integration.Tests.DTO"],
            RootNamespacesExcluded = ["Dto.Integration.Tests.DTO.Extensions"],
            SerializationNamingAttributes = new Dictionary<string, string>
            {
                { nameof(JsonStringEnumMemberNameAttribute), "Name" },
                { nameof(CustomNameAttribute), "CustomName" }
            }.ToFrozenDictionary(),
            NoSerializationAttributes = [nameof(JsonIgnoreAttribute), nameof(NoSerializeAttribute)],
            OutputDirectory = "",
            UnknownTypesToString = true,
            FileExtension = extension
        };

        var files = new TypesGenerator(config).TransformTypes();

        Assert.NotEmpty(files);
        Assert.All(files, kv => Assert.EndsWith(extension, kv.Key));
    }

    // === Fix 2: generated TypeScript does not end with a blank line ===

    [Fact]
    public void BuildFileFromType_OutputDoesNotEndWithDoubleNewline()
    {
        var ts = TypesGenerator.Create(BaseConfig()).BuildFileFromType(typeof(SimpleObject));
        Assert.False(ts.EndsWith(Environment.NewLine + Environment.NewLine),
            "Generated TypeScript should not end with a blank line.");
        Assert.EndsWith("}" + Environment.NewLine, ts);
    }

    // === CustomMap path is exercised ===

    [Fact]
    public void CustomMap_ByShortName_OverridesGeneratedType()
    {
        var config = BaseConfig();
        var configWithMap = new TypesGeneratorParameters
        {
            CamelCaseProperties = config.CamelCaseProperties,
            LibraryFileNames = config.LibraryFileNames,
            RootNamespaces = config.RootNamespaces,
            RootNamespacesExcluded = config.RootNamespacesExcluded,
            SerializationNamingAttributes = config.SerializationNamingAttributes,
            NoSerializationAttributes = config.NoSerializationAttributes,
            OutputDirectory = config.OutputDirectory,
            CustomMap = new Dictionary<string, string> { { "SimpleObject", "MyTsType" } }.ToFrozenDictionary()
        };
        var ts = TypesGenerator.Create(configWithMap).BuildFileFromType(typeof(CustomMappedDto));

        Assert.Contains("mapped: MyTsType", ts);
        Assert.DoesNotContain("mapped: SimpleObject", ts);
        Assert.DoesNotContain("import { SimpleObject }", ts);
    }

    // === UseFullNames path is exercised ===

    [Fact]
    public void UseFullNames_True_ProducesUnderscoreSeparatedNames()
    {
        var config = BaseConfig();
        var fullNameConfig = new TypesGeneratorParameters
        {
            CamelCaseProperties = config.CamelCaseProperties,
            LibraryFileNames = config.LibraryFileNames,
            RootNamespaces = config.RootNamespaces,
            RootNamespacesExcluded = config.RootNamespacesExcluded,
            SerializationNamingAttributes = config.SerializationNamingAttributes,
            NoSerializationAttributes = config.NoSerializationAttributes,
            OutputDirectory = config.OutputDirectory,
            UseFullNames = true
        };

        var ts = TypesGenerator.Create(fullNameConfig).BuildFileFromType(typeof(SimpleObject));

        Assert.Contains("export interface tests_DTO_SimpleObject", ts);
    }

    // === Error path: UnsupportedTypeException ===

    [Fact]
    public void UnknownTypesToString_False_ThrowsForUnsupportedType()
    {
        var config = BaseConfig();
        Assert.Throws<UnsupportedTypeException>(() =>
            TypesGenerator.Create(config).BuildFileFromType(typeof(UnknownTypeDto)));
    }

    [Fact]
    public void UnknownTypesToString_True_MapsUnsupportedToString()
    {
        var config = BaseConfig();
        var withUnknown = new TypesGeneratorParameters
        {
            CamelCaseProperties = config.CamelCaseProperties,
            LibraryFileNames = config.LibraryFileNames,
            RootNamespaces = config.RootNamespaces,
            RootNamespacesExcluded = config.RootNamespacesExcluded,
            SerializationNamingAttributes = config.SerializationNamingAttributes,
            NoSerializationAttributes = config.NoSerializationAttributes,
            OutputDirectory = config.OutputDirectory,
            UnknownTypesToString = true
        };
        var ts = TypesGenerator.Create(withUnknown).BuildFileFromType(typeof(UnknownTypeDto));

        Assert.Contains("unsupportedField: string", ts);
    }

    // === Error path: InvalidSerializationConfigException ===

    [Fact]
    public void DuplicateNamingAttributes_OnOneProperty_Throws()
    {
        var ex = Assert.Throws<InvalidSerializationConfigException>(() =>
            TypesGenerator.Create(BaseConfig()).BuildFileFromType(typeof(DuplicateNamingDto)));
        Assert.Contains("more than one serialization naming attribute", ex.Message);
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
        var config = new TypesGeneratorParameters
        {
            CamelCaseProperties = true,
            LibraryFileNames = [],
            RootNamespaces = ["tests.DTO"],
            RootNamespacesExcluded = ["DTO"], // substring but not a prefix of "tests.DTO.*"
            SerializationNamingAttributes = new Dictionary<string, string>().ToFrozenDictionary(),
            NoSerializationAttributes = [],
            OutputDirectory = ""
        };

        var ts = TypesGenerator.Create(config).BuildFileFromType(typeof(ComplexType<>));

        Assert.Contains("SimpleGenericType", ts);
    }

    // === Fix 4: single-character (or empty) property name does not crash camelCase ===

    [Fact]
    public void GetClassPropertyNameToWrite_EmptyString_DoesNotThrow()
    {
        // Reach private method via reflection so we test the guard explicitly.
        var paramsObj = BaseConfig();
        var builderType = typeof(TypesGenerator).Assembly.GetType("csh2tscc.TypeScriptBuilder")!;
        var resolverType = typeof(TypesGenerator).Assembly.GetType("csh2tscc.TypeResolver")!;
        var discoveryType = typeof(TypesGenerator).Assembly.GetType("csh2tscc.TypeDiscovery")!;
        var resolver = Activator.CreateInstance(resolverType, paramsObj)!;
        var discovery = Activator.CreateInstance(discoveryType, paramsObj)!;
        var builder = Activator.CreateInstance(builderType, paramsObj, resolver, discovery)!;
        var method = builderType.GetMethod("GetClassPropertyNameToWrite", BindingFlags.NonPublic | BindingFlags.Instance)!;

        var resultEmpty = method.Invoke(builder, new object[] { string.Empty });
        Assert.Equal(string.Empty, resultEmpty);

        var resultSingle = method.Invoke(builder, new object[] { "X" });
        Assert.Equal("x", resultSingle);
    }
}
