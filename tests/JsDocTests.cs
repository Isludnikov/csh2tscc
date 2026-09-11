using Dto.Integration.Tests.DTO;
using tests.TestSupport;

namespace tests;

/// <summary>
/// Covers carrying the XML documentation of the converted types over into JSDoc. The fixtures live
/// in Dto.Integration.Tests because that project is built with GenerateDocumentationFile — the
/// documentation reaches the generator through the .xml file, not through the assembly metadata.
/// </summary>
public class JsDocTests(ITestOutputHelper testOutputHelper)
{
    private string Generate(Type type, bool jsDoc = true)
    {
        var text = ParametersBuilder.ForIntegrationDll()
            .WithJsDoc(jsDoc)
            .BuildGenerator()
            .BuildFileFromType(type);
        testOutputHelper.WriteLine(text);
        return text;
    }

    [Fact]
    public void InterfaceCarriesSummaryAndRemarks()
    {
        var text = Generate(typeof(DocumentedDto));

        Assert.Contains("/**", text);
        Assert.Contains(" * A deployment as the documentation generator sees it.", text);
        Assert.Contains(" * The remarks are carried over too", text);
        Assert.Contains(" * A second paragraph stays a second paragraph.", text);

        // Summary, remarks and the <para> inside them are separated by blank comment lines.
        Assert.Contains($" */{Environment.NewLine}export interface DocumentedDto {{", text);
    }

    [Fact]
    public void SingleLineSummaryStaysOnOneLine() =>
        Assert.Contains("  /** Identifier of the deployment. */", Generate(typeof(DocumentedDto)));

    [Fact]
    public void SeeCrefBecomesTheReferencedName()
    {
        var text = Generate(typeof(DocumentedDto));
        Assert.Contains("See UserRole for what they are allowed to ask for.", text);
        Assert.DoesNotContain("T:Dto.Integration.Tests", text);
    }

    [Fact]
    public void InlineCodeBecomesBackticks() =>
        Assert.Contains("Set to `true` once every step has reported back.", Generate(typeof(DocumentedDto)));

    [Fact]
    public void CommentTerminatorInsideSummaryIsDefused()
    {
        var text = Generate(typeof(DocumentedDto));

        // A literal */ would close the comment early and leave the rest of the line as code.
        Assert.Contains("A summary carrying a comment terminator *∕ inside it.", text);
        Assert.DoesNotContain("terminator */ inside", text);
    }

    [Fact]
    public void UndocumentedMemberGetsNoComment()
    {
        var text = Generate(typeof(DocumentedDto));
        var lines = text.Split(Environment.NewLine);
        var index = Array.FindIndex(lines, line => line.Contains("undocumented"));

        Assert.True(index > 0, "The undocumented property is missing from the output.");
        Assert.DoesNotContain("*", lines[index - 1]);
    }

    [Fact]
    public void EnumAndItsMembersAreDocumented()
    {
        var text = Generate(typeof(DocumentedStage));

        Assert.Contains("/** How far along a deployment is. */", text);
        Assert.Contains("  /** Nothing has started yet. */", text);
        Assert.Contains("  /** At least one step is running. */", text);
        Assert.Contains("  Finished = 'Finished',", text);
    }

    [Fact]
    public void MethodCrefKeepsOnlyTheMethodName()
    {
        var text = Generate(typeof(DocumentedDto));

        // "M:Dto.Integration.Tests.DTO.DocumentedDto.Describe(System.String)" — the parameter list
        // has dots of its own and must not win.
        Assert.Contains("Filled in by Describe, null until then;", text);
        Assert.DoesNotContain("String)", text);
        Assert.DoesNotContain("M:Dto", text);
    }

    [Fact]
    public void SeeAlsoAndNestedMemberCrefsBecomeTheirLastSegment()
    {
        var text = Generate(typeof(DocumentedDto));
        Assert.Contains("see also DocumentedStage and Line.", text);
    }

    [Fact]
    public void CodeBlockBecomesBackticksOnItsOwnLine()
    {
        var text = Generate(typeof(DocumentedDto));
        Assert.Contains($"{Environment.NewLine}   * `var reason = deployment.Reason;`{Environment.NewLine}", text);
    }

    [Fact]
    public void UnknownInlineTagsKeepTheirTextAndLinksTheirCaption() =>
        Assert.Contains("  /** Formatting tags keep their text; a link keeps its caption. */", Generate(typeof(DocumentedDto)));

    [Fact]
    public void RemarksAloneAreEnoughForAComment() =>
        Assert.Contains("  /** Only remarks, no summary. */", Generate(typeof(DocumentedDto)));

    [Fact]
    public void EmptySummaryGetsNoComment()
    {
        var lines = Generate(typeof(DocumentedDto)).Split(Environment.NewLine);
        var index = Array.FindIndex(lines, line => line.Contains("emptySummary"));

        Assert.True(index > 0, "The emptySummary property is missing from the output.");
        Assert.DoesNotContain("*", lines[index - 1]);
    }

    [Fact]
    public void GenericTypeIsLookedUpByItsDefinition()
    {
        // The compiler documents "T:...DocumentedEnvelope`1"; the arity has to be kept for the lookup.
        var text = Generate(typeof(DocumentedEnvelope<>));

        Assert.Contains("Wraps a T for the wire.", text);
        Assert.Contains("  /** The payload. */", text);
        Assert.Contains("export interface DocumentedEnvelope<T> {", text);
    }

    [Fact]
    public void ParagraphOnlyRemarksAreSeparatedByBlankCommentLines()
    {
        var text = Generate(typeof(DocumentedEnvelope<>));
        Assert.Contains($" * Paragraphs only.{Environment.NewLine} *{Environment.NewLine} * No loose text around them.", text);
    }

    [Fact]
    public void NestedTypeIsLookedUpWithDotsNotPlus()
    {
        // Reflection says "DocumentedDto+Detail"; the documentation file says "DocumentedDto.Detail".
        var text = Generate(typeof(DocumentedDto.Detail));

        Assert.Contains("/** A nested detail of the deployment. */", text);
        Assert.Contains("  /** One detail line. */", text);
    }

    [Fact]
    public void MalformedDocumentationFileIsIgnored()
    {
        // A half-written .xml next to the assembly must not stop generation: the output is still
        // correct, just without comments.
        var dir = Path.Combine(Path.GetTempPath(), "csh2tscc-xmldoc", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            var source = typeof(DocumentedDto).Assembly.Location;
            var copy = Path.Combine(dir, Path.GetFileName(source));
            File.Copy(source, copy);
            File.WriteAllText(Path.ChangeExtension(copy, ".xml"), "<doc><members><member name=\"T:Broken\">");

            var files = ParametersBuilder.ForIntegrationDll()
                .WithLibraries(copy)
                .WithJsDoc()
                .BuildGenerator()
                .TransformTypes();

            var documented = files["DocumentedDto.tsx"];
            Assert.DoesNotContain("/**", documented);
            Assert.Contains("export interface DocumentedDto {", documented);
        }
        finally
        {
            try { Directory.Delete(dir, recursive: true); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
    }

    [Fact]
    public void FlagOffLeavesOutputUnchanged()
    {
        var text = Generate(typeof(DocumentedDto), jsDoc: false);

        Assert.DoesNotContain("/**", text);
        Assert.Contains("export interface DocumentedDto {", text);
    }

    [Fact]
    public void AssemblyWithoutDocumentationFileIsNotAnError()
    {
        // The local test DTOs are compiled without GenerateDocumentationFile: the lookup must come
        // back empty rather than fail, so a half-configured build still generates.
        var text = ParametersBuilder.ForLocalDto()
            .WithJsDoc()
            .BuildGenerator()
            .BuildFileFromType(typeof(DTO.SimpleObject));

        Assert.DoesNotContain("/**", text);
        Assert.Contains("export interface SimpleObject {", text);
    }
}
