using System.Collections.Frozen;
using tests.DTO;

namespace tests;

public class AffectedTypesTask
{

    public required Type Klass;
    public FrozenSet<Type> ShouldContain = [];
    public FrozenSet<Type> ShouldNotContain = [];

    public static IEnumerable<TheoryDataRow<AffectedTypesTask>> GetFixtures()
    {
        yield return new AffectedTypesTask { Klass = typeof(AffectedType<>), ShouldContain = [typeof(SimpleGenericType<>)], ShouldNotContain = [] };
    }
}