using tests.TestSupport;

namespace tests;

public class AffectedClassTests
{
    [Theory]
    [MemberData(nameof(AffectedTypesTask.GetFixtures), MemberType = typeof(AffectedTypesTask))]
    public void TestAffectedTypes(AffectedTypesTask task)
    {
        var affectedTypes = ParametersBuilder.ForLocalDto().BuildGenerator().ListAffectedTypes(task.Klass);

        Assert.True(task.ShouldContain.Count == 0 || task.ShouldContain.All(x => affectedTypes.Any(y => y.GUID == x.GUID)));
        Assert.True(task.ShouldNotContain.Count == 0 || task.ShouldNotContain.All(x => affectedTypes.All(y => y.GUID != x.GUID)));
    }
}
