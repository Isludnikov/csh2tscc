namespace tests.DTO;

/// <summary>Ссылается и на перечисление, и на интерфейс: импорты у них разные.</summary>
public class MixedImportsDto
{
    public SimpleEnum Stage { get; set; }

    public SimpleObject Payload { get; set; } = new();
}
