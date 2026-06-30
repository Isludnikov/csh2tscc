using CommandLine;
using csh2tscc;
using System.Collections.Frozen;
using TypeConverter.CommandLine;

namespace TypeConverter;

internal class Program
{
    private static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(RunOptions)
            .WithNotParsed(HandleParseError);
    }

    private static void RunOptions(Options opts)
    {
        opts.Validate();

        Executor.Executor.Execute(new TypesGeneratorParameters
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
            Verbose = opts.Verbose,
            UnknownTypesToString = opts.UnknownTypeToString,
            ExportAttributes = opts.ExportAttributes.ToFrozenSet(),
        });
    }

    private static void HandleParseError(IEnumerable<Error> errs)
    {
        Console.Error.WriteLine("Failed to parse command-line arguments.");
        Environment.ExitCode = -1;
    }
}