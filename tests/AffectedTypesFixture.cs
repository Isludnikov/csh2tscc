using System.Collections.Frozen;
using tests.DTO;

namespace tests;

public class AffectedTypesFixture
{

    public required Type Klass;
    public FrozenSet<Type> ShouldContain = [];
    public FrozenSet<Type> ShouldNotContain = [];

    public static IEnumerable<TheoryDataRow<AffectedTypesFixture>> GetFixtures()
    {
        yield return new AffectedTypesFixture { Klass = typeof(AffectedType<>), ShouldContain = [typeof(SimpleGenericType<>)], ShouldNotContain = [] };
        // A direct DTO reference must surface as an affected (imported) type.
        yield return new AffectedTypesFixture { Klass = typeof(CustomMappedDto), ShouldContain = [typeof(SimpleObject)] };
        // Inside a collection, the element type surfaces — the collection type itself does not.
        yield return new AffectedTypesFixture { Klass = typeof(CollectionOfDtoDto), ShouldContain = [typeof(SimpleObject)] };
        // A primitives-only DTO has no affected types.
        yield return new AffectedTypesFixture { Klass = typeof(SimpleObject), ShouldNotContain = [typeof(SimpleObject)] };
    }
}