using tests.DTO;
using tests.TestSupport;

namespace tests;

/// <summary>
/// "May be absent" and "may be null" are different statements, and the generator makes only the
/// second one unless asked for both.
/// </summary>
public class NullableOptionalTests
{
    private static string Build(bool optional) =>
        ParametersBuilder.ForLocalDto()
            .WithOptionalNullable(optional)
            .BuildGenerator()
            .BuildFileFromType(typeof(ComplexType<>));

    [Fact]
    public void NullableIsNotOptionalByDefault()
    {
        var text = Build(optional: false);

        Assert.Contains("description: string | null;", text);
        Assert.DoesNotContain("description?:", text);
    }

    [Fact]
    public void OptionalIsOptedInto()
    {
        var text = Build(optional: true);

        Assert.Contains("description?: string | null;", text);
    }

    [Fact]
    public void NonNullablePropertyIsNeverOptional()
    {
        Assert.Contains("name: string;", Build(optional: true));
    }
}
