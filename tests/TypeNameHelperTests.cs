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
    public void NormalizeClassName_UsesLastIndexOfBacktick()
    {
        // Nested generics like "Outer`1+Inner`1": LastIndexOf trims at the last backtick.
        Assert.Equal("Outer`1+Inner", TypeNameHelper.NormalizeClassName("Outer`1+Inner`1"));
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
