using Dto.Integration.Tests.DTO.Enums;

namespace Dto.Integration.Tests.DTO;

/// <summary>
/// A deployment as the documentation generator sees it.
/// </summary>
/// <remarks>
/// The remarks are carried over too: on the TypeScript side there is nowhere else for them to go,
/// and they usually hold the reason a field exists at all.
/// <para>
/// A second paragraph stays a second paragraph.
/// </para>
/// </remarks>
public class DocumentedDto
{
    /// <summary>Identifier of the deployment.</summary>
    public int Id { get; init; }

    /// <summary>
    /// Who asked for it. See <see cref="UserRole" /> for what they are allowed to ask for.
    /// </summary>
    public string? RequestedBy { get; init; }

    /// <summary>Set to <c>true</c> once every step has reported back.</summary>
    public bool Complete { get; init; }

    /// <summary>A summary carrying a comment terminator */ inside it.</summary>
    public string? Awkward { get; init; }

    /// <summary>
    /// Why it was run. Filled in by <see cref="Describe(string)"/>, <see langword="null"/> until then;
    /// see also <seealso cref="DocumentedStage"/> and <see cref="Detail.Line"/>.
    /// <code>
    /// var reason = deployment.Reason;
    /// </code>
    /// </summary>
    public string? Reason { get; init; }

    /// <remarks>Only remarks, no summary.</remarks>
    public int RemarksOnly { get; init; }

    /// <summary>
    /// Formatting tags keep their <b>text</b>; a link keeps its <see href="https://example.com">caption</see>.
    /// </summary>
    public int Formatted { get; init; }

    /// <summary></summary>
    public int EmptySummary { get; init; }

    public int Undocumented { get; init; }

    /// <summary>Documented, but a method: never rendered, only referenced.</summary>
    /// <param name="reason">What to describe.</param>
    public string Describe(string reason) => reason;

    /// <summary>A nested detail of the deployment.</summary>
    public class Detail
    {
        /// <summary>One detail line.</summary>
        public string? Line { get; init; }
    }
}

/// <summary>How far along a deployment is.</summary>
public enum DocumentedStage
{
    /// <summary>Nothing has started yet.</summary>
    Queued,

    /// <summary>At least one step is running.</summary>
    Running,

    Finished
}

/// <summary>Wraps a <typeparamref name="T"/> for the wire.</summary>
/// <remarks>
/// <para>Paragraphs only.</para>
/// <para>No loose text around them.</para>
/// </remarks>
public class DocumentedEnvelope<T>
{
    /// <summary>The payload.</summary>
    public T? Payload { get; init; }
}
