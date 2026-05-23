namespace tests.DTO;

public class CustomMappedDto
{
    public int Id { get; set; }
    public SimpleObject Mapped { get; set; } = new();
}
