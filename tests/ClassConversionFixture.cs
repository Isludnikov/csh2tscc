using System.Collections.Frozen;
using tests.DTO;

namespace tests;

public class ClassConversionFixture
{
    public required Type Klass;
    public FrozenSet<string> ShouldContain = [];
    public FrozenSet<string> ShouldNotContain = [];

    public static IEnumerable<TheoryDataRow<ClassConversionFixture>> GetFixtures()
    {
        yield return new ClassConversionFixture
        {
            Klass = typeof(SimpleObject),
            ShouldContain = [
                "id: number;",
            "export interface SimpleObject",
            "nameCustom: string;"
            ]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(SimpleEnum),
            ShouldContain = [
                "export enum SimpleEnum",
            "Zero = 'Zero',",
            "One = 'Mia',",
            "Two = '2',",
            "Three = 'three',",
            @"Four = 'O\'Brien',"
            ]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(SimpleGenericType<>),
            ShouldContain = [
                "export interface SimpleGenericType<T>",
                "id: number;",
                "name: string;",
                "data: T;"
            ]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(MultiGenericType<,>),
            ShouldContain = [
                "export interface MultiGenericType<T, D>",
                "name: string;",
                "type: T;",
                "value: D | null;"
            ]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(ComplexType<>),
            ShouldContain = [
                "export interface ComplexType<T>",
                "id: number;",
                "number: number | null;",
                "name: string;",
                "description: string | null;",
                "guid: string;",
                "dictionary: Record<string, Map<T, SimpleGenericType<T | null> | null> | null> | null;"
            ]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(AttributedType),
            ShouldContain = [
                "export interface AttributedType",
                "id: number;", "name: string;",
                "number2: number | null;",
                "description: string;",
                "UUID: string;"
            ],
            ShouldNotContain = [
                "number: number | null;",
                "guid: string;",
                "description: string | null;"
            ]
        };
        yield return new ClassConversionFixture
        {
            // The property is OuterContainer<int>.InnerContainer<string>. The generated interface
            // declares both parameters (outer first, and the members use TOuter), so a reference
            // has to pass both — outer first, the way reflection lists them.
            Klass = typeof(UseNestedGeneric),
            ShouldContain = [
                "export interface UseNestedGeneric",
                "import type { InnerContainer } from './InnerContainer';",
                "nested: InnerContainer<number, string>;"
            ],
            ShouldNotContain = [
                "InnerContainer<string>;",
                "InnerContainer<number>;"
            ]
        };
        yield return new ClassConversionFixture
        {
            // The nested generic itself: its header lists the outer parameter too, since it is used.
            Klass = typeof(OuterContainer<>.InnerContainer<>),
            ShouldContain = [
                "export interface InnerContainer<TOuter, TInner>",
                "outer: TOuter;",
                "inner: TInner;"
            ]
        };
        yield return new ClassConversionFixture
        {
            // A non-generic type nested in a generic one is still generic (it carries T) and used
            // to crash the resolver, which looked for "locally declared" parameters and found none.
            Klass = typeof(UseNestedInGeneric),
            ShouldContain = [
                "import type { Leaf } from './Leaf';",
                "leaf: Leaf<string>;"
            ]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(OuterContainer<>.Leaf),
            ShouldContain = [
                "export interface Leaf<TOuter>",
                "outer: TOuter;",
                "label: string;"
            ],
            ShouldNotContain = ["label: string | null;"]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(CollectionsDto),
            ShouldContain = [
                "export interface CollectionsDto",
                "numbers: number[];",
                "names: string[];",
                "intList: number[];",
                "stringEnumerable: string[];",
                "intSet: number[];",
                "readOnlyMap: Record<string, number>;"
            ],
            ShouldNotContain = [
                "number[] | null",
                "string[] | null"
            ]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(ToStringTypesDto),
            ShouldContain = [
                "export interface ToStringTypesDto",
                "id: string;",        // Guid
                "created: string;",   // DateTime
                "link: string;",      // Uri
                "duration: string;"   // TimeSpan
            ]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(ObjectPropertyDto),
            ShouldContain = [
                "export interface ObjectPropertyDto",
                "payload: unknown;"
            ]
        };
        yield return new ClassConversionFixture
        {
            // An enum is a runtime object and must be imported as a value; an interface exists
            // only at compile time and has to say so where verbatimModuleSyntax is on.
            Klass = typeof(MixedImportsDto),
            ShouldContain = [
                "import { SimpleEnum } from './SimpleEnum';",
                "import type { SimpleObject } from './SimpleObject';",
                "stage: SimpleEnum;",
                "payload: SimpleObject;"
            ],
            ShouldNotContain = ["import type { SimpleEnum }"]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(ArrayOfComplexDto),
            ShouldContain = [
                "import type { SimpleObject } from './SimpleObject';",
                "export interface ArrayOfComplexDto",
                "items: SimpleObject[];"
            ]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(InterfaceDictionaryDto),
            ShouldContain = [
                "export interface InterfaceDictionaryDto",
                "map: Record<string, number>;"
            ]
        };
        yield return new ClassConversionFixture
        {
            // Non-generic subclass of List<string>: element type must come from the
            // implemented IEnumerable<T>, not from the (empty) own generic arguments.
            Klass = typeof(NonGenericCollectionDto),
            ShouldContain = [
                "export interface NonGenericCollectionDto",
                "items: string[];"
            ]
        };
        yield return new ClassConversionFixture
        {
            // Static properties and indexers are not serialized and must not be emitted.
            Klass = typeof(StaticAndIndexerDto),
            ShouldContain = [
                "export interface StaticAndIndexerDto",
                "id: number;"
            ],
            ShouldNotContain = [
                "staticName",
                "item:"
            ]
        };
        yield return new ClassConversionFixture
        {
            // Recursive generic: self-references (direct and inside a collection) must not
            // import the type's own file — TreeNode<T> constructed != typeof(TreeNode<>).
            Klass = typeof(TreeNode<>),
            ShouldContain = [
                "export interface TreeNode<T>",
                "value: T;",
                "parent: TreeNode<T> | null;",
                "children: TreeNode<T>[];"
            ],
            ShouldNotContain = [
                "import"
            ]
        };
        yield return new ClassConversionFixture
        {
            // The nullable annotation of an element is its own flag, and a union element is
            // parenthesised: "string | null[]" would mean string | (null[]).
            Klass = typeof(ArrayNullabilityDto),
            ShouldContain = [
                "import type { SimpleObject } from './SimpleObject';",
                "nullableElements: (string | null)[];",
                "nullableArray: string[] | null;",
                "nullableArrayOfNullable: (string | null)[] | null;",
                "nullableIntArray: number[] | null;",
                "nullableIntElements: (number | null)[];",
                "nullableItems: (SimpleObject | null)[];",
                "listOfNullable: (string | null)[];",
                "listOfNullableInt: (number | null)[];",
                "nullableListOfNullableItems: (SimpleObject | null)[] | null;",
                "deep: Record<string, (string | null)[] | null> | null;",
                "jagged: number[][];",
                "jaggedOfNullable: (string | null)[][];",
                "arrayOfLists: (string | null)[][];"
            ],
            ShouldNotContain = ["| null[]", "((", "))"]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(GenericKeyDictionaryDto),
            ShouldContain = [
                "import { SimpleEnum } from './SimpleEnum';",
                "genericKey: Map<string[], string | null>;",
                "nullableStringKey: Map<string | null, number>;",
                "nullableEnumKey: Map<SimpleEnum | null, number>;",
                "nullableIntKey: Map<number | null, string>;",
                "subclass: Record<string, number>;"
            ]
        };
        yield return new ClassConversionFixture
        {
            // The nested class has no NullableContext of its own; it inherits the outer one.
            Klass = typeof(NestedNullabilityDto.Nested),
            ShouldContain = [
                "export interface Nested {",
                "c: string;",
                "d: string | null;",
                "e: number;"
            ],
            ShouldNotContain = ["c: string | null;"]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(NestedNullabilityDto.Nested.Deeper),
            ShouldContain = ["f: string;"],
            ShouldNotContain = ["f: string | null;"]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(GenericStructDto),
            ShouldContain = [
                "import type { Pair } from './Pair';",
                "pair: Pair<string | null, string>;",
                "pairWithValue: Pair<number, string | null>;",
                "nullablePair: Pair<string, string> | null;"
            ]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(Pair<,>),
            ShouldContain = ["export interface Pair<TA, TB>", "a: TA;", "b: TB;"]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(DeepGenericDto),
            ShouldContain = [
                "import type { Wrapper } from './Wrapper';",
                "import type { SimpleObject } from './SimpleObject';",
                "deep: Wrapper<Wrapper<string>>;",
                "flat: Wrapper<number>;",
                "deepNullable: Wrapper<Wrapper<SimpleObject> | null>;"
            ],
            ShouldNotContain = ["`", "[["]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(QuotedNameDto),
            ShouldContain = [
                "'kebab-name': string;",
                "'with space': number;",
                "'123start': number;",
                @"'it\'s': number;",
                "$ok_1: number;"
            ],
            ShouldNotContain = ["kebab-name:", "'$ok_1'"]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(HiddenPropertyDto),
            ShouldContain = ["id: string;", "name: string;"],
            ShouldNotContain = ["id: number;"]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(InheritedContextDto),
            ShouldContain = ["count: number;", "title: string;", "subtitle: string | null;"],
            ShouldNotContain = ["  title: string | null;"]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(IDerivedShape),
            ShouldContain = ["export interface IDerivedShape", "age: number;", "name: string;", "id: number;"]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(RecordDto),
            ShouldContain = ["export interface RecordDto", "id: number;", "name: string | null;"],
            ShouldNotContain = ["equalityContract"]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(StructDto),
            ShouldContain = ["export interface StructDto", "x: number;"]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(AbstractDtoBase),
            ShouldContain = ["export interface AbstractDtoBase", "id: number;"]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(EscapedEnum),
            ShouldContain = [
                @"Backslash = 'back\\slash',",
                "NullNamed = 'NullNamed',",
                "Plain = 'Plain',"
            ]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(IgnoredEnum),
            ShouldContain = [
                "export enum IgnoredEnum",
                "Visible = 'Visible',",
                "Renamed = 'renamed',"
            ],
            ShouldNotContain = [
                "Hidden"
            ]
        };
    }
}