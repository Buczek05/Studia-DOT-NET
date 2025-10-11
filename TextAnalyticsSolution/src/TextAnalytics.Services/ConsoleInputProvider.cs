namespace TextAnalytics.Services;

public sealed class ConsoleInputProvider : IInputProvider
{
    private readonly ILoggerService _logger;

    public ConsoleInputProvider(ILoggerService logger)
    {
        _logger = logger;
    }

    public string GetInput()
    {
        _logger.LogInfo("Waiting for console input...");
        Console.WriteLine("Enter text to analyze (press Enter when done):");
        Console.Write("> ");

        var input = Console.ReadLine() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(input))
        {
            _logger.LogWarning("Empty input received");
        }
        else
        {
            _logger.LogSuccess($"Input received: {input.Length} characters");
        }

        return input;
    }
}