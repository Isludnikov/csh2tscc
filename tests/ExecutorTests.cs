using tests.TestSupport;
using ExecutorRunner = TypeConverter.Executor.Executor;

namespace tests;

public class ExecutorTests
{
    [Fact]
    public void Execute_WritesGeneratedFilesToOutputDirectory()
    {
        using var output = new TempOutputDirectory();
        var config = ParametersBuilder.ForIntegrationDll().WithOutputDirectory(output.Path).Build();

        ExecutorRunner.Execute(config);

        var written = Directory.EnumerateFiles(output.Path, "*.tsx").ToList();
        Assert.NotEmpty(written);
    }

    [Fact]
    public void Execute_CleanOutputDirectory_RemovesOnlyMatchingExtension()
    {
        using var output = new TempOutputDirectory();
        var keep = Path.Combine(output.Path, "keep.txt");
        var stale = Path.Combine(output.Path, "stale-not-generated.tsx");
        File.WriteAllText(keep, "keep");
        File.WriteAllText(stale, "old");

        var config = ParametersBuilder.ForIntegrationDll()
            .WithOutputDirectory(output.Path)
            .WithCleanOutputDirectory()
            .Build();

        ExecutorRunner.Execute(config);

        Assert.True(File.Exists(keep), "Non-.tsx files must not be removed by CleanOutputDirectory.");
        Assert.False(File.Exists(stale), "Stale .tsx files must be removed before writing.");
        Assert.NotEmpty(Directory.EnumerateFiles(output.Path, "*.tsx"));
    }
}
