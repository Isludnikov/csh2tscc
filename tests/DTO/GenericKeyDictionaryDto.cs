#pragma warning disable CS8714 // nullable key: not a runtime concern here, the metadata is what is under test

namespace tests.DTO;

/// <summary>
/// Dictionaries whose key needs care: a generic key owns nullable flags of its own that sit
/// between the key's and the value's flag, and a key admitting null cannot be a Record key.
/// </summary>
public class GenericKeyDictionaryDto
{
    public Dictionary<List<string>, string?> GenericKey { get; set; } = [];
    public Dictionary<string?, int> NullableStringKey { get; set; } = [];
    public Dictionary<SimpleEnum?, int> NullableEnumKey { get; set; } = [];
    public Dictionary<int?, string> NullableIntKey { get; set; } = [];
    public StringIntMap Subclass { get; set; } = [];
}

/// <summary>Non-generic subclass of a dictionary: the key/value types come from the interface.</summary>
public class StringIntMap : Dictionary<string, int>;
