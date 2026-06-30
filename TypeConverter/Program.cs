using CommandLine;
using csh2tscc;
using System.Collections.Frozen;
using TypeConverter.CommandLine;

namespace TypeConverter;

internal static class Program
{
    private static void Main(string[] args) => Run(args);

    /// <summary>
    /// Parses <paramref name="args"/> and dispatches to the generation pipeline. Extracted from
    /// <c>Main</c> so the argument-handling wiring can be unit-tested without launching the process.
    /// </summary>
    internal static void Run(string[] args) => Parser.Default.ParseArguments<Options>(args)
            .WithParsed(RunOptions)
            .WithNotParsed(HandleParseError);

    internal static void RunOptions(Options opts)
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
            CustomMap = opts.CustomMap.SplitToDictionary($"--{Options.CustomMapOption}"),
            SerializationNamingAttributes = opts.SerializationNaming.SplitToDictionary($"--{Options.SerializationNamingOption}"),
            Verbose = opts.Verbose,
            UnknownTypesToString = opts.UnknownTypeToString,
            ExportAttributes = opts.ExportAttributes.ToFrozenSet(),
        });
    }

    internal static void HandleParseError(IEnumerable<Error> errs)
    {
        Console.Error.WriteLine("Failed to parse command-line arguments.");
        Environment.ExitCode = -1;
    }
}