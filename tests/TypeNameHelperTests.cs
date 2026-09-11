using csh2tscc;

namespace tests;

public class TypeNameHelperTests
{
    [Fact]
    public void NormalizeClassName_StripsGenericArity()
    {
        Assert.Equal("Foo", TypeNameHelper.NormalizeClassName("Foo`1"));
        Assert.Equal("Foo", TypeNameHelper.NormalizeClassName("Foo`2"));
    }

    [Fact]
    public void NormalizeClassName_NonGenericName_Unchanged()
    {
        Assert.Equal("Foo", TypeNameHelper.NormalizeClassName("Foo"));
    }

    [Fact]
    public void NormalizeClassName_StripsEveryArityOfNestedGenerics()
    {
        // Nested generics like "Outer`1+Inner`1" carry one arity suffix per level.
        Assert.Equal("Outer+Inner", TypeNameHelper.NormalizeClassName("Outer`1+Inner`1"));
    }

    [Fact]
    public void NormalizeClassName_StripsGenericMethodArity()
    {
        Assert.Equal("Run", TypeNameHelper.NormalizeClassName("Run``1"));
    }

    [Fact]
    public void GetTypeScriptName_NestedType_FullName_PlusBecomesUnderscore()
    {
        var result = TypeNameHelper.GetNormalizedTypeScriptName(typeof(DTO.OuterContainer<>.InnerContainer<>), useFullNames: true);
        Assert.Equal("tests_DTO_OuterContainer_InnerContainer", result);
    }

    [Fact]
    public void GetTypeScriptName_ConstructedGeneric_IsNamedAfterItsDefinition()
    {
        // The FullName of a constructed generic spells out every argument with its assembly;
        // the name must be the definition's, whatever the arguments are.
        var nested = TypeNameHelper.GetNormalizedTypeScriptName(typeof(List<List<string>>), useFullNames: true);
        var flat = TypeNameHelper.GetNormalizedTypeScriptName(typeof(List<int>), useFullNames: true);

        Assert.Equal("System_Collections_Generic_List", nested);
        Assert.Equal(nested, flat);
    }

    [Fact]
    public void NormalizeClassName_EmptyString_Unchanged()
    {
        Assert.Equal(string.Empty, TypeNameHelper.NormalizeClassName(string.Empty));
    }

    [Fact]
    public void GetTypeScriptName_ShortName_NoDotsInResult()
    {
        var result = TypeNameHelper.GetTypeScriptName(typeof(string), useFullNames: false);
        Assert.Equal("String", result);
    }

    [Fact]
    public void GetTypeScriptName_FullName_DotsReplacedByUnderscore()
    {
        var result = TypeNameHelper.GetTypeScriptName(typeof(string), useFullNames: true);
        Assert.Equal("System_String", result);
    }

    [Fact]
    public void GetTypeScriptName_GenericFullName_Retains_Backtick_For_Normalization()
    {
        var result = TypeNameHelper.GetTypeScriptName(typeof(List<int>).GetGenericTypeDefinition(), useFullNames: true);
        Assert.StartsWith("System_Collections_Generic_List`", result);
    }

    [Fact]
    public void ToCamelCase_Disabled_ReturnsInputUnchanged()
    {
        Assert.Equal("Name", TypeNameHelper.ToCamelCase("Name", camelCase: false));
    }

    [Theory]
    [InlineData("Name", "name")]
    [InlineData("X", "x")]
    [InlineData("ID", "iD")]
    public void ToCamelCase_Enabled_LowersFirstChar(string input, string expected)
    {
        Assert.Equal(expected, TypeNameHelper.ToCamelCase(input, camelCase: true));
    }

    [Fact]
    public void ToCamelCase_EmptyString_DoesNotThrow()
    {
        // Guards the s.Length > 0 check — previously verified via reflection on a private method.
        Assert.Equal(string.Empty, TypeNameHelper.ToCamelCase(string.Empty, camelCase: true));
    }
}
