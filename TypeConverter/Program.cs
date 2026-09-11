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
        try
        {
            RunOptionsCore(opts);
        }
        catch (Exception ex)
        {
            // Configuration and generation errors surface as a readable message, not a stack
            // trace; --verbose prints the full exception for troubleshooting.
            Console.Error.WriteLine($"Error: {ex.Message}");
            if (opts.Verbose)
            {
                Console.Error.WriteLine(ex);
            }

            Environment.ExitCode = 1;
        }
    }

    private static void RunOptionsCore(Options opts)
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
            GenerateJsDoc = opts.GenerateJsDoc,
            OptionalNullableProperties = opts.OptionalNullableProperties,
            ExportAttributes = opts.ExportAttributes.ToFrozenSet(),
        });
    }

    internal static void HandleParseError(IEnumerable<Error> errs)
    {
        // --help and --version also arrive here; the parser has already printed what was asked
        // for, and answering a question is not a failure.
        var errors = errs.ToList();
        if (errors.IsHelp() || errors.IsVersion())
        {
            return;
        }

        Console.Error.WriteLine("Failed to parse command-line arguments.");
        Environment.ExitCode = -1;
    }
}