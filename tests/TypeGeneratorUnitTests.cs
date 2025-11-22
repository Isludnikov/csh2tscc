using System.Collections.Frozen;
using csh2tscc;
using dto.DTO.Extensions;
using System.Text.Json.Serialization;

namespace tests;

public class TypeGeneratorUnitTests
{
    public required TypesGeneratorParameters Config;
    [SetUp]
    public void Setup()
    {
        Config = new TypesGeneratorParameters
        {
            CamelCaseProperties = true,
            LibraryFileNames = [],
            RootNamespaces = ["tests.DTO"],
            RootNamespacesExcluded = ["tests.DTO.Extensions"],
            SerializationNamingAttributes = new Dictionary<string, string> { { nameof(JsonStringEnumMemberNameAttribute), "Name" }, { nameof(CustomNameAttribute), "CustomName" }, { nameof(JsonPropertyNameAttribute), "Name" } }.ToFrozenDictionary(),
            NoSerializationAttributes = [nameof(JsonIgnoreAttribute), nameof(NoSerializeAttribute)],
            OutputDirectory = "",
            Verbose = false
        };
    }

    [TestCaseSource(typeof(ClassConversionTask), nameof(ClassConversionTask.GetFixtures))]
    public void TestClassConversion(ClassConversionTask task)
    {
        var tsInterface = TypesGenerator.Create(Config).BuildFileFromType(task.Klass);
        foreach (var definition in task.ShouldContain)
        {
            Assert.That(tsInterface, Does.Contain(definition));
        }
        foreach (var definition in task.ShouldNotContain)
        {
            Assert.That(tsInterface, Does.Not.Contain(definition));
        }
    }
}