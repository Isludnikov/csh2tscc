using csh2tscc;
using tests.DTO;
using tests.TestSupport;

namespace tests;

/// <summary>Import lines and the type names they and the file names are built from.</summary>
public class ImportTests
{
    private static int Count(string text, string fragment) =>
        (text.Length - text.Replace(fragment, string.Empty).Length) / fragment.Length;

    [Fact]
    public void GenericConstructedTwiceIsImportedOnce()
    {
        var text = ParametersBuilder.ForLocalDto().BuildGenerator().BuildFileFromType(typeof(DeepGenericDto));

        Assert.Equal(1, Count(text, "import type { Wrapper } from './Wrapper';"));
    }

    [Fact]
    public void FullNames_ConstructedGenericIsNamedAfterItsDefinition()
    {
        var text = ParametersBuilder.ForLocalDto().WithUseFullNames().BuildGenerator().BuildFileFromType(typeof(DeepGenericDto));

        Assert.Equal(1, Count(text, "import type { tests_DTO_Wrapper } from './tests_DTO_Wrapper';"));
        Assert.Contains("deep: tests_DTO_Wrapper<tests_DTO_Wrapper<string>>;", text);
        Assert.DoesNotContain("`", text);
        Assert.DoesNotContain("[[", text);
    }

    [Fact]
    public void FullNames_NestedTypesDoNotCollideWithTheirOuterType()
    {
        // OuterContainer`1 and OuterContainer`1+Leaf used to normalize to the same name.
        var files = ParametersBuilder.ForLocalDto()
            .WithUseFullNames()
            .WithLibraries(typeof(OuterContainer<>).Assembly.Location)
            .WithRootNamespaces("tests.DTO.OuterContainer")
            .BuildGenerator()
            .TransformTypes();

        Assert.Contains("tests_DTO_OuterContainer.tsx", files.Keys);
        Assert.Contains("tests_DTO_OuterContainer_InnerContainer.tsx", files.Keys);
        Assert.Contains("tests_DTO_OuterContainer_Leaf.tsx", files.Keys);
        Assert.Contains("export interface tests_DTO_OuterContainer_InnerContainer<TOuter, TInner>", files["tests_DTO_OuterContainer_InnerContainer.tsx"]);
        Assert.All(files.Keys, key => Assert.DoesNotContain("+", key));
    }

    [Fact]
    public void FullNames_NestedReferenceUsesTheSameName()
    {
        var text = ParametersBuilder.ForLocalDto().WithUseFullNames().BuildGenerator().BuildFileFromType(typeof(UseNestedInGeneric));

        Assert.Contains("import type { tests_DTO_OuterContainer_Leaf } from './tests_DTO_OuterContainer_Leaf';", text);
        Assert.Contains("leaf: tests_DTO_OuterContainer_Leaf<string>;", text);
    }

    [Fact]
    public void ReferenceToExcludedNamespace_IsRefusedAndNotImported()
    {
        var generator = ParametersBuilder.ForLocalDto().BuildGenerator();

        var ex = Assert.Throws<UnsupportedTypeException>(() => generator.BuildFileFromType(typeof(ExcludedReferenceDto)));
        Assert.Equal(typeof(DTO.Extensions.ExcludedHelper), ex.UnsupportedType);
    }

    [Fact]
    public void ReferenceToExcludedNamespace_BecomesStringWhenAsked()
    {
        var text = ParametersBuilder.ForLocalDto().WithUnknownTypesToString().BuildGenerator().BuildFileFromType(typeof(ExcludedReferenceDto));

        Assert.Contains("helper: string;", text);
        Assert.DoesNotContain("import", text);
    }
}
