using tests.TestSupport;

namespace tests;

public class AffectedClassTests
{
    [Theory]
    [MemberData(nameof(AffectedTypesFixture.GetFixtures), MemberType = typeof(AffectedTypesFixture))]
    public void TestAffectedTypes(AffectedTypesFixture task)
    {
        var affectedTypes = ParametersBuilder.ForLocalDto().BuildGenerator().ListAffectedTypes(task.Klass);

        foreach (var expected in task.ShouldContain)
        {
            Assert.Contains(affectedTypes, t => SameTypeDefinition(t, expected));
        }

        foreach (var forbidden in task.ShouldNotContain)
        {
            Assert.DoesNotContain(affectedTypes, t => SameTypeDefinition(t, forbidden));
        }
    }

    /// <summary>
    /// Affected types may be constructed generics (SimpleGenericType&lt;T&gt;) while fixtures
    /// reference open definitions (SimpleGenericType&lt;&gt;) — compare by generic definition.
    /// </summary>
    private static bool SameTypeDefinition(Type a, Type b) => Definition(a) == Definition(b);

    private static Type Definition(Type type) => type.IsGenericType ? type.GetGenericTypeDefinition() : type;
}
