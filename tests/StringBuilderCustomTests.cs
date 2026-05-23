using csh2tscc;

namespace tests;

public class StringBuilderCustomTests
{
    [Fact]
    public void AppendLineChar_ProducesSingleLineTerminator()
    {
        var sb = new StringBuilderCustom(verbose: false);
        sb.AppendLine('}');

        var output = sb.ToString();

        Assert.Equal("}" + Environment.NewLine, output);
    }

    [Fact]
    public void AppendLineChar_DoesNotProduceTrailingBlankLine()
    {
        var sb = new StringBuilderCustom(verbose: false);
        sb.AppendLine("interface Foo {");
        sb.AppendLine('}');

        var output = sb.ToString();

        Assert.False(output.EndsWith(Environment.NewLine + Environment.NewLine),
            $"Output ended with a blank line. Actual: [{output.Replace("\r", "\\r").Replace("\n", "\\n")}]");
    }

    [Fact]
    public void AppendDebugLine_VerboseFalse_WritesNothing()
    {
        var sb = new StringBuilderCustom(verbose: false);
        sb.AppendDebugLine("hidden");

        Assert.Equal(string.Empty, sb.ToString());
    }

    [Fact]
    public void AppendDebugLine_VerboseTrue_WritesCommentLine()
    {
        var sb = new StringBuilderCustom(verbose: true);
        sb.AppendDebugLine("hello");

        Assert.Equal("//hello" + Environment.NewLine, sb.ToString());
    }

    [Fact]
    public void AppendLine_NoArgs_WritesLineTerminator()
    {
        var sb = new StringBuilderCustom(verbose: false);
        sb.AppendLine();

        Assert.Equal(Environment.NewLine, sb.ToString());
    }

    [Fact]
    public void AppendLine_NullString_WritesLineTerminator()
    {
        var sb = new StringBuilderCustom(verbose: false);
        sb.AppendLine(null);

        Assert.Equal(Environment.NewLine, sb.ToString());
    }

    [Fact]
    public void Append_Char_DoesNotAddNewline()
    {
        var sb = new StringBuilderCustom(verbose: false);
        sb.Append('x');
        sb.Append('y');

        Assert.Equal("xy", sb.ToString());
    }
}
