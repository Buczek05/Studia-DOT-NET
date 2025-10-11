namespace TextAnalytics.Core;

public sealed record TextStatistics(
    int CharactersWithSpaces,
    int CharactersWithoutSpaces,
    int Letters,
    int Digits,
    int Punctuation,
    int WordCount,
    int UniqueWordCount,
    string MostCommonWord,
    double AverageWordLength,
    string LongestWord,
    string ShortestWord,
    int SentenceCount,
    double AverageWordsPerSentence,
    string LongestSentence
);

public sealed class TextAnalyzer
{
    private static readonly char[] SentenceDelimiters = { '.', '!', '?' };
    private static readonly char[] WordDelimiters = { ' ', '\t', '\n', '\r', '.', ',', ';', ':', '!', '?', '-', '—', '(', ')', '[', ']', '{', '}', '"', '\'', '/', '\\' };

    public TextStatistics Analyze(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return new TextStatistics(
                CharactersWithSpaces: 0,
                CharactersWithoutSpaces: 0,
                Letters: 0,
                Digits: 0,
                Punctuation: 0,
                WordCount: 0,
                UniqueWordCount: 0,
                MostCommonWord: string.Empty,
                AverageWordLength: 0,
                LongestWord: string.Empty,
                ShortestWord: string.Empty,
                SentenceCount: 0,
                AverageWordsPerSentence: 0,
                LongestSentence: string.Empty
            );
        }

        var charactersWithSpaces = CountCharacters(text, includeSpaces: true);
        var charactersWithoutSpaces = CountCharacters(text, includeSpaces: false);
        var letters = CountLetters(text);
        var digits = CountDigits(text);
        var punctuation = CountPunctuation(text);

        var words = GetWords(text);
        var wordCount = words.Count;
        var uniqueWordCount = CountUniqueWords(words);
        var mostCommonWord = GetMostCommonWord(words);
        var averageWordLength = CalculateAverageWordLength(words);
        var longestWord = GetLongestWord(words);
        var shortestWord = GetShortestWord(words);

        var sentences = GetSentences(text);
        var sentenceCount = sentences.Count;
        var averageWordsPerSentence = CalculateAverageWordsPerSentence(sentences);
        var longestSentence = GetLongestSentence(sentences);

        return new TextStatistics(
            CharactersWithSpaces: charactersWithSpaces,
            CharactersWithoutSpaces: charactersWithoutSpaces,
            Letters: letters,
            Digits: digits,
            Punctuation: punctuation,
            WordCount: wordCount,
            UniqueWordCount: uniqueWordCount,
            MostCommonWord: mostCommonWord,
            AverageWordLength: averageWordLength,
            LongestWord: longestWord,
            ShortestWord: shortestWord,
            SentenceCount: sentenceCount,
            AverageWordsPerSentence: averageWordsPerSentence,
            LongestSentence: longestSentence
        );
    }

    public int CountCharacters(string text, bool includeSpaces = true)
    {
        if (string.IsNullOrEmpty(text))
            return 0;

        return includeSpaces ? text.Length : text.Count(c => !char.IsWhiteSpace(c));
    }

    public int CountWords(string text)
    {
        return GetWords(text).Count;
    }

    public int CountLetters(string text)
    {
        if (string.IsNullOrEmpty(text))
            return 0;

        return text.Count(char.IsLetter);
    }

    public int CountDigits(string text)
    {
        if (string.IsNullOrEmpty(text))
            return 0;

        return text.Count(char.IsDigit);
    }

    public int CountPunctuation(string text)
    {
        if (string.IsNullOrEmpty(text))
            return 0;

        return text.Count(char.IsPunctuation);
    }

    public int CountUniqueWords(string text)
    {
        return CountUniqueWords(GetWords(text));
    }

    public string GetMostCommonWord(string text)
    {
        return GetMostCommonWord(GetWords(text));
    }

    public double CalculateAverageWordLength(string text)
    {
        return CalculateAverageWordLength(GetWords(text));
    }

    public string GetLongestWord(string text)
    {
        return GetLongestWord(GetWords(text));
    }

    public string GetShortestWord(string text)
    {
        return GetShortestWord(GetWords(text));
    }

    public int CountSentences(string text)
    {
        return GetSentences(text).Count;
    }

    public double CalculateAverageWordsPerSentence(string text)
    {
        return CalculateAverageWordsPerSentence(GetSentences(text));
    }

    public string GetLongestSentence(string text)
    {
        return GetLongestSentence(GetSentences(text));
    }

    private List<string> GetWords(string text)
    {
        if (string.IsNullOrEmpty(text))
            return new List<string>();

        return text.Split(WordDelimiters, StringSplitOptions.RemoveEmptyEntries)
                   .Where(word => !string.IsNullOrWhiteSpace(word))
                   .ToList();
    }

    private int CountUniqueWords(List<string> words)
    {
        if (words.Count == 0)
            return 0;

        return words.Select(w => w.ToLowerInvariant()).Distinct().Count();
    }

    private string GetMostCommonWord(List<string> words)
    {
        if (words.Count == 0)
            return string.Empty;

        return words.GroupBy(w => w.ToLowerInvariant())
                    .OrderByDescending(g => g.Count())
                    .First()
                    .Key;
    }

    private double CalculateAverageWordLength(List<string> words)
    {
        if (words.Count == 0)
            return 0;

        return words.Average(w => w.Length);
    }

    private string GetLongestWord(List<string> words)
    {
        if (words.Count == 0)
            return string.Empty;

        return words.OrderByDescending(w => w.Length).First();
    }

    private string GetShortestWord(List<string> words)
    {
        if (words.Count == 0)
            return string.Empty;

        return words.OrderBy(w => w.Length).First();
    }

    private List<string> GetSentences(string text)
    {
        if (string.IsNullOrEmpty(text))
            return new List<string>();

        var sentences = new List<string>();
        var currentSentence = string.Empty;

        foreach (var ch in text)
        {
            currentSentence += ch;

            if (SentenceDelimiters.Contains(ch))
            {
                var trimmed = currentSentence.Trim();
                if (!string.IsNullOrWhiteSpace(trimmed))
                {
                    sentences.Add(trimmed);
                }
                currentSentence = string.Empty;
            }
        }

        var lastSentence = currentSentence.Trim();
        if (!string.IsNullOrWhiteSpace(lastSentence))
        {
            sentences.Add(lastSentence);
        }

        return sentences;
    }

    private double CalculateAverageWordsPerSentence(List<string> sentences)
    {
        if (sentences.Count == 0)
            return 0;

        var totalWords = sentences.Sum(s => GetWords(s).Count);
        return (double)totalWords / sentences.Count;
    }

    private string GetLongestSentence(List<string> sentences)
    {
        if (sentences.Count == 0)
            return string.Empty;

        return sentences.OrderByDescending(s => GetWords(s).Count).First();
    }
}