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

    [Fact]
    public void Run_EveryOption_ReachesTheGenerator()
    {
        var originalExitCode = Environment.ExitCode;
        try
        {
            using var output = new TempOutputDirectory();
            var stale = Path.Combine(output.Path, "stale.ts");
            File.WriteAllText(stale, "old");

            Program.Run(
            [
                "-o", output.Path,
                "-l", "Dto.Integration.Tests.dll",
                "-n", "Dto.Integration.Tests.DTO",
                "-e", "Dto.Integration.Tests.DTO.Extensions",
                "-a", "ExportTestAttribute",
                "-f", "NoSerializeAttribute",
                "-c", "JsonObject;unknown",
                "-s", "CustomNameAttribute;CustomName",
                "--camelcase",
                "--cleanoutputdirectory",
                "--fileextension", ".ts",
                "--usefullnames",
                "--unknown2string",
                "--jsdoc",
                "--optionalnullable",
            ]);

            Assert.Equal(0, Environment.ExitCode);
            Assert.False(File.Exists(stale), "--cleanoutputdirectory must remove stale files.");

            var files = Directory.EnumerateFiles(output.Path).Select(Path.GetFileName).ToList();
            Assert.All(files, name => Assert.EndsWith(".ts", name));                       // --fileextension
            Assert.Contains("Dto_Integration_Tests_DTO_Account.ts", files);                 // --usefullnames
            Assert.Contains("Dto_Integration_Tests_Extensions_WebhookBase.ts", files);      // -a

            var account = File.ReadAllText(Path.Combine(output.Path, "Dto_Integration_Tests_DTO_Account.ts"));
            Assert.Contains("SuperDomain?: string | null;", account);                       // -s, --camelcase, --optionalnullable
            Assert.DoesNotContain("team", account);                                         // -f

            var webhook = File.ReadAllText(Path.Combine(output.Path, "Dto_Integration_Tests_Extensions_WebhookBase.ts"));
            Assert.Contains("variables?: unknown | null;", webhook);                        // -c

            var documented = File.ReadAllText(Path.Combine(output.Path, "Dto_Integration_Tests_DTO_DocumentedDto.ts"));
            Assert.Contains("/**", documented);                                             // --jsdoc
        }
        finally
        {
            Environment.ExitCode = originalExitCode;
        }
    }

    [Theory]
    [InlineData("--help")]
    [InlineData("--version")]
    public void Run_HelpOrVersion_IsNotAnError(string flag)
    {
        var originalExitCode = Environment.ExitCode;
        try
        {
            Environment.ExitCode = 0;

            Program.Run([flag]);

            Assert.Equal(0, Environment.ExitCode);
        }
        finally
        {
            Environment.ExitCode = originalExitCode;
        }
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
