using System.Collections.Frozen;

namespace TypeConverter.CommandLine;

public static class DictionaryHelper
{
    /// <summary>
    /// Parses a list of "Key;Value" strings into a dictionary.
    /// </summary>
    /// <param name="list">List of strings in "Key;Value" format</param>
    /// <param name="optionName">Optional name of the CLI option for better error messages</param>
    /// <returns>Frozen dictionary of parsed key-value pairs</returns>
    /// <exception cref="ArgumentException">Thrown when a string doesn't match the expected format</exception>
    public static FrozenDictionary<string, string> SplitToDictionary(this IEnumerable<string> list, string? optionName = null) => list.Select(x => ParseKeyValuePair(x, optionName)).ToFrozenDictionary();

    private static KeyValuePair<string, string> ParseKeyValuePair(string input, string? optionName)
    {
        var split = input.Split(';');

        if (split.Length != 2)
        {
            var optionContext = optionName != null ? $" for option '{optionName}'" : "";
            throw new ArgumentException($"Invalid format{optionContext}: '{input}'. Expected format: 'Key;Value' (e.g., 'JsonPropertyNameAttribute;Name')");
        }

        var key = split[0].Trim();
        var value = split[1].Trim();

        if (string.IsNullOrEmpty(key))
        {
            var optionContext = optionName != null ? $" for option '{optionName}'" : "";
            throw new ArgumentException($"Empty key{optionContext} in '{input}'. Expected format: 'Key;Value'");
        }

        return new KeyValuePair<string, string>(key, value);
    }
}