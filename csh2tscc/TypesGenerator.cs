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
        _builder = new TypeScriptBuilder(parameters, new TypeResolver(parameters), _discovery);
    }

    public static TypesGenerator Create(TypesGeneratorParameters parameters) => new(parameters);

    public TypesGeneratorParameters Config => _parameters;

    public Dictionary<string, string> TransformTypes() =>
        _discovery.GetTypes().ToDictionary(
            typeToWrite => TypeNameHelper.NormalizeClassName(TypeNameHelper.GetTypeScriptName(typeToWrite, _parameters.UseFullNames)) + ".tsx",
            _builder.BuildFileFromType);

    internal string BuildFileFromType(Type typeToWrite) => _builder.BuildFileFromType(typeToWrite);

    internal List<Type> ListAffectedTypes(Type type) => _discovery.ListAffectedTypes(type);
}
