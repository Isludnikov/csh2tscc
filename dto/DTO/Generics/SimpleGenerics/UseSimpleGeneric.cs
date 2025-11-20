using System.ComponentModel.DataAnnotations;

namespace dto.DTO.Generics.SimpleGenerics;

public class UseSimpleGeneric
{
    [Required] public int? Index { get; set; }
    [Required] public Guid? UserToken { get; init; }
    public SimpleGeneric<double> SimpleGeneric { get; set; }
    public SimpleGeneric<MockEnum> SimpleGeneric2 { get; set; }
}