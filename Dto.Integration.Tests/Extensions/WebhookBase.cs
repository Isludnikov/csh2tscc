using Dto.Integration.Tests.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Dto.Integration.Tests.Extensions;

[ExportTest]
public class WebhookBase
{
    private static readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

    [Required, MinLength(1), MaxLength(255)]
    public required string Name { get; set; }

    [Required, Range(0, 1)]
    public int Status { get; set; } = 1;
    public string? BodyTemplate { get; set; }
    public JsonObject? Variables { get; set; }
    public required List<string> Environments { get; set; }

    public bool TryGetVariables<T>([NotNullWhen(true)] out T? result)
    {
        result = default;

        if (Variables is null)
        {
            return false;
        }

        try
        {
            result = Variables.Deserialize<T>(_options);
            return result is not null;
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            return false;
        }
    }
}
