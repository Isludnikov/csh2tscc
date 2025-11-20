using System.Reflection;
using System.Runtime.Loader;

namespace csh2tscc;

public class CustomAssemblyLoadContext(IEnumerable<string> basePath) : AssemblyLoadContext(true)
{
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        var loadedAssembly = Default.Assemblies.FirstOrDefault(a => a.GetName().Name == assemblyName.Name);
        if (loadedAssembly != null)
        {
            return loadedAssembly;
        }

        foreach (var basP in basePath)
        {
            var assemblyPath = Path.Combine(basP, assemblyName.Name + ".dll");
            if (File.Exists(assemblyPath))
            {
                return LoadFromAssemblyPath(assemblyPath);
            }
        }

        try
        {
            return Default.LoadFromAssemblyName(assemblyName);
        }
        catch
        {
            return null;
        }
    }

    public Assembly LoadAssembly(string assemblyPath) => LoadFromAssemblyPath(assemblyPath);
}