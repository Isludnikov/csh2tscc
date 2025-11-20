using System.Text;

namespace csh2tscc;

internal class StringBuilderCustom(bool verbose)
{
    private readonly StringBuilder _sb = new();

    public void AppendDebugLine(string message)
    {
        if (verbose)
        {
            _sb.AppendLine("//" + message);
        }
    }

    public void Append(char chr)
    {
        _sb.Append(chr);
    }

    public void AppendLine(string? message = null)
    {
        _sb.AppendLine(message);
    }

    public override string ToString() => _sb.ToString();
}