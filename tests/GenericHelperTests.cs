using csh2tscc;

namespace tests;

public class GenericHelperTests
{
    private class Outer<T>
    {
        // ReSharper disable once UnusedTypeParameter
        public class Inner<U>;
    }

    [Fact]
    public void NonGenericType_ReturnsZero()
    {
        Assert.Equal(0, GenericHelper.LocalGenericParameterCount(typeof(string)));
    }

    [Fact]
    public void OpenGenericType_ReturnsArgumentCount()
    {
        Assert.Equal(1, GenericHelper.LocalGenericParameterCount(typeof(List<>)));
        Assert.Equal(2, GenericHelper.LocalGenericParameterCount(typeof(Dictionary<,>)));
    }

    [Fact]
    public void NestedGeneric_ExcludesParentParameters()
    {
        // Outer<T>.Inner<U> has 2 total generic args (T, U) but only 1 declared locally (U).
        var innerOpen = typeof(Outer<>).GetNestedType("Inner`1")!;
        Assert.Equal(1, GenericHelper.LocalGenericParameterCount(innerOpen));
    }
}
