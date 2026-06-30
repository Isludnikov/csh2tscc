namespace tests.TestSupport;

/// <summary>
/// Creates a unique temporary directory for tests that write generated files to disk and removes
/// it recursively on <see cref="Dispose"/>. Keeps file IO out of the repository tree and makes
/// such tests hermetic and parallel-safe (the old <c>./generated</c> directory was shared and
/// left behind).
/// </summary>
public sealed class TempOutputDirectory : IDisposable
{
    public string Path { get; }

    public TempOutputDirectory()
    {
        Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "csh2tscc-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(Path))
            {
                Directory.Delete(Path, recursive: true);
            }
        }
        catch (IOException) { /* best-effort cleanup */ }
        catch (UnauthorizedAccessException) { /* best-effort cleanup */ }
    }
}
