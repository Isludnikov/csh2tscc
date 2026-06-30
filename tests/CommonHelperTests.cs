using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using csh2tscc;

namespace tests;

public class CommonHelperTests
{
    private class Samples
    {
        public string Plain { get; set; } = "";

        [Required]
        public string? Required { get; set; }

        [NotNull]
        public string? NotNullOnProperty { get; set; }

        // [return: NotNull] lands on the getter's return parameter, not the property —
        // exercises the second branch of HasNonNullableAttribute.
        public string? NotNullOnGetter { [return: NotNull] get; set; }
    }

    private static PropertyInfo Prop(string name) => typeof(Samples).GetProperty(name)!;

    [Fact]
    public void HasNonNullableAttribute_PlainProperty_False()
    {
        Assert.False(CommonHelper.HasNonNullableAttribute(Prop(nameof(Samples.Plain))));
    }

    [Fact]
    public void HasNonNullableAttribute_RequiredProperty_True()
    {
        Assert.True(CommonHelper.HasNonNullableAttribute(Prop(nameof(Samples.Required))));
    }

    [Fact]
    public void HasNonNullableAttribute_NotNullOnProperty_True()
    {
        Assert.True(CommonHelper.HasNonNullableAttribute(Prop(nameof(Samples.NotNullOnProperty))));
    }

    [Fact]
    public void HasNonNullableAttribute_NotNullOnGetterReturn_True()
    {
        Assert.True(CommonHelper.HasNonNullableAttribute(Prop(nameof(Samples.NotNullOnGetter))));
    }

    [Theory]
    [InlineData(true, "number | null")]
    [InlineData(false, "number")]
    public void GetPropertyTypeWithNullable_AppendsUnionWhenNullable(bool nullable, string expected)
    {
        Assert.Equal(expected, CommonHelper.GetPropertyTypeWithNullable("number", nullable));
    }
}
