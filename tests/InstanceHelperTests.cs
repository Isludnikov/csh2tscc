using csh2tscc;

namespace tests;

public class InstanceHelperTests
{
    [Fact]
    public void InstanceOfGenericType_MatchingOpenGeneric_True()
    {
        Assert.True(typeof(List<int>).InstanceOfGenericType(typeof(List<>)));
    }

    [Fact]
    public void InstanceOfGenericType_NonGeneric_False()
    {
        Assert.False(typeof(int).InstanceOfGenericType(typeof(List<>)));
    }

    [Fact]
    public void InstanceOfGenericType_DifferentGeneric_False()
    {
        Assert.False(typeof(List<int>).InstanceOfGenericType(typeof(HashSet<>)));
    }

    [Fact]
    public void InstanceOfGenericInterface_SelfIsTheInterface_True()
    {
        Assert.True(typeof(IEnumerable<int>).InstanceOfGenericInterface(typeof(IEnumerable<>)));
    }

    [Fact]
    public void InstanceOfGenericInterface_ImplementedInterface_True()
    {
        Assert.True(typeof(List<int>).InstanceOfGenericInterface(typeof(IEnumerable<>)));
    }

    [Fact]
    public void InstanceOfGenericInterface_Dictionary_ImplementsIDictionary_True()
    {
        Assert.True(typeof(Dictionary<string, int>).InstanceOfGenericInterface(typeof(IDictionary<,>)));
    }

    [Fact]
    public void InstanceOfGenericInterface_TypeWithoutInterface_False()
    {
        Assert.False(typeof(int).InstanceOfGenericInterface(typeof(IEnumerable<>)));
    }
}
