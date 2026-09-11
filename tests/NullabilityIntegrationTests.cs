using Dto.Integration.Tests.DTO.Generics.Enumerable;
using Dto.Integration.Tests.DTO.Generics.MultiGenerics;
using Dto.Integration.Tests.DTO.Nullability;
using tests.TestSupport;

namespace tests;

/// <summary>
/// The nullable annotations of the integration DTOs, checked in the generated text. The dry-run
/// theory only checks that these files are well-formed; this is where their content is pinned.
/// </summary>
public class NullabilityIntegrationTests
{
    private static string Generate(Type type) =>
        ParametersBuilder.ForIntegrationDll().BuildGenerator().BuildFileFromType(type);

    [Fact]
    public void NestedCollectionsCarryEveryAnnotation()
    {
        var text = Generate(typeof(NullabilityTestDto));

        Assert.Contains("name: string;", text);
        Assert.Contains("testDictionary: Record<string, (string | null)[] | null> | null;", text);
        Assert.Contains("testList: (string | null)[];", text);
    }

    [Fact]
    public void GenericArgumentsAndNullableKeysAreKept()
    {
        var text = Generate(typeof(UseMultiGeneric));

        Assert.Contains("data: MultiGeneric<string | null, string> | null;", text);
        // A key admitting null cannot be a Record key.
        Assert.Contains("dict: Map<string | null, Map<number | null, SimpleDataObject | null>> | null;", text);
    }

    [Fact]
    public void OpenGenericParametersFollowTheirOwnFlags()
    {
        var text = Generate(typeof(GenericEnumerable<>));

        Assert.Contains("strNN: string;", text);
        Assert.Contains("str: string | null;", text);
        Assert.Contains("first: T | null;", text);
        Assert.Contains("list: (T | null)[] | null;", text);
        Assert.Contains("list2: (number | null)[];", text);
        Assert.Contains("dictionary: Map<string | null, string | null> | null;", text);
        Assert.Contains("dictionary2: Map<number | null, T> | null;", text);
        Assert.Contains("dictionary3: Map<string | null, T> | null;", text);
        Assert.Contains("arr: T[];", text);
    }

    [Fact]
    public void ClosedGenericArgumentIsWrittenInFull()
    {
        var text = Generate(typeof(UsingGenericEnumerable));

        Assert.Contains("description: GenericEnumerable<number[]>;", text);
        Assert.Contains("testDictionary: Record<string, (string | null)[]> | null;", text);
        Assert.Contains("testList: (string | null)[] | null;", text);
    }
}
