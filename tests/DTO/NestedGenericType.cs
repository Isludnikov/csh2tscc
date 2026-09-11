namespace tests.DTO;

public class OuterContainer<TOuter>
{
    public TOuter Outer { get; set; }

    public class InnerContainer<TInner>
    {
        public TOuter Outer { get; set; }
        public TInner Inner { get; set; }
    }

    /// <summary>
    /// Not generic by itself, yet reflection reports it as generic over TOuter. It also has no
    /// NullableContext of its own: the compiler put the attribute on OuterContainer, and Label
    /// must still come out non-nullable.
    /// </summary>
    public class Leaf
    {
        public TOuter Outer { get; set; }
        public string Label { get; set; } = string.Empty;
    }
}

// Property typed with a nested CLOSED generic. GetGenericArguments() on
// OuterContainer<int>.InnerContainer<string> returns [int, string] (outer first); the generated
// interface declares both, so the reference passes both.
public class UseNestedGeneric
{
    public OuterContainer<int>.InnerContainer<string> Nested { get; set; }
}

public class UseNestedInGeneric
{
    public OuterContainer<string>.Leaf Leaf { get; set; } = null!;
}
