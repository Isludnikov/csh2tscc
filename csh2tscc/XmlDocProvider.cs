using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace csh2tscc;

/// <summary>
/// Reads the XML documentation files the C# compiler writes next to the assemblies being
/// converted, and renders the prose for a type or a member as a JSDoc block.
/// </summary>
/// <remarks>
/// The file is looked up next to the assembly the type comes from rather than next to the
/// libraries named on the command line: types reach the builder from both places, and the
/// assembly always knows where it was loaded from. An assembly built without
/// <c>GenerateDocumentationFile</c> simply has no file, and every lookup on it yields nothing —
/// documentation is an enrichment, never a precondition.
/// </remarks>
internal sealed class XmlDocProvider
{
    private readonly Dictionary<string, Dictionary<string, XElement>> _byAssembly = [];

    internal string? ForType(Type type) => Render(Lookup(type.Assembly, TypeDocId(type)));

    internal string? ForMember(MemberInfo member)
    {
        var declaringType = member.DeclaringType;
        if (declaringType == null)
        {
            return null;
        }

        var prefix = member is FieldInfo ? 'F' : 'P';
        var id = $"{prefix}:{MemberScope(declaringType)}.{member.Name}";
        return Render(Lookup(declaringType.Assembly, id));
    }

    private static string TypeDocId(Type type) => $"T:{MemberScope(type)}";

    /// <summary>
    /// The type name as XML documentation ids spell it: nested types separated by a dot rather
    /// than a plus, generics kept in their `arity form, constructed generics reduced to their
    /// definition (the compiler documents the definition, not each construction).
    /// </summary>
    private static string MemberScope(Type type)
    {
        var subject = type is { IsGenericType: true, IsGenericTypeDefinition: false }
            ? type.GetGenericTypeDefinition()
            : type;

        var name = subject.FullName ?? $"{subject.Namespace}.{subject.Name}";
        return name.Replace('+', '.');
    }

    private XElement? Lookup(Assembly assembly, string id)
    {
        var members = MembersOf(assembly);
        return members != null && members.TryGetValue(id, out var member) ? member : null;
    }

    private Dictionary<string, XElement>? MembersOf(Assembly assembly)
    {
        var location = assembly.IsDynamic ? string.Empty : assembly.Location;
        if (string.IsNullOrEmpty(location))
        {
            return null;
        }

        if (_byAssembly.TryGetValue(location, out var cached))
        {
            return cached;
        }

        var members = Load(Path.ChangeExtension(location, ".xml"));
        _byAssembly[location] = members;
        return members;
    }

    private static Dictionary<string, XElement> Load(string xmlPath)
    {
        if (!File.Exists(xmlPath))
        {
            return [];
        }

        // A malformed or half-written documentation file must not stop code generation: the
        // output is still correct without comments.
        try
        {
            return XDocument.Load(xmlPath)
                .Descendants("member")
                .Where(member => member.Attribute("name") != null)
                .GroupBy(member => member.Attribute("name")!.Value)
                .ToDictionary(group => group.Key, group => group.First());
        }
        catch (Exception e) when (e is System.Xml.XmlException or IOException or UnauthorizedAccessException)
        {
            return [];
        }
    }

    /// <summary>
    /// Renders summary and remarks as a JSDoc block, or null when the member carries neither.
    /// </summary>
    private static string? Render(XElement? member)
    {
        if (member == null)
        {
            return null;
        }

        var paragraphs = new[] { "summary", "remarks" }
            .Select(name => member.Element(name))
            .Where(section => section != null)
            .SelectMany(section => Paragraphs(section!))
            .ToList();

        if (paragraphs.Count == 0)
        {
            return null;
        }

        return paragraphs.Count == 1 && !paragraphs[0].Contains('\n')
            ? $"/** {paragraphs[0]} */"
            : BuildBlock(paragraphs);
    }

    private static string BuildBlock(IEnumerable<string> paragraphs)
    {
        var sb = new StringBuilder("/**");
        var first = true;
        foreach (var paragraph in paragraphs)
        {
            if (!first)
            {
                sb.Append(Environment.NewLine).Append(" *");
            }

            first = false;
            foreach (var line in paragraph.Split('\n'))
            {
                sb.Append(Environment.NewLine).Append(" * ").Append(line);
            }
        }

        return sb.Append(Environment.NewLine).Append(" */").ToString();
    }

    /// <summary>
    /// Splits one documentation section into paragraphs: every &lt;para&gt; is its own, and the
    /// text around them forms another. Line breaks inside a paragraph are preserved, so the
    /// wrapping the author chose in the C# source survives into the generated file.
    /// </summary>
    private static IEnumerable<string> Paragraphs(XElement section)
    {
        foreach (var node in ParagraphNodes(section))
        {
            var text = Flatten(node);
            if (text.Length > 0)
            {
                yield return text;
            }
        }
    }

    private static IEnumerable<XElement> ParagraphNodes(XElement section)
    {
        var paragraphs = section.Elements("para").ToList();
        if (paragraphs.Count == 0)
        {
            return [section];
        }

        // Text outside <para> still belongs to the section; it is documentation the author wrote.
        var loose = new XElement(section.Name, section.Nodes().Where(node => node is not XElement { Name.LocalName: "para" }));
        return Flatten(loose).Length == 0 ? paragraphs : paragraphs.Prepend(loose);
    }

    /// <summary>
    /// Turns the mixed content of a documentation node into plain text: inline tags become the
    /// thing they refer to, and the leading indentation the compiler copies from the source file
    /// is dropped line by line.
    /// </summary>
    private static string Flatten(XElement node)
    {
        var sb = new StringBuilder();
        foreach (var child in node.Nodes())
        {
            switch (child)
            {
                case XText text:
                    sb.Append(text.Value);
                    break;
                case XElement { Name.LocalName: "see" or "seealso" } reference:
                    sb.Append(ReferenceText(reference));
                    break;
                case XElement { Name.LocalName: "paramref" or "typeparamref" } reference:
                    sb.Append(reference.Attribute("name")?.Value ?? string.Empty);
                    break;
                case XElement { Name.LocalName: "c" or "code" } literal:
                    sb.Append('`').Append(literal.Value.Trim()).Append('`');
                    break;
                case XElement element:
                    sb.Append(Flatten(element));
                    break;
            }
        }

        return Normalize(sb.ToString());
    }

    private static string ReferenceText(XElement reference)
    {
        var target = reference.Attribute("cref")?.Value ?? reference.Attribute("langword")?.Value;
        if (string.IsNullOrEmpty(target))
        {
            return reference.Value.Trim();
        }

        // "T:My.Namespace.Type" and "P:My.Namespace.Type.Prop" both reduce to the last segment:
        // the full name is a C# address that means nothing on the TypeScript side. A method's
        // parameter list ("M:My.Type.Run(System.String)") goes first, or its dots would win.
        var withoutPrefix = target.Length > 1 && target[1] == ':' ? target[2..] : target;
        var withoutParameters = withoutPrefix.Split('(')[0];
        return TypeNameHelper.NormalizeClassName(withoutParameters.Split('.').Last());
    }

    private static string Normalize(string raw)
    {
        var lines = raw.Replace("\r\n", "\n").Split('\n').Select(line => line.Trim()).ToList();
        while (lines.Count > 0 && lines[0].Length == 0)
        {
            lines.RemoveAt(0);
        }

        while (lines.Count > 0 && lines[^1].Length == 0)
        {
            lines.RemoveAt(lines.Count - 1);
        }

        // A blank line inside a section would close nothing and read as a stray " *" line; the
        // paragraph split above is the only thing allowed to produce those.
        var text = string.Join('\n', lines.Where(line => line.Length > 0));

        // A comment cannot contain its own terminator.
        return text.Replace("*/", "*∕");
    }

    /// <summary>Shifts a rendered block to the nesting level of the declaration it documents.</summary>
    internal static string Indent(string block, string indent) =>
        string.Join(Environment.NewLine, block.Split(Environment.NewLine).Select(line => indent + line));
}
