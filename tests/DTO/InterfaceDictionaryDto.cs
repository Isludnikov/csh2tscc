namespace tests.DTO;

/// <summary>
/// A property typed as the <c>IDictionary&lt;,&gt;</c> interface (not a concrete Dictionary)
/// exercises the resolver's fallback to <c>propertyType.GetGenericArguments()</c>.
/// </summary>
public class InterfaceDictionaryDto
{
    public IDictionary<string, int> Map { get; set; } = new Dictionary<string, int>();
}
