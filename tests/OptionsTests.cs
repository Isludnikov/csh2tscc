using CommandLine;
using TypeConverter.CommandLine;

namespace tests;

public class OptionsTests
{
    [Fact]
    public void Validate_NamespacesAndExportAttributesBothEmpty_Throws()
    {
        var opts = new Options { Namespaces = [], ExportAttributes = [] };
        var ex = Assert.Throws<ArgumentException>(opts.Validate);
        Assert.Contains("must not be empty simultaneously", ex.Message);
    }

    [Fact]
    public void Validate_WithNamespaces_DoesNotThrow()
    {
        new Options { Namespaces = ["My.Ns"], ExportAttributes = [] }.Validate();
    }

    [Fact]
    public void Validate_WithExportAttributes_DoesNotThrow()
    {
        new Options { Namespaces = [], ExportAttributes = ["ExportAttribute"] }.Validate();
    }

    [Fact]
    public void Parser_EveryOption_IsBoundToItsProperty()
    {
        using var parser = new Parser(settings => settings.HelpWriter = null);

        var result = parser.ParseArguments<Options>(
        [
            "-o", "out",
            "-l", "a.dll", "b.dll",
            "-n", "Ns.One", "Ns.Two",
            "-a", "ExportAttribute",
            "-e", "Ns.One.Internal",
            "-f", "JsonIgnoreAttribute", "NoSerializeAttribute",
            "--camelcase",
            "--cleanoutputdirectory",
            "--fileextension", ".ts",
            "--usefullnames",
            "-c", "JsonObject;unknown",
            "-s", "JsonPropertyNameAttribute;Name",
            "-v",
            "--unknown2string",
            "--jsdoc",
            "--optionalnullable",
        ]);

        var opts = Assert.IsType<Parsed<Options>>(result).Value;
        Assert.Equal("out", opts.OutputDirectory);
        Assert.Equal(["a.dll", "b.dll"], opts.Libraries);
        Assert.Equal(["Ns.One", "Ns.Two"], opts.Namespaces);
        Assert.Equal(["ExportAttribute"], opts.ExportAttributes);
        Assert.Equal(["Ns.One.Internal"], opts.NamespacesExcluded);
        Assert.Equal(["JsonIgnoreAttribute", "NoSerializeAttribute"], opts.ForbidSerializationAttributes);
        Assert.True(opts.CamelCase);
        Assert.True(opts.CleanOutputDirectory);
        Assert.Equal(".ts", opts.FileExtension);
        Assert.True(opts.UseFullNames);
        Assert.Equal(["JsonObject;unknown"], opts.CustomMap);
        Assert.Equal(["JsonPropertyNameAttribute;Name"], opts.SerializationNaming);
        Assert.True(opts.Verbose);
        Assert.True(opts.UnknownTypeToString);
        Assert.True(opts.GenerateJsDoc);
        Assert.True(opts.OptionalNullableProperties);
    }

    [Fact]
    public void Parser_Defaults_AreOffAndTsx()
    {
        using var parser = new Parser(settings => settings.HelpWriter = null);

        var opts = Assert.IsType<Parsed<Options>>(parser.ParseArguments<Options>(["-o", "out", "-l", "lib.dll"])).Value;

        Assert.Equal(".tsx", opts.FileExtension);
        Assert.False(opts.CamelCase);
        Assert.False(opts.CleanOutputDirectory);
        Assert.False(opts.UseFullNames);
        Assert.False(opts.Verbose);
        Assert.False(opts.UnknownTypeToString);
        Assert.False(opts.GenerateJsDoc);
        Assert.False(opts.OptionalNullableProperties);
        Assert.Empty(opts.Namespaces);
        Assert.Empty(opts.ExportAttributes);
    }

    [Theory]
    [InlineData("-l", "lib.dll")] // -o missing
    [InlineData("-o", "out")]     // -l missing
    public void Parser_MissingRequiredOption_Fails(params string[] args)
    {
        using var parser = new Parser(settings => settings.HelpWriter = null);

        var result = parser.ParseArguments<Options>(args);

        var errors = Assert.IsType<NotParsed<Options>>(result).Errors;
        Assert.Contains(errors, e => e.Tag == ErrorType.MissingRequiredOptionError);
    }

    [Fact]
    public void Parser_ValidArgs_PopulatesRequiredOptions()
    {
        using var parser = new Parser(settings => settings.HelpWriter = null);

        var result = parser.ParseArguments<Options>(["-o", "out", "-l", "lib.dll", "-n", "My.Ns"]);

        var opts = Assert.IsType<Parsed<Options>>(result).Value;
        Assert.Equal("out", opts.OutputDirectory);
        Assert.Contains("lib.dll", opts.Libraries);
        Assert.Contains("My.Ns", opts.Namespaces);
    }
}
