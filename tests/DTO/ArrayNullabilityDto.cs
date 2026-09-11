namespace tests.DTO;

/// <summary>
/// Every placement of the nullable annotation around an array or a collection. The compiler
/// writes the array and its element as two separate nullable flags (string?[] is [1, 2], string[]?
/// is [2, 1]); a union element must be parenthesised or "string | null[]" means string | (null[]).
/// </summary>
public class ArrayNullabilityDto
{
    public string?[] NullableElements { get; set; } = [];
    public string[]? NullableArray { get; set; }
    public string?[]? NullableArrayOfNullable { get; set; }
    public int[]? NullableIntArray { get; set; }
    public int?[] NullableIntElements { get; set; } = [];
    public SimpleObject?[] NullableItems { get; set; } = [];
    public List<string?> ListOfNullable { get; set; } = [];
    public List<int?> ListOfNullableInt { get; set; } = [];
    public List<SimpleObject?>? NullableListOfNullableItems { get; set; }
    public Dictionary<string, List<string?>?>? Deep { get; set; }
    public int[][] Jagged { get; set; } = [];
    public string?[][] JaggedOfNullable { get; set; } = [];
    public List<string?>[] ArrayOfLists { get; set; } = [];
}
