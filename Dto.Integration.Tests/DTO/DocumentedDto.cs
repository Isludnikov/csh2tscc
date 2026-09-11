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

    public int Undocumented { get; init; }
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
