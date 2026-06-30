namespace tests.DTO;

public class OuterContainer<TOuter>
{
    public TOuter Outer { get; set; }

    public class InnerContainer<TInner>
    {
        public TOuter Outer { get; set; }
        public TInner Inner { get; set; }
    }
}

// Property typed with a nested CLOSED generic. GetGenericArguments() on
// OuterContainer<int>.InnerContainer<string> returns [int, string] (outer first),
// so the locally-declared argument is the LAST one (string), not the first.
public class UseNestedGeneric
{
    public OuterContainer<int>.InnerContainer<string> Nested { get; set; }
}
