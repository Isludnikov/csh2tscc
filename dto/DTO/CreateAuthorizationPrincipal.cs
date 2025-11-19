using dto.DTO.Enums;

namespace dto.DTO;

[Serializable]
public class CreateAuthorizationPrincipal
{
    public PrincipalType PrincipalType { get; init; }
    public string Comment { get; init; }
    public HashSet<PrincipalPermission>? Requirements { get; init; }
}