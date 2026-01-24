using csh2tscc;
using Dto.Integration.Tests.DTO.Extensions;
using System.Collections.Frozen;
using System.Text.Json.Serialization;

namespace tests;

public class TypeGeneratorUnitTests
{
    private readonly TypesGeneratorParameters _config = new()
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

    [Theory]
    [MemberData(nameof(ClassConversionTask.GetFixtures), MemberType = typeof(ClassConversionTask))]
    public void TestClassConversion(ClassConversionTask task)
    {
        var tsInterface = TypesGenerator.Create(_config).BuildFileFromType(task.Klass);
        foreach (var definition in task.ShouldContain)
        {
            Assert.Contains(definition, tsInterface);
        }
        foreach (var definition in task.ShouldNotContain)
        {
            Assert.DoesNotContain(definition, tsInterface);
        }
    }
}