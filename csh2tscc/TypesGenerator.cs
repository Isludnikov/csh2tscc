namespace csh2tscc;

public class TypesGenerator
{
    private readonly TypesGeneratorParameters _parameters;
    private readonly TypeDiscovery _discovery;
    private readonly TypeScriptBuilder _builder;

    public TypesGenerator(TypesGeneratorParameters parameters)
    {
        _parameters = parameters;
        _discovery = new TypeDiscovery(parameters);
        _builder = new TypeScriptBuilder(parameters, new TypeResolver(parameters, _discovery), _discovery);
    }

    public static TypesGenerator Create(TypesGeneratorParameters parameters) => new(parameters);

    public TypesGeneratorParameters Config => _parameters;

    public Dictionary<string, string> TransformTypes()
    {
        var types = _discovery.GetTypes();

        var collisions = types.GroupBy(OutputFileName).Where(g => g.Count() > 1).ToList();
        if (collisions.Count > 0)
        {
            var details = string.Join("; ", collisions.Select(g =>
                $"[{g.Key}] <- {string.Join(", ", g.Select(t => t.FullName ?? t.Name))}"));
            throw new TypeConversionException(
                $"Multiple types map to the same output file: {details}. Use UseFullNames or CustomMap to disambiguate.");
        }

        return types.ToDictionary(OutputFileName, _builder.BuildFileFromType);
    }

    private string OutputFileName(Type type) =>
        TypeNameHelper.GetNormalizedTypeScriptName(type, _parameters.UseFullNames) + _parameters.FileExtension;

    internal string BuildFileFromType(Type typeToWrite) => _builder.BuildFileFromType(typeToWrite);

    internal List<Type> ListAffectedTypes(Type type) => _discovery.ListAffectedTypes(type);
}
