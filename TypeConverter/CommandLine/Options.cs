using CommandLine;

namespace TypeConverter.CommandLine;

public class Options
{
    [Option('o', "out", Required = true, HelpText = "OutputDirectory")]
    public string OutputDirectory { get; set; }

    [Option('l', "libraries", Required = true, HelpText = "Libraries files to be processed")]
    public IEnumerable<string> Libraries { get; set; }

    [Option('n', "namespaces", Required = false, HelpText = "Namespaces to be processed")]
    public IEnumerable<string> Namespaces { get; set; }
    [Option('a', "exportAttributes", Required = false, HelpText = "Export attributes")]
    public IEnumerable<string> ExportAttributes { get; set; }

    [Option('e', "namespacesExcluded", Required = false, HelpText = "Namespaces to be excluded")]
    public IEnumerable<string> NamespacesExcluded { get; set; } = [];

    [Option('f', "forbidAttributes", Required = false, HelpText = "Attributes forbid the serialization of property")]
    public IEnumerable<string> ForbidSerializationAttributes { get; set; } = [];

    [Option(Default = false, HelpText = "Properties names to camelCase")]
    public bool CamelCase { get; set; }

    [Option(Default = false, HelpText = "Clean output directory from previously generated files")]
    public bool CleanOutputDirectory { get; set; }

    [Option(Default = ".tsx", HelpText = "files extension")]
    public string FileExtension { get; set; }

    [Option(Default = false, HelpText = "use full names")]
    public bool UseFullNames { get; set; }

    [Option('c', "customMap", Required = false,
        HelpText = "Dictionary for custom mapping. Example \"BadConstructedClass;Map<string, unknown>\"")]
    public IEnumerable<string> CustomMap { get; set; } = [];

    [Option('s', "serializationNaming", Required = true,
        HelpText = "Dictionary for serialization naming. Example \"JsonPropertyNameAttribute;Name\"")]
    public IEnumerable<string> SerializationNaming { get; set; } = [];

    [Option('v', "verbose", Required = false, Default = false, HelpText = "Verbose output")]
    public bool Verbose { get; set; }
    [Option("unknown2string", Required = false, Default = false, HelpText = "Map unknown type to string")]
    public bool UnknownTypeToString { get; set; }
}