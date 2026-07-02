namespace tests.DTO;

public class CollectionsDto
{
    public int[] Numbers { get; set; } = [];
    public string[] Names { get; set; } = [];
    public List<int> IntList { get; set; } = [];
    public IEnumerable<string> StringEnumerable { get; set; } = [];
    public HashSet<int> IntSet { get; set; } = [];
    public IReadOnlyDictionary<string, int> ReadOnlyMap { get; set; } = new Dictionary<string, int>();
}
