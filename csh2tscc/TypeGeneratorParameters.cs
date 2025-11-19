using System.Collections.Frozen;

namespace csh2tscc;

public class TypesGeneratorParameters
{
    public required FrozenSet<string> LibraryFileNames { get; init; }
    public required FrozenSet<string> RootNamespaces { get; init; }
    public FrozenSet<string> RootNamespacesExcluded { get; init; } = [];

    public FrozenDictionary<string, string> CustomMap { get; init; } =
        new Dictionary<string, string>().ToFrozenDictionary();

    public bool CamelCaseProperties { get; init; }
    public bool UseFullNames { get; init; }

    public FrozenDictionary<string, string> SerializationNamingAttributes { get; init; } =
        new Dictionary<string, string>().ToFrozenDictionary();

    public FrozenSet<string> NoSerializationAttributes { get; init; } = [];
    public string FileExtension { get; init; } = ".tsx";
    public bool CleanOutputDirectory { get; init; }
    public required string OutputDirectory { get; init; }
    public bool Verbose { get; init; }
}