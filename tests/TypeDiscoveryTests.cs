using csh2tscc;
using tests.DTO;
using tests.TestSupport;

namespace tests;

public class TypeDiscoveryTests
{
    private static TypeDiscovery Discovery(ParametersBuilder? builder = null) =>
        new((builder ?? ParametersBuilder.ForLocalDto()).Build());

    [Fact]
    public void ListAffectedTypes_DtoReferencingDto_IncludesReferencedType()
    {
        var affected = Discovery().ListAffectedTypes(typeof(CustomMappedDto));

        Assert.Contains(affected, t => t == typeof(SimpleObject));
    }

    [Fact]
    public void ListAffectedTypes_PrimitivesOnly_IsEmpty()
    {
        // SimpleObject has only int + string properties — neither lives in the processed namespace.
        var affected = Discovery().ListAffectedTypes(typeof(SimpleObject));

        Assert.Empty(affected);
    }

    [Fact]
    public void ListAffectedTypes_Collection_IncludesElementButNotCollectionType()
    {
        var affected = Discovery().ListAffectedTypes(typeof(CollectionOfDtoDto));

        Assert.Contains(affected, t => t == typeof(SimpleObject));
        Assert.DoesNotContain(affected, t => t.Name.StartsWith("List"));
    }

    [Fact]
    public void ListAffectedTypes_CustomMappedReference_IsExcluded()
    {
        var builder = ParametersBuilder.ForLocalDto().WithCustomMap(("SimpleObject", "Mapped"));
        var affected = Discovery(builder).ListAffectedTypes(typeof(CustomMappedDto));

        Assert.DoesNotContain(affected, t => t == typeof(SimpleObject));
    }

    [Fact]
    public void GetTypes_LoadsExportedTypesFromAssembly_AndAppliesNamespaceFilters()
    {
        // Exercises the full assembly-loading + IsExportableType pipeline against the on-disk
        // Dto.Integration.Tests.dll (only otherwise covered indirectly by FullTest).
        var types = new TypeDiscovery(ParametersBuilder.ForIntegrationDll().Build()).GetTypes();

        Assert.Contains(types, t => t.Name == "Account");                 // plain DTO in a processed namespace
        Assert.Contains(types, t => t.Name == "UserRole");                // enum in a sub-namespace
        Assert.DoesNotContain(types, t => t.Namespace?.StartsWith("Dto.Integration.Tests.DTO.Extensions") ?? false);
        Assert.DoesNotContain(types, t => t.Name.StartsWith("<"));        // compiler-generated types
    }
}
