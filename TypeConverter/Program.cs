using CommandLine;
using csh2tscc;
using System.Collections.Frozen;
using TypeConverter.CommandLine;

namespace TypeConverter;

internal class Program
{
    private static bool _hasErrors;
    private static TypesGeneratorParameters? _typesGeneratorParameters;

    private static int Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(RunOptions)
            .WithNotParsed(HandleParseError);
        if (_hasErrors)
        {
            return -1;
        }

        Executor.Executor.Execute(_typesGeneratorParameters ?? throw new Exception("config is empty"));
        return 0;
    }

    private static void RunOptions(Options opts)
    {
        _typesGeneratorParameters = new TypesGeneratorParameters
        {
            CamelCaseProperties = opts.CamelCase,
            CleanOutputDirectory = opts.CleanOutputDirectory,
            FileExtension = opts.FileExtension,
            LibraryFileNames = opts.Libraries.ToFrozenSet(),
            RootNamespaces = opts.Namespaces.ToFrozenSet(),
            OutputDirectory = opts.OutputDirectory,
            RootNamespacesExcluded = opts.NamespacesExcluded.ToFrozenSet(),
            UseFullNames = opts.UseFullNames,
            NoSerializationAttributes = opts.ForbidSerializationAttributes.ToFrozenSet(),
            CustomMap = opts.CustomMap.SplitToDictionary("--customMap"),
            SerializationNamingAttributes = opts.SerializationNaming.SplitToDictionary("--serializationNaming"),
            Verbose = opts.Verbose
        };
    }

    private static void HandleParseError(IEnumerable<Error> errs)
    {
        _hasErrors = true;
        Console.WriteLine("Execution terminated");
    }
}