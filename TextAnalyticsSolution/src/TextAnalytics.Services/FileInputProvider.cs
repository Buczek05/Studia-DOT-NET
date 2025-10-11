namespace TextAnalytics.Services;

public sealed class FileInputProvider : IInputProvider
{
    private readonly ILoggerService _logger;
    private readonly string _filePath;

    public FileInputProvider(ILoggerService logger, string filePath)
    {
        _logger = logger;
        _filePath = filePath;
    }

    public string GetInput()
    {
        try
        {
            _logger.LogInfo($"Reading file: {_filePath}");

            if (!File.Exists(_filePath))
            {
                _logger.LogError($"File not found: {_filePath}");
                return string.Empty;
            }

            var content = File.ReadAllText(_filePath);

            if (string.IsNullOrWhiteSpace(content))
            {
                _logger.LogWarning("File is empty or contains only whitespace");
            }
            else
            {
                _logger.LogSuccess($"File read successfully: {content.Length} characters");
            }

            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error reading file: {ex.Message}");
            return string.Empty;
        }
    }
}