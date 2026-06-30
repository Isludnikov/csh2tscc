using Dto.Integration.Tests.DTO.Extensions;

namespace tests.DTO;

/// <summary>
/// The property carries a serialization-naming attribute whose configured name property
/// resolves to <c>null</c>. With <c>throwOnNullName: true</c> (the class-property path),
/// generation must surface an <see cref="csh2tscc.AttributeProcessingException"/>.
/// </summary>
public class NullNameDto
{
    [CustomName(null!)]
    public string Name { get; set; } = string.Empty;
}
