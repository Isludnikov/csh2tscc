namespace tests.DTO;

/// <summary>
/// The compiler emits NullableContext once, on the outermost type, and nested types inherit it.
/// A nested class therefore carries no attribute of its own, and its non-nullable members must
/// not be mistaken for nullable ones.
/// </summary>
public class NestedNullabilityDto
{
    public string A { get; set; } = string.Empty;
    public string B { get; set; } = string.Empty;

    public class Nested
    {
        public string C { get; set; } = string.Empty;
        public string? D { get; set; }
        public int E { get; set; }

        public class Deeper
        {
            public string F { get; set; } = string.Empty;
        }
    }
}
