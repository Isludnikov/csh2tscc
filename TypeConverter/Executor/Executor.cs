using csh2tscc;

namespace TypeConverter.Executor;

internal static class Executor
{
    public static void Execute(TypesGeneratorParameters parameters)
    {
        var generator = TypesGenerator.Create(parameters);
        var config = generator.Config;

        // Generate TypeScript definitions in memory
        var generatedFiles = generator.TransformTypes();

        // Prepare output directory
        Directory.CreateDirectory(config.OutputDirectory);
        if (config.CleanOutputDirectory)
        {
            CleanOutputDirectory(config.OutputDirectory, config.FileExtension);
        }

        // Write generated files to disk
        WriteGeneratedFiles(config.OutputDirectory, generatedFiles);
    }

    private static void CleanOutputDirectory(string outputDirectory, string fileExtension)
    {
        var directory = new DirectoryInfo(outputDirectory);
        foreach (var file in directory.EnumerateFiles($"*{fileExtension}"))
        {
            file.Delete();
        }
    }

    private static void WriteGeneratedFiles(string outputDirectory, Dictionary<string, string> files)
    {
        foreach (var (fileName, content) in files)
        {
            var filePath = Path.Combine(outputDirectory, fileName);
            File.WriteAllText(filePath, content);
            Console.WriteLine($"File [{fileName}] created");
        }

        Console.WriteLine($"[{files.Count}] file(s) generated");
    }
}