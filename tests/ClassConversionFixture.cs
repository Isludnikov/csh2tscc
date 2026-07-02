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
                "value?: D | null;"
            ]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(ComplexType<>),
            ShouldContain = [
                "export interface ComplexType<T>",
                "id: number;",
                "number?: number | null;",
                "name: string;",
                "description?: string | null;",
                "guid: string;",
                "dictionary?: Map<string, Map<T, SimpleGenericType<T | null> | null> | null> | null;"
            ]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(AttributedType),
            ShouldContain = [
                "export interface AttributedType",
                "id: number;", "name: string;",
                "number2?: number | null;",
                "description: string;",
                "UUID: string;"
            ],
            ShouldNotContain = [
                "number?: number | null;",
                "guid: string;",
                "description?: string | null;"
            ]
        };
        yield return new ClassConversionFixture
        {
            // The property is OuterContainer<int>.InnerContainer<string>. The locally-declared
            // generic argument is the inner one (string), so the emitted type must be
            // InnerContainer<string>, NOT InnerContainer<number> (the outer arg).
            Klass = typeof(UseNestedGeneric),
            ShouldContain = [
                "export interface UseNestedGeneric",
                "nested: InnerContainer<string>;"
            ],
            ShouldNotContain = [
                "InnerContainer<number>"
            ]
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
                "readOnlyMap: Map<string, number>;"
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
            Klass = typeof(ArrayOfComplexDto),
            ShouldContain = [
                "import { SimpleObject }",
                "export interface ArrayOfComplexDto",
                "items: SimpleObject[];"
            ]
        };
        yield return new ClassConversionFixture
        {
            Klass = typeof(InterfaceDictionaryDto),
            ShouldContain = [
                "export interface InterfaceDictionaryDto",
                "map: Map<string, number>;"
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
                "parent?: TreeNode<T> | null;",
                "children: TreeNode<T>[];"
            ],
            ShouldNotContain = [
                "import"
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