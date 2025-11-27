using csh2tscc;
using Dto.Integration.Tests.DTO.Extensions;
using System.Collections.Frozen;
using System.Text.Json.Serialization;

namespace tests;

public class AffectedClassTests
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
    [Test]
    [TestCaseSource(typeof(AffectedTypesTask), nameof(AffectedTypesTask.GetFixtures))]
    public void TestAffectedTypes(AffectedTypesTask task)
    {
        var affectedTypes = TypesGenerator.Create(Config).ListAffectedTypes(task.Klass);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(task.ShouldContain.Count == 0 || task.ShouldContain.All(x => affectedTypes.Any(y => y.GUID == x.GUID)), Is.True);
            Assert.That(task.ShouldNotContain.Count == 0 || task.ShouldNotContain.All(x => affectedTypes.All(y => y.GUID != x.GUID)), Is.True);
        }

    }
}

