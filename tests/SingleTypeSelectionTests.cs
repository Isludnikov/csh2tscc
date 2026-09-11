using csh2tscc;
using tests.DTO;
using tests.TestSupport;

namespace tests;

/// <summary>
/// Naming one type in the selection instead of a whole namespace has to mean the same thing to
/// both halves of the pipeline: a file is generated for that type, and properties of that type are
/// imported rather than refused.
/// </summary>
public class SingleTypeSelectionTests
{
    private const string OneType = "tests.DTO.SimpleObject";

    [Fact]
    public void ReferencedTypeSelectedByFullNameIsImported()
    {
        var text = ParametersBuilder.ForLocalDto()
            .WithRootNamespaces("tests.DTO.CustomMappedDto", OneType)
            .BuildGenerator()
            .BuildFileFromType(typeof(CustomMappedDto));

        Assert.Contains("import type { SimpleObject } from './SimpleObject';", text);
        Assert.Contains("SimpleObject", text);
    }

    [Fact]
    public void NeighbouringTypeIsNotDraggedIn()
    {
        var files = ParametersBuilder.ForLocalDto()
            .WithLibraries(typeof(SimpleObject).Assembly.Location)
            .WithRootNamespaces(OneType)
            .BuildGenerator()
            .TransformTypes();

        Assert.Equal(["SimpleObject.tsx"], files.Keys);
    }

    [Fact]
    public void TypeOutsideTheSelectionIsStillRefused()
    {
        // Only the owner is selected, not the SimpleObject it refers to.
        var generator = ParametersBuilder.ForLocalDto()
            .WithRootNamespaces("tests.DTO.CustomMappedDto")
            .BuildGenerator();

        Assert.Throws<UnsupportedTypeException>(() => generator.BuildFileFromType(typeof(CustomMappedDto)));
    }
}
