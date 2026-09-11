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
    public bool UnknownTypesToString { get; init; }

    /// <summary>
    /// Carry the XML documentation of the converted types over into JSDoc comments. Requires the
    /// assemblies to be built with GenerateDocumentationFile; without it the output is unchanged.
    /// </summary>
    public bool GenerateJsDoc { get; init; }

    /// <summary>
    /// Also mark nullable properties optional (<c>name?: T | null</c>) instead of merely allowing
    /// null (<c>name: T | null</c>).
    /// </summary>
    /// <remarks>
    /// Off by default because the two say different things and only one of them is usually true:
    /// a nullable property is written by System.Text.Json like any other, so the field is present
    /// and holds null. Declaring it optional adds undefined to the type of every reader, for a
    /// value that never arrives.
    /// </remarks>
    public bool OptionalNullableProperties { get; init; }
    public FrozenSet<string> ExportAttributes { get; init; } = [];
}