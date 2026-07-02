// "tests.DTONeighbor" starts with the literal string "tests.DTO" but is a different
// namespace. Namespace filtering must respect the dot boundary and not treat it as included.
namespace tests.DTONeighbor
{
    public class NeighborType
    {
        public int Id { get; set; }
    }
}

namespace tests.DTO
{
    public class NeighborHostDto
    {
        public tests.DTONeighbor.NeighborType Neighbor { get; set; } = new();
    }
}
