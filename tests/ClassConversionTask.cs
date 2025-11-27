using System.Collections.Frozen;
using tests.DTO;

namespace tests;

public class ClassConversionTask
{
    public required Type Klass;
    public FrozenSet<string> ShouldContain = [];
    public FrozenSet<string> ShouldNotContain = [];
    internal static IEnumerable<ClassConversionTask> GetFixtures()
    {
        yield return new ClassConversionTask
        {
            Klass = typeof(SimpleObject),
            ShouldContain = [
                "id: number;",
            "export interface SimpleObject",
            "nameCustom: string;"
            ]
        };
        yield return new ClassConversionTask
        {
            Klass = typeof(SimpleEnum),
            ShouldContain = [
                "export enum SimpleEnum",
            "Zero = 'Zero',",
            "One = 'Mia',",
            "Two = '2',",
            "Three = 'three',"
            ]
        };
        yield return new ClassConversionTask
        {
            Klass = typeof(SimpleGenericType<>),
            ShouldContain = [
                "export interface SimpleGenericType<T>",
                "id: number;",
                "name: string;",
                "data: T;"
            ]
        };
        yield return new ClassConversionTask
        {
            Klass = typeof(MultiGenericType<,>),
            ShouldContain = [
                "export interface MultiGenericType<T,D>",
                "name: string;",
                "type: T;",
                "value?: D | null;"
            ]
        };
        yield return new ClassConversionTask
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
        yield return new ClassConversionTask
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
    }
}