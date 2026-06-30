using System.Reflection;
using csh2tscc;
using tests.DTO;

namespace tests;

public class IsNullableHelperTests
{
    private static PropertyInfo Prop(string name) => typeof(NullabilityProbe).GetProperty(name)!;

    [Theory]
    [InlineData(typeof(int?), true)]
    [InlineData(typeof(int), false)]
    [InlineData(typeof(string), false)]
    [InlineData(typeof(DateTime), false)]
    public void IsValueTypeNullable_DetectsNullableValueTypes(Type type, bool expected)
    {
        Assert.Equal(expected, IsNullableHelper.IsValueTypeNullable(type));
    }

    [Theory]
    [InlineData(nameof(NullabilityProbe.Value), false)]
    [InlineData(nameof(NullabilityProbe.NullableValue), true)]
    [InlineData(nameof(NullabilityProbe.Reference), false)]
    [InlineData(nameof(NullabilityProbe.NullableReference), true)]
    public void IsNullable_ReflectsDeclaredNullability(string propertyName, bool expected)
    {
        Assert.Equal(expected, IsNullableHelper.IsNullable(typeof(NullabilityProbe), Prop(propertyName)));
    }

    [Fact]
    public void IsNullableContainer_ValueType_NonNullable_IsFalse()
    {
        var container = IsNullableHelper.IsNullableContainer(typeof(NullabilityProbe), Prop(nameof(NullabilityProbe.Value)));
        Assert.False(container.GetValueAndMoveNext());
    }

    [Fact]
    public void IsNullableContainer_ValueType_Nullable_IsTrue()
    {
        var container = IsNullableHelper.IsNullableContainer(typeof(NullabilityProbe), Prop(nameof(NullabilityProbe.NullableValue)));
        Assert.True(container.GetValueAndMoveNext());
    }

    [Fact]
    public void IsNullableContainer_GenericArg_ReflectsInnerNullability()
    {
        // First generic argument of List<string?> is nullable; of List<string> is not.
        var nullableInner = IsNullableHelper.IsNullableContainer(typeof(NullabilityProbe), Prop(nameof(NullabilityProbe.ListOfNullable)));
        var nonNullableInner = IsNullableHelper.IsNullableContainer(typeof(NullabilityProbe), Prop(nameof(NullabilityProbe.ListOfNonNullable)));

        Assert.True(nullableInner.GetValueAndMoveNext());
        Assert.False(nonNullableInner.GetValueAndMoveNext());
    }
}
