namespace tests.DTO.Extensions
{
    /// <summary>Lives in the sub-namespace the local preset excludes.</summary>
    public class ExcludedHelper
    {
        public int Value { get; set; }
    }
}

namespace tests.DTO
{
    /// <summary>
    /// References a type from an excluded namespace: no file is generated for it, so it must not
    /// be imported, and naming it would leave a dangling reference.
    /// </summary>
    public class ExcludedReferenceDto
    {
        public Extensions.ExcludedHelper Helper { get; set; } = new();
    }
}
