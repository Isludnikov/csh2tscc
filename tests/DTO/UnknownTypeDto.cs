namespace tests.DTO;

public class UnknownTypeDto
{
    public int Id { get; set; }
    public System.IO.Stream UnsupportedField { get; set; } = null!;
}
