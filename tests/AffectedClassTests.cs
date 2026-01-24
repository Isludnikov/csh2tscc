using csh2tscc;
using Dto.Integration.Tests.DTO.Extensions;
using System.Collections.Frozen;
using System.Text.Json.Serialization;

namespace tests;

public class AffectedClassTests
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
    [MemberData(nameof(AffectedTypesTask.GetFixtures), MemberType = typeof(AffectedTypesTask))]
    public void TestAffectedTypes(AffectedTypesTask task)
    {
        var affectedTypes = TypesGenerator.Create(_config).ListAffectedTypes(task.Klass);

        Assert.True(task.ShouldContain.Count == 0 || task.ShouldContain.All(x => affectedTypes.Any(y => y.GUID == x.GUID)));
        Assert.True(task.ShouldNotContain.Count == 0 || task.ShouldNotContain.All(x => affectedTypes.All(y => y.GUID != x.GUID)));
    }
}