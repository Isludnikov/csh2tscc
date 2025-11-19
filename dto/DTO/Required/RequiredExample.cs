using System.ComponentModel.DataAnnotations;

namespace dto.DTO.Required;

public class RequiredExample
{
    public int Id { get; set; }

    [Required] public string? Description { get; set; }

    [Required] public RequiredExample? Requ { get; set; }
    public Guid? UserToken { get; init; }

    public RequiredExample? NonRequ { get; set; }
}