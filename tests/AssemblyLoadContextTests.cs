using System.Reflection;
using csh2tscc;

namespace tests;

public class AssemblyLoadContextTests
{
    /// <summary>Exposes the protected <see cref="CustomAssemblyLoadContext.Load"/> override for testing.</summary>
    private sealed class ProbeLoadContext(IEnumerable<string> basePath) : CustomAssemblyLoadContext(basePath)
    {
        public Assembly? InvokeLoad(AssemblyName name) => Load(name);
    }

    [Fact]
    public void Load_AssemblyPresentInBasePath_LoadsFromThatPath()
    {
        // Copy a real managed assembly under a name that is not loaded in the default context,
        // so resolution falls through to the base-path probe rather than returning an already
        // loaded assembly.
        var dir = Path.Combine(Path.GetTempPath(), "csh2tscc-alc", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            var probeName = "AlcProbe_" + Guid.NewGuid().ToString("N");
            var probePath = Path.Combine(dir, probeName + ".dll");
            File.Copy(typeof(TypesGenerator).Assembly.Location, probePath);

            var context = new ProbeLoadContext([dir]);
            var loaded = context.InvokeLoad(new AssemblyName(probeName));

            Assert.NotNull(loaded);
        }
        finally
        {
            // The probe assembly stays locked while loaded in the collectible context, so deletion
            // is best-effort — the OS reclaims the temp directory regardless.
            try { Directory.Delete(dir, recursive: true); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
    }

    [Fact]
    public void Load_UnresolvableAssembly_ReturnsNull()
    {
        // Not loaded, not present in any base path, and unknown to the default context:
        // the FileNotFoundException fallback must be swallowed and null returned.
        var context = new ProbeLoadContext([]);

        var loaded = context.InvokeLoad(new AssemblyName("Missing_" + Guid.NewGuid().ToString("N")));

        Assert.Null(loaded);
    }

    [Fact]
    public void Load_AlreadyLoadedAssembly_ReturnsExistingInstance()
    {
        var context = new ProbeLoadContext([]);
        var coreLibName = typeof(object).Assembly.GetName();

        var loaded = context.InvokeLoad(coreLibName);

        Assert.NotNull(loaded);
        Assert.Equal(coreLibName.Name, loaded!.GetName().Name);
    }
}
