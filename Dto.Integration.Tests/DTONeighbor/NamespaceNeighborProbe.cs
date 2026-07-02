// "Dto.Integration.Tests.DTONeighbor" starts with the literal string
// "Dto.Integration.Tests.DTO" but is a different namespace — assembly-level type
// discovery must not export it when the root namespace is "Dto.Integration.Tests.DTO".
namespace Dto.Integration.Tests.DTONeighbor;

public class NamespaceNeighborProbe
{
    public int Id { get; set; }
}
