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
        // A direct DTO reference must surface as an affected (imported) type.
        yield return new AffectedTypesTask { Klass = typeof(CustomMappedDto), ShouldContain = [typeof(SimpleObject)] };
        // Inside a collection, the element type surfaces — the collection type itself does not.
        yield return new AffectedTypesTask { Klass = typeof(CollectionOfDtoDto), ShouldContain = [typeof(SimpleObject)] };
        // A primitives-only DTO has no affected types.
        yield return new AffectedTypesTask { Klass = typeof(SimpleObject), ShouldNotContain = [typeof(SimpleObject)] };
    }
}