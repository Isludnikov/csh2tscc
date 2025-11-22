using csh2tscc;

namespace TypeConverter.Executor;

internal static class Executor
{
    public static void Execute(TypesGeneratorParameters parameters)
    {
        var generator = TypesGenerator.Create(parameters);
        Directory.CreateDirectory(generator.Config.OutputDirectory);
        var tsClasses = generator.TransformTypes();
        if (generator.Config.CleanOutputDirectory)
        {
            foreach (var file in new DirectoryInfo(generator.Config.OutputDirectory).EnumerateFiles($"*{generator.Config.FileExtension}"))
            {
                file.Delete();
            }
        }
        var filesCount = 0;
        foreach (var tsClass in tsClasses)
        {
            File.WriteAllText($"{generator.Config.OutputDirectory}/{tsClass.Key}", tsClass.Value);
            ++filesCount;
            Console.WriteLine($"File [{tsClass.Key}] created");
        }

        Console.WriteLine($"[{filesCount}] file(s) generated");
    }
}