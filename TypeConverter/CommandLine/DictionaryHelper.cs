using System.Collections.Frozen;

namespace TypeConverter.CommandLine;

public static class DictionaryHelper
{
    public static FrozenDictionary<string, string> SplitToDictionary(this IEnumerable<string> list) =>
        list.Select(x =>
        {
            var split = x.Split(';');
            return split.Length != 2
                ? throw new Exception($"Bad argument [{x}]")
                : new KeyValuePair<string, string>(split[0], split[1]);
        }).ToFrozenDictionary();
}