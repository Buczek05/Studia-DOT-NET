using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using TextAnalytics.Core;
using TextAnalytics.Services;

Console.WriteLine("╔════════════════════════════════════════════╗");
Console.WriteLine("║   TEXT ANALYTICS APPLICATION v1.0         ║");
Console.WriteLine("╚════════════════════════════════════════════╝\n");

string? filePath = null;
bool useFileInput = false;

if (args.Length > 0)
{
    filePath = args[0];
    useFileInput = true;
    Console.WriteLine($"[CLI Mode] Using file path from argument: {filePath}\n");
}
else
{
    Console.WriteLine("Select input source:");
    Console.WriteLine("1. Enter text from keyboard");
    Console.WriteLine("2. Load text from file");
    Console.Write("\nYour choice (1 or 2): ");

    var choice = Console.ReadLine();

    if (choice == "2")
    {
        Console.Write("Enter file path: ");
        filePath = Console.ReadLine();
        useFileInput = true;
    }
    Console.WriteLine();
}

var services = new ServiceCollection();

services.AddSingleton<ILoggerService, ConsoleLogger>();
services.AddSingleton<TextAnalyzer>();

if (useFileInput && !string.IsNullOrWhiteSpace(filePath))
{
    if (!File.Exists(filePath))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ERROR] File not found: {filePath}");
        Console.ResetColor();
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
        return;
    }

    services.AddSingleton<IInputProvider>(sp =>
        new FileInputProvider(sp.GetRequiredService<ILoggerService>(), filePath));
}
else
{
    services.AddSingleton<IInputProvider, ConsoleInputProvider>();
}

var serviceProvider = services.BuildServiceProvider();

var logger = serviceProvider.GetRequiredService<ILoggerService>();
var inputProvider = serviceProvider.GetRequiredService<IInputProvider>();
var analyzer = serviceProvider.GetRequiredService<TextAnalyzer>();

try
{
    logger.LogInfo("Aplikacja uruchomiona.");

    var text = inputProvider.GetInput();

    if (string.IsNullOrWhiteSpace(text))
    {
        logger.LogWarning("Brak tekstu do analizy. Zamykanie aplikacji.");
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
        return;
    }

    logger.LogInfo("Analizowanie tekstu...");
    var stats = analyzer.Analyze(text);
    logger.LogSuccess("Analiza zakończona pomyślnie!");

    Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
    Console.WriteLine("║              WYNIKI ANALIZY TEKSTU                         ║");
    Console.WriteLine("╚════════════════════════════════════════════════════════════╝");

    Console.WriteLine("\n┌─── STATYSTYKI ZNAKÓW ───────────────────────────────────────┐");
    Console.WriteLine($"│ Znaki (ze spacjami):         {stats.CharactersWithSpaces,30} │");
    Console.WriteLine($"│ Znaki (bez spacji):          {stats.CharactersWithoutSpaces,30} │");
    Console.WriteLine($"│ Litery:                      {stats.Letters,30} │");
    Console.WriteLine($"│ Cyfry:                       {stats.Digits,30} │");
    Console.WriteLine($"│ Znaki interpunkcyjne:        {stats.Punctuation,30} │");
    Console.WriteLine("└─────────────────────────────────────────────────────────────┘");

    Console.WriteLine("\n┌─── STATYSTYKI SŁÓW ─────────────────────────────────────────┐");
    Console.WriteLine($"│ Liczba słów:                 {stats.WordCount,30} │");
    Console.WriteLine($"│ Unikalne słowa:              {stats.UniqueWordCount,30} │");
    Console.WriteLine($"│ Najczęstsze słowo:           {stats.MostCommonWord,30} │");
    Console.WriteLine($"│ Średnia długość słowa:       {stats.AverageWordLength,30:F2} │");
    Console.WriteLine($"│ Najdłuższe słowo:            {stats.LongestWord,30} │");
    Console.WriteLine($"│ Najkrótsze słowo:            {stats.ShortestWord,30} │");
    Console.WriteLine("└─────────────────────────────────────────────────────────────┘");

    Console.WriteLine("\n┌─── STATYSTYKI ZDAŃ ─────────────────────────────────────────┐");
    Console.WriteLine($"│ Liczba zdań:                 {stats.SentenceCount,30} │");
    Console.WriteLine($"│ Śr. słów na zdanie:          {stats.AverageWordsPerSentence,30:F2} │");
    Console.WriteLine($"│ Najdłuższe zdanie:                                          │");

    var longestSentence = stats.LongestSentence;
    if (longestSentence.Length > 56)
    {
        Console.WriteLine($"│   {longestSentence.Substring(0, 56)}  │");
        int offset = 56;
        while (offset < longestSentence.Length)
        {
            int length = Math.Min(56, longestSentence.Length - offset);
            Console.WriteLine($"│   {longestSentence.Substring(offset, length).PadRight(56)}  │");
            offset += 56;
        }
    }
    else
    {
        Console.WriteLine($"│   {longestSentence.PadRight(56)} │");
    }
    Console.WriteLine("└─────────────────────────────────────────────────────────────┘");

    // Export to JSON
    logger.LogInfo("Zapisywanie wyników do pliku JSON...");

    var json = JsonConvert.SerializeObject(stats, Formatting.Indented);
    var outputPath = "results.json";
    File.WriteAllText(outputPath, json);

    logger.LogSuccess($"Wyniki zapisane do {outputPath}");

    Console.WriteLine($"\n✓ Pełne wyniki zostały zapisane w pliku: {outputPath}");
    Console.WriteLine("\nNaciśnij dowolny klawisz aby zakończyć...");
    Console.ReadKey();
}
catch (Exception ex)
{
    logger.LogError($"Wystąpił błąd: {ex.Message}");
    Console.WriteLine($"\nSzczegóły błędu: {ex}");
    Console.WriteLine("\nNaciśnij dowolny klawisz aby zakończyć...");
    Console.ReadKey();
}