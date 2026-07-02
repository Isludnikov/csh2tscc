// Two exported classes with the same short name in different namespaces. With
// UseFullNames=false both would map to "Clash.tsx" — TransformTypes must report the
// collision instead of throwing a bare duplicate-key exception. Lives outside the
// Dto.Integration.Tests.DTO root so the regular integration tests are unaffected.
namespace Dto.Integration.Tests.Duplicates.First
{
    public class Clash
    {
        public int Id { get; set; }
    }
}

namespace Dto.Integration.Tests.Duplicates.Second
{
    public class Clash
    {
        public string? Name { get; set; }
    }
}
