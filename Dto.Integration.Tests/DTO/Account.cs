using Dto.Integration.Tests.DTO.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Dto.Integration.Tests.DTO;

public class Account
{
    [NoSerialize] public int Team { get; init; }

    [CustomName("SuperDomain")] public string? Domain { get; init; }

    [NotNull] public List<string>? Environments { get; init; }
}