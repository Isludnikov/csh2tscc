using System.ComponentModel.DataAnnotations;

namespace Dto.Integration.Tests.Extensions;

public class CreateWebhook : WebhookBase
{

    [Required]
    public required string ProjectName { get; init; }

    public WebHookTestEnum EnumType { get; init; }

}