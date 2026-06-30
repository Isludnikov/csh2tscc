using csh2tscc;
using tests.TestSupport;

namespace tests;

public class TypesGeneratorIntegrationTests(ITestOutputHelper testOutputHelper)
{
    [Theory]
    [InlineData("", false)]
    [InlineData(".Generics.SimpleGenerics", false)]
    [InlineData(".Generics.Enumerable", false)]
    [InlineData(".Generics.MultiGenerics", false)]
    [InlineData(".Required", false)]
    [InlineData(".Nullability", false)]
    [InlineData(".NonExisting", true, "ExportTestAttribute")]
    public void TestDryGeneration(string subpath, bool emptyOk, string? exportAttribute = null)
    {
        var builder = ParametersBuilder.ForIntegrationDll()
            .WithRootNamespaces("Dto.Integration.Tests.DTO" + subpath)
            .WithUnknownTypesToString()
            .WithVerbose();
        if (exportAttribute != null)
        {
            builder.WithExportAttributes(exportAttribute);
        }

        var types = builder.BuildGenerator().TransformTypes();
        foreach (var type in types)
        {
            testOutputHelper.WriteLine(type.Key);
            testOutputHelper.WriteLine(type.Value);
            testOutputHelper.WriteLine("=====================================================");
        }

        // The .NonExisting namespace has no types — emptyOk allows that case.
        // When ExportTestAttribute is set, only [ExportTest]-marked types must appear.
        if (!emptyOk)
        {
            Assert.NotEmpty(types);
        }

        if (exportAttribute == "ExportTestAttribute")
        {
            Assert.Contains(types.Keys, k => k.StartsWith("WebhookBase"));
            Assert.Contains(types.Keys, k => k.StartsWith("WebHookTestEnum"));
        }

        // Every generated file must be well-formed (header present, no trailing blank line, .tsx).
        foreach (var (key, value) in types)
        {
            TsAssertions.AssertGeneratedFileWellFormed(key, value);
        }

        // No type from the excluded namespace must leak into the output.
        TsAssertions.AssertNoKeysContain(types.Keys, "CustomNameAttribute", "NoSerializeAttribute", "ILocationRequest");
    }

    [Fact]
    public void FullTest()
    {
        using var output = new TempOutputDirectory();
        var generator = ParametersBuilder.ForIntegrationDll()
            .WithCleanOutputDirectory()
            .WithOutputDirectory(output.Path)
            .WithVerbose()
            .BuildGenerator();

        var tsClasses = generator.TransformTypes();
        var count = 0;
        foreach (var tsClass in tsClasses)
        {
            File.WriteAllText(Path.Combine(output.Path, tsClass.Key), tsClass.Value);
            ++count;
            testOutputHelper.WriteLine($"File [{tsClass.Key}] created");
        }
        testOutputHelper.WriteLine($"[{count}] file(s) generated");

        Assert.NotEmpty(tsClasses);
        Assert.Equal(tsClasses.Count, count);

        // Every emitted file lands on disk with the configured extension.
        var onDisk = Directory.EnumerateFiles(output.Path, $"*{generator.Config.FileExtension}")
            .Select(Path.GetFileName)
            .ToHashSet(StringComparer.Ordinal);
        Assert.Equal(tsClasses.Count, onDisk.Count);

        foreach (var (key, value) in tsClasses)
        {
            Assert.Contains(key, onDisk);
            TsAssertions.AssertGeneratedFileWellFormed(key, value);
        }

        // A known enum should be emitted with `export enum`.
        var enumFile = tsClasses.Single(kv => kv.Key == "UserRole.tsx");
        Assert.Contains("export enum UserRole", enumFile.Value);

        // Types from the excluded namespace must not appear.
        TsAssertions.AssertNoKeysContain(tsClasses.Keys, "CustomNameAttribute", "NoSerializeAttribute", "ILocationRequest");
    }
}
