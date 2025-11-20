using csh2tscc;

namespace TypeConverter.Executor;

internal static class Executor
{
    public static void Execute(TypesGeneratorParameters parameters)
    {
        var generator = new TypesGenerator(parameters);
        Directory.CreateDirectory(generator.Config.OutputDirectory);
        var tsClasses = generator.TransformTypes();
        var count = 0;
        if (generator.Config.CleanOutputDirectory)
        {
            foreach (var file in new DirectoryInfo(generator.Config.OutputDirectory).EnumerateFiles($"*{generator.Config.FileExtension}"))
            {
                file.Delete();
            }
        }

        foreach (var tsClass in tsClasses)
        {
            File.WriteAllText($"{generator.Config.OutputDirectory}/{tsClass.Key}", tsClass.Value);
            ++count;
            Console.WriteLine($"File [{tsClass.Key}] created");
        }

        Console.WriteLine($"[{count}] file(s) generated");
    }
}