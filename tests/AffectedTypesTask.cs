using System.Collections.Frozen;
using tests.DTO;

namespace tests;

public class AffectedTypesTask
{

    public required Type Klass;
    public FrozenSet<Type> ShouldContain = [];
    public FrozenSet<Type> ShouldNotContain = [];

    internal static IEnumerable<AffectedTypesTask> GetFixtures()
    {
        //yield return new AffectedTypesTask { Klass = typeof(SimpleObject) };
        yield return new AffectedTypesTask { Klass = typeof(AffectedType<>) , ShouldContain = [typeof(SimpleGenericType<>)], ShouldNotContain = [] };
    }
}

