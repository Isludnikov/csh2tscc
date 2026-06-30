using csh2tscc;
using tests.TestSupport;

namespace tests;

public class TypeGeneratorUnitTests
{
    [Theory]
    [MemberData(nameof(ClassConversionTask.GetFixtures), MemberType = typeof(ClassConversionTask))]
    public void TestClassConversion(ClassConversionTask task)
    {
        var tsInterface = ParametersBuilder.ForLocalDto().BuildGenerator().BuildFileFromType(task.Klass);
        foreach (var definition in task.ShouldContain)
        {
            Assert.Contains(definition, tsInterface);
        }
        foreach (var definition in task.ShouldNotContain)
        {
            Assert.DoesNotContain(definition, tsInterface);
        }
    }
}
