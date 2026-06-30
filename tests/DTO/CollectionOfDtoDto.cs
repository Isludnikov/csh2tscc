namespace tests.DTO;

/// <summary>Holds a collection of a DTO type — its element type must surface as an affected/imported type.</summary>
public class CollectionOfDtoDto
{
    public List<SimpleObject> Items { get; set; } = [];
}
