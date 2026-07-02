using tests.TestSupport;
using TypeConverter;

namespace tests;

public class ProgramTests
{
    [Fact]
    public void Run_ValidArgs_GeneratesFilesIntoOutputDirectory()
    {
        using var output = new TempOutputDirectory();

        // Mirrors ParametersBuilder.ForIntegrationDll, but driven entirely through the CLI surface
        // so argument parsing, the Options -> TypesGeneratorParameters mapping and the dictionary
        // option splitting are all exercised end to end.
        Program.Run(
        [
            "-o", output.Path,
            "-l", "Dto.Integration.Tests.dll",
            "-n", "Dto.Integration.Tests.DTO",
            "-e", "Dto.Integration.Tests.DTO.Extensions",
            "--camelcase",
            "-s", "JsonStringEnumMemberNameAttribute;Name",
        ]);

        Assert.NotEmpty(Directory.EnumerateFiles(output.Path, "*.tsx"));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)] // --verbose additionally prints the full exception
    public void Run_ParsableArgsButInvalidConfig_ReportsErrorAndSetsExitCode(bool verbose)
    {
        var originalExitCode = Environment.ExitCode;
        try
        {
            using var output = new TempOutputDirectory();

            // Parses fine but fails Options.Validate (neither namespaces nor export attributes):
            // the CLI must report the error and set the exit code instead of crashing.
            string[] args = ["-o", output.Path, "-l", "Dto.Integration.Tests.dll"];
            Program.Run(verbose ? [.. args, "-v"] : args);

            Assert.Equal(1, Environment.ExitCode);
        }
        finally
        {
            Environment.ExitCode = originalExitCode;
        }
    }

    [Fact]
    public void Run_InvalidArgs_InvokesParseErrorHandlerAndSetsExitCode()
    {
        var originalExitCode = Environment.ExitCode;
        try
        {
            // Missing the required -o/-l options: parsing fails and HandleParseError runs.
            Program.Run(["--unknown-flag"]);

            Assert.Equal(-1, Environment.ExitCode);
        }
        finally
        {
            Environment.ExitCode = originalExitCode;
        }
    }
}
