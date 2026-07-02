using csh2tscc;
using tests.TestSupport;

namespace tests;

public class TypeNameCollisionTests
{
    private static ParametersBuilder ForDuplicatesNamespace() => ParametersBuilder.ForIntegrationDll()
        .WithRootNamespaces("Dto.Integration.Tests.Duplicates");

    [Fact]
    public void TransformTypes_SameShortNameInDifferentNamespaces_ThrowsDescriptiveException()
    {
        var generator = ForDuplicatesNamespace().BuildGenerator();

        var ex = Assert.Throws<TypeConversionException>(() => generator.TransformTypes());

        // The message must name the colliding file and every involved type.
        Assert.Contains("Clash.tsx", ex.Message);
        Assert.Contains("Dto.Integration.Tests.Duplicates.First.Clash", ex.Message);
        Assert.Contains("Dto.Integration.Tests.Duplicates.Second.Clash", ex.Message);
    }

    [Fact]
    public void TransformTypes_SameShortName_UseFullNames_Disambiguates()
    {
        var files = ForDuplicatesNamespace()
            .WithUseFullNames()
            .BuildGenerator()
            .TransformTypes();

        Assert.Equal(2, files.Count);
        Assert.Contains("Dto_Integration_Tests_Duplicates_First_Clash.tsx", files.Keys);
        Assert.Contains("Dto_Integration_Tests_Duplicates_Second_Clash.tsx", files.Keys);
    }
}
