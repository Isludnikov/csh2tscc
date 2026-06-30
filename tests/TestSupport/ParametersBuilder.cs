using System.Collections.Frozen;
using System.Text.Json.Serialization;
using csh2tscc;
using Dto.Integration.Tests.DTO.Extensions;

namespace tests.TestSupport;

/// <summary>
/// Fluent builder for <see cref="TypesGeneratorParameters"/>. Centralizes the configuration that
/// was previously assembled field-by-field in every test file (the type's <c>init</c>-only
/// properties forced verbose copy blocks just to tweak one or two values).
/// </summary>
public sealed class ParametersBuilder
{
    private FrozenSet<string> _libraryFileNames = [];
    private FrozenSet<string> _rootNamespaces = [];
    private FrozenSet<string> _rootNamespacesExcluded = [];
    private FrozenDictionary<string, string> _customMap = FrozenDictionary<string, string>.Empty;
    private FrozenDictionary<string, string> _serializationNaming = FrozenDictionary<string, string>.Empty;
    private FrozenSet<string> _noSerializationAttributes = [];
    private FrozenSet<string> _exportAttributes = [];
    private bool _camelCase;
    private bool _useFullNames;
    private bool _cleanOutputDirectory;
    private bool _verbose;
    private bool _unknownTypesToString;
    private string _fileExtension = ".tsx";
    private string _outputDirectory = "";

    /// <summary>Serialization-naming map shared by the local-DTO tests.</summary>
    public static FrozenDictionary<string, string> DefaultNamingAttributes { get; } =
        new Dictionary<string, string>
        {
            { nameof(JsonStringEnumMemberNameAttribute), "Name" },
            { nameof(CustomNameAttribute), "CustomName" },
            { nameof(JsonPropertyNameAttribute), "Name" }
        }.ToFrozenDictionary();

    /// <summary>No-serialization attribute names shared by all tests.</summary>
    public static FrozenSet<string> DefaultNoSerializationAttributes { get; } =
        [nameof(JsonIgnoreAttribute), nameof(NoSerializeAttribute)];

    /// <summary>
    /// Preset for tests that convert DTOs compiled into the test assembly (namespace "tests.DTO").
    /// No external library is loaded — types are handed directly to <c>BuildFileFromType</c>.
    /// </summary>
    public static ParametersBuilder ForLocalDto() => new ParametersBuilder()
        .WithCamelCase()
        .WithRootNamespaces("tests.DTO")
        .WithRootNamespacesExcluded("tests.DTO.Extensions")
        .WithSerializationNaming(DefaultNamingAttributes)
        .WithNoSerializationAttributes(DefaultNoSerializationAttributes);

    /// <summary>
    /// Preset for tests that load Dto.Integration.Tests.dll from disk and run the full
    /// discovery + generation pipeline.
    /// </summary>
    public static ParametersBuilder ForIntegrationDll() => new ParametersBuilder()
        .WithCamelCase()
        .WithLibraries("Dto.Integration.Tests.dll")
        .WithRootNamespaces("Dto.Integration.Tests.DTO")
        .WithRootNamespacesExcluded("Dto.Integration.Tests.DTO.Extensions")
        .WithSerializationNaming(new Dictionary<string, string>
        {
            { nameof(JsonStringEnumMemberNameAttribute), "Name" },
            { nameof(CustomNameAttribute), "CustomName" }
        }.ToFrozenDictionary())
        .WithNoSerializationAttributes(DefaultNoSerializationAttributes);

    public ParametersBuilder WithLibraries(params string[] libraries) { _libraryFileNames = libraries.ToFrozenSet(); return this; }
    public ParametersBuilder WithRootNamespaces(params string[] namespaces) { _rootNamespaces = namespaces.ToFrozenSet(); return this; }
    public ParametersBuilder WithRootNamespacesExcluded(params string[] namespaces) { _rootNamespacesExcluded = namespaces.ToFrozenSet(); return this; }
    public ParametersBuilder WithCustomMap(params (string Key, string Value)[] entries) { _customMap = entries.ToFrozenDictionary(e => e.Key, e => e.Value); return this; }
    public ParametersBuilder WithSerializationNaming(FrozenDictionary<string, string> map) { _serializationNaming = map; return this; }
    public ParametersBuilder WithNoSerializationAttributes(FrozenSet<string> attributes) { _noSerializationAttributes = attributes; return this; }
    public ParametersBuilder WithExportAttributes(params string[] attributes) { _exportAttributes = attributes.ToFrozenSet(); return this; }
    public ParametersBuilder WithCamelCase(bool value = true) { _camelCase = value; return this; }
    public ParametersBuilder WithUseFullNames(bool value = true) { _useFullNames = value; return this; }
    public ParametersBuilder WithCleanOutputDirectory(bool value = true) { _cleanOutputDirectory = value; return this; }
    public ParametersBuilder WithVerbose(bool value = true) { _verbose = value; return this; }
    public ParametersBuilder WithUnknownTypesToString(bool value = true) { _unknownTypesToString = value; return this; }
    public ParametersBuilder WithFileExtension(string extension) { _fileExtension = extension; return this; }
    public ParametersBuilder WithOutputDirectory(string directory) { _outputDirectory = directory; return this; }

    public TypesGeneratorParameters Build() => new()
    {
        LibraryFileNames = _libraryFileNames,
        RootNamespaces = _rootNamespaces,
        RootNamespacesExcluded = _rootNamespacesExcluded,
        CustomMap = _customMap,
        CamelCaseProperties = _camelCase,
        UseFullNames = _useFullNames,
        SerializationNamingAttributes = _serializationNaming,
        NoSerializationAttributes = _noSerializationAttributes,
        FileExtension = _fileExtension,
        CleanOutputDirectory = _cleanOutputDirectory,
        OutputDirectory = _outputDirectory,
        Verbose = _verbose,
        UnknownTypesToString = _unknownTypesToString,
        ExportAttributes = _exportAttributes
    };

    /// <summary>Shortcut for <c>TypesGenerator.Create(Build())</c>.</summary>
    public TypesGenerator BuildGenerator() => TypesGenerator.Create(Build());
}
