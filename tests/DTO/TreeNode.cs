namespace tests.DTO;

/// <summary>
/// Recursive generic DTO: references to itself (directly and inside a collection)
/// must not produce an import of the type's own file.
/// </summary>
public class TreeNode<T>
{
    public T Value { get; set; } = default!;
    public TreeNode<T>? Parent { get; set; }
    public List<TreeNode<T>> Children { get; set; } = [];
}
