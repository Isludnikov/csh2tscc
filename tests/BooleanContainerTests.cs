using csh2tscc;

namespace tests;

public class BooleanContainerTests
{
    [Fact]
    public void CreateTrue_AlwaysReturnsTrue()
    {
        var container = BooleanContainer.CreateTrue();
        Assert.True(container.GetValueAndMoveNext());
        Assert.True(container.GetValueAndMoveNext());
    }

    [Fact]
    public void CreateFalse_AlwaysReturnsFalse()
    {
        var container = BooleanContainer.CreateFalse();
        Assert.False(container.GetValueAndMoveNext());
        Assert.False(container.GetValueAndMoveNext());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void EmptyFlags_ReturnsDefaultNullable(bool defaultNullable)
    {
        var container = new BooleanContainer([], defaultNullable);
        Assert.Equal(defaultNullable, container.GetValueAndMoveNext());
        Assert.Equal(defaultNullable, container.GetValueAndMoveNext());
    }

    [Fact]
    public void Flags_IterationStartsAtIndexOne_ThenFallsBackToIndexZero()
    {
        // Index 0 is the property type's own nullability; generic args start at index 1.
        var container = new BooleanContainer([false, true, false], defaultNullable: true);

        Assert.True(container.GetValueAndMoveNext());   // index 1
        Assert.False(container.GetValueAndMoveNext());  // index 2
        Assert.False(container.GetValueAndMoveNext());  // exhausted -> base flag (index 0 == false)
        Assert.False(container.GetValueAndMoveNext());  // still index 0
    }

    [Fact]
    public void SingleFlag_UsedForEveryPosition()
    {
        // Only index 0 is present, so position 1 is already past the end -> always index 0.
        var container = new BooleanContainer([true], defaultNullable: false);

        Assert.True(container.GetValueAndMoveNext());
        Assert.True(container.GetValueAndMoveNext());
    }
}
