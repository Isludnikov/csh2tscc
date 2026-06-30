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
