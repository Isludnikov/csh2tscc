using csh2tscc;
using dto.DTO.Extensions;
using System.Collections.Frozen;
using System.Text.Json.Serialization;

namespace tests;

public class TypesGeneratorTests
{
    [Test]
    [TestCase("")]
    [TestCase(".Generics.SimpleGenerics")]
    [TestCase(".Generics.Enumerable")]
    [TestCase(".Generics.MultiGenerics")]
    [TestCase(".Required")]
    [TestCase(".Nullability")]
    public void TestDryGeneration(string subpath)
    {
        var config = new TypesGeneratorParameters
        {
            CamelCaseProperties = true,
            LibraryFileNames = ["dto.dll"],
            RootNamespaces = ["dto.DTO" + subpath],
            RootNamespacesExcluded = ["dto.DTO.Extensions"],
            SerializationNamingAttributes = new Dictionary<string, string> { { nameof(JsonStringEnumMemberNameAttribute), "Name" }, { nameof(CustomNameAttribute), "CustomName" } }.ToFrozenDictionary(),
            NoSerializationAttributes = [nameof(JsonIgnoreAttribute), nameof(NoSerializeAttribute)],
            OutputDirectory = "",
            Verbose = true
        };
        var generator = new TypesGenerator(config);
        var types = generator.TransformTypes();
        foreach (var type in types)
        {
            Console.WriteLine(type.Key);
            Console.WriteLine(type.Value);
            Console.WriteLine("=====================================================");
        }
    }

    [Test]
    public void FullTest()
    {
        var generator = new TypesGenerator(new TypesGeneratorParameters
        {
            CamelCaseProperties = true,
            LibraryFileNames = ["dto.dll"],
            RootNamespaces = ["dto.DTO"],
            RootNamespacesExcluded = ["dto.DTO.Extensions"],
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
            Console.WriteLine($"File [{tsClass.Key}] created");
        }

        Console.WriteLine($"[{count}] file(s) generated");
    }
}