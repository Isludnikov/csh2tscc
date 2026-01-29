using csh2tscc;
using Dto.Integration.Tests.DTO.Extensions;
using System.Collections.Frozen;
using System.Text.Json.Serialization;

namespace tests;

public class TypesGeneratorIntegrationTests(ITestOutputHelper testOutputHelper)
{
    [Theory]
    [InlineData("")]
    [InlineData(".Generics.SimpleGenerics")]
    [InlineData(".Generics.Enumerable")]
    [InlineData(".Generics.MultiGenerics")]
    [InlineData(".Required")]
    [InlineData(".Nullability")]
    [InlineData(".NonExisting", "ExportTestAttribute")]
    public void TestDryGeneration(string subpath, string? exportAttribute = null)
    {
        var config = new TypesGeneratorParameters
        {
            CamelCaseProperties = true,
            LibraryFileNames = ["Dto.Integration.Tests.dll"],
            RootNamespaces = ["Dto.Integration.Tests.DTO" + subpath],
            RootNamespacesExcluded = ["Dto.Integration.Tests.DTO.Extensions"],
            SerializationNamingAttributes = new Dictionary<string, string> { { nameof(JsonStringEnumMemberNameAttribute), "Name" }, { nameof(CustomNameAttribute), "CustomName" } }.ToFrozenDictionary(),
            NoSerializationAttributes = [nameof(JsonIgnoreAttribute), nameof(NoSerializeAttribute)],
            ExportAttributes = exportAttribute == null ? [] : [exportAttribute],
            OutputDirectory = "",
            UnknownTypesToString = true,
            Verbose = true
        };
        var generator = new TypesGenerator(config);
        var types = generator.TransformTypes();
        foreach (var type in types)
        {
            testOutputHelper.WriteLine(type.Key);
            testOutputHelper.WriteLine(type.Value);
            testOutputHelper.WriteLine("=====================================================");
        }
    }

    [Fact]
    public void FullTest()
    {
        var generator = new TypesGenerator(new TypesGeneratorParameters
        {
            CamelCaseProperties = true,
            LibraryFileNames = ["Dto.Integration.Tests.dll"],
            RootNamespaces = ["Dto.Integration.Tests.DTO"],
            RootNamespacesExcluded = ["Dto.Integration.Tests.DTO.Extensions"],
            SerializationNamingAttributes = new Dictionary<string, string> { { nameof(JsonStringEnumMemberNameAttribute), "Name" }, { nameof(CustomNameAttribute), "CustomName" } }.ToFrozenDictionary(),
            NoSerializationAttributes = [nameof(JsonIgnoreAttribute), nameof(NoSerializeAttribute)],
            UseFullNames = false,
            FileExtension = ".tsx",
            CleanOutputDirectory = true,
            OutputDirectory = "./generated",
            Verbose = true
        });
        Directory.CreateDirectory(generator.Config.OutputDirectory);
        if (generator.Config.CleanOutputDirectory)
        {
            foreach (var file in new DirectoryInfo(generator.Config.OutputDirectory).EnumerateFiles($"*{generator.Config.FileExtension}"))
            {
                file.Delete();
            }
        }

        var tsClasses = generator.TransformTypes();
        var count = 0;
        foreach (var tsClass in tsClasses)
        {
            File.WriteAllText($"{generator.Config.OutputDirectory}/{tsClass.Key}", tsClass.Value);
            ++count;
            testOutputHelper.WriteLine($"File [{tsClass.Key}] created");
        }
        testOutputHelper.WriteLine($"[{count}] file(s) generated");
    }
}