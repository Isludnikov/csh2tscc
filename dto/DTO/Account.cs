using System.Diagnostics.CodeAnalysis;
using dto.DTO.Extensions;

namespace dto.DTO;

public class Account
{
    [NoSerialize] public int Team { get; init; }

    [CustomName("SuperDomain")] public string? Domain { get; init; }

    [NotNull] public List<string>? Environments { get; init; }
}