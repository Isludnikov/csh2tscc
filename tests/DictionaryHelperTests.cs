using TypeConverter.CommandLine;

namespace tests;

public class DictionaryHelperTests
{
    [Fact]
    public void SplitToDictionary_ValidPair_ParsesKeyAndValue()
    {
        var result = new[] { "JsonPropertyNameAttribute;Name" }.SplitToDictionary();

        Assert.Single(result);
        Assert.Equal("Name", result["JsonPropertyNameAttribute"]);
    }

    [Fact]
    public void SplitToDictionary_MultiplePairs_ParsesAll()
    {
        var result = new[] { "A;1", "B;2", "C;3" }.SplitToDictionary();

        Assert.Equal(3, result.Count);
        Assert.Equal("1", result["A"]);
        Assert.Equal("2", result["B"]);
        Assert.Equal("3", result["C"]);
    }

    [Fact]
    public void SplitToDictionary_TrimsKeyAndValue()
    {
        var result = new[] { "  Key  ;  Value  " }.SplitToDictionary();

        Assert.Equal("Value", result["Key"]);
    }

    [Fact]
    public void SplitToDictionary_EmptyValue_KeepsEmptyValueButThrowsForMissingSeparator()
    {
        // Documents current behavior: empty value passes validation, no separator does not.
        var emptyValueResult = new[] { "Key;" }.SplitToDictionary();
        Assert.Equal(string.Empty, emptyValueResult["Key"]);
    }

    [Fact]
    public void SplitToDictionary_MissingSeparator_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() => new[] { "BadEntry" }.SplitToDictionary());
        Assert.Contains("BadEntry", ex.Message);
    }

    [Fact]
    public void SplitToDictionary_EmptyKey_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() => new[] { ";Value" }.SplitToDictionary());
        Assert.Contains("Empty key", ex.Message);
    }

    [Fact]
    public void SplitToDictionary_OptionName_MentionedInErrorMessage()
    {
        var ex = Assert.Throws<ArgumentException>(() => new[] { "Bad" }.SplitToDictionary("--customMap"));
        Assert.Contains("--customMap", ex.Message);
    }

    [Fact]
    public void SplitToDictionary_EmptyInput_ReturnsEmptyDictionary()
    {
        var result = Array.Empty<string>().SplitToDictionary();
        Assert.Empty(result);
    }
}
