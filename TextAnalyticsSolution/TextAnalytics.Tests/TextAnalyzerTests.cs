using NUnit.Framework;
using TextAnalytics.Core;

namespace TextAnalytics.Tests;

[TestFixture]
public class TextAnalyzerTests
{
    private TextAnalyzer _analyzer = null!;

    [SetUp]
    public void Setup()
    {
        _analyzer = new TextAnalyzer();
    }

    #region Empty and Whitespace Tests

    [Test]
    public void Analyze_EmptyString_ReturnsZeroStatistics()
    {
        var text = string.Empty;
        var result = _analyzer.Analyze(text);

        Assert.Multiple(() =>
        {
            Assert.That(result.CharactersWithSpaces, Is.EqualTo(0));
            Assert.That(result.CharactersWithoutSpaces, Is.EqualTo(0));
            Assert.That(result.WordCount, Is.EqualTo(0));
            Assert.That(result.UniqueWordCount, Is.EqualTo(0));
            Assert.That(result.SentenceCount, Is.EqualTo(0));
            Assert.That(result.MostCommonWord, Is.EqualTo(string.Empty));
        });
    }

    [Test]
    public void Analyze_OnlyWhitespace_ReturnsZeroStatistics()
    {
        var text = "   \t\n\r   ";
        var result = _analyzer.Analyze(text);

        Assert.Multiple(() =>
        {
            Assert.That(result.CharactersWithSpaces, Is.EqualTo(9));
            Assert.That(result.CharactersWithoutSpaces, Is.EqualTo(0));
            Assert.That(result.WordCount, Is.EqualTo(0));
            Assert.That(result.Letters, Is.EqualTo(0));
        });
    }

    #endregion

    #region Word Counting Tests

    [Test]
    public void CountWords_Returns2_ForHelloWorld()
    {
        var text = "Hello world!";
        var result = _analyzer.CountWords(text);

        Assert.That(result, Is.EqualTo(2));
    }

    [Test]
    public void CountWords_HandlesMultipleSpaces_Correctly()
    {
        var text = "Hello    world!   How   are    you?";
        var result = _analyzer.CountWords(text);

        Assert.That(result, Is.EqualTo(5));
    }

    [Test]
    public void CountWords_HandlesPunctuationCorrectly()
    {
        var text = "Hello, world! How are you? I'm fine.";
        var result = _analyzer.CountWords(text);

        Assert.That(result, Is.EqualTo(8));
    }

    [Test]
    public void CountUniqueWords_ReturnsCorrectCount()
    {
        var text = "hello world hello test world";
        var result = _analyzer.CountUniqueWords(text);

        Assert.That(result, Is.EqualTo(3));
    }

    #endregion

    #region Character Counting Tests

    [Test]
    public void CountCharacters_WithSpaces_ReturnsCorrectCount()
    {
        var text = "Hello world";
        var result = _analyzer.CountCharacters(text, includeSpaces: true);

        Assert.That(result, Is.EqualTo(11));
    }

    [Test]
    public void CountCharacters_WithoutSpaces_ReturnsCorrectCount()
    {
        var text = "Hello world";
        var result = _analyzer.CountCharacters(text, includeSpaces: false);

        Assert.That(result, Is.EqualTo(10));
    }

    [Test]
    public void CountLetters_ReturnsOnlyLetterCount()
    {
        var text = "Hello123 World!";
        var result = _analyzer.CountLetters(text);

        Assert.That(result, Is.EqualTo(10));
    }

    [Test]
    public void CountDigits_ReturnsOnlyDigitCount()
    {
        var text = "Hello123 World456";
        var result = _analyzer.CountDigits(text);

        Assert.That(result, Is.EqualTo(6));
    }

    [Test]
    public void CountPunctuation_ReturnsCorrectCount()
    {
        var text = "Hello, world! How are you?";
        var result = _analyzer.CountPunctuation(text);

        Assert.That(result, Is.EqualTo(3));
    }

    #endregion

    #region Word Analysis Tests

    [Test]
    public void GetMostCommonWord_ReturnsMostFrequentWord()
    {
        var text = "hello world hello test world hello";
        var result = _analyzer.GetMostCommonWord(text);

        Assert.That(result, Is.EqualTo("hello"));
    }

    [Test]
    public void GetMostCommonWord_WithTie_ReturnsFirstOccurrence()
    {
        var text = "hello world hello world";
        var result = _analyzer.GetMostCommonWord(text);

        Assert.That(result, Is.EqualTo("hello").Or.EqualTo("world"));
    }

    [Test]
    public void CalculateAverageWordLength_ReturnsCorrectAverage()
    {
        var text = "I am happy";
        var result = _analyzer.CalculateAverageWordLength(text);

        Assert.That(result, Is.EqualTo(2.67).Within(0.01));
    }

    [Test]
    public void GetLongestWord_ReturnsLongestWord()
    {
        var text = "I am extremely happy today";
        var result = _analyzer.GetLongestWord(text);

        Assert.That(result, Is.EqualTo("extremely"));
    }

    [Test]
    public void GetShortestWord_ReturnsShortestWord()
    {
        var text = "I am extremely happy today";
        var result = _analyzer.GetShortestWord(text);

        Assert.That(result, Is.EqualTo("I"));
    }

    #endregion

    #region Sentence Analysis Tests

    [Test]
    public void CountSentences_ReturnsCorrectCount()
    {
        var text = "Hello world! How are you? I am fine.";
        var result = _analyzer.CountSentences(text);

        Assert.That(result, Is.EqualTo(3));
    }

    [Test]
    public void CountSentences_HandlesMixedDelimiters()
    {
        var text = "Hello! Are you okay? Yes. Great!";
        var result = _analyzer.CountSentences(text);

        Assert.That(result, Is.EqualTo(4));
    }

    [Test]
    public void CalculateAverageWordsPerSentence_ReturnsCorrectAverage()
    {
        var text = "Hello world! How are you?";
        var result = _analyzer.CalculateAverageWordsPerSentence(text);

        Assert.That(result, Is.EqualTo(2.5).Within(0.01));
    }

    [Test]
    public void GetLongestSentence_ReturnsSentenceWithMostWords()
    {
        var text = "Hi! How are you doing today? Good.";
        var result = _analyzer.GetLongestSentence(text);

        Assert.That(result, Is.EqualTo("How are you doing today?"));
    }

    #endregion

    #region Complex Integration Tests

    [Test]
    public void Analyze_CompleteText_ReturnsAllStatistics()
    {
        var text = "Hello world! This is a test. How are you? I am doing great!";
        var result = _analyzer.Analyze(text);

        Assert.Multiple(() =>
        {
            Assert.That(result.WordCount, Is.GreaterThan(0));
            Assert.That(result.UniqueWordCount, Is.GreaterThan(0));
            Assert.That(result.SentenceCount, Is.EqualTo(4));
            Assert.That(result.Letters, Is.GreaterThan(0));
            Assert.That(result.CharactersWithSpaces, Is.GreaterThan(result.CharactersWithoutSpaces));
            Assert.That(result.AverageWordLength, Is.GreaterThan(0));
            Assert.That(result.LongestWord, Is.Not.Empty);
            Assert.That(result.ShortestWord, Is.Not.Empty);
        });
    }

    [Test]
    public void Analyze_TextWithNumbers_HandlesDigitsCorrectly()
    {
        var text = "I have 123 apples and 456 oranges.";
        var result = _analyzer.Analyze(text);

        Assert.Multiple(() =>
        {
            Assert.That(result.Digits, Is.EqualTo(6));
            Assert.That(result.WordCount, Is.EqualTo(7));
        });
    }

    [Test]
    public void Analyze_TextWithExcessivePunctuation_HandlesCorrectly()
    {
        var text = "Hello!!! World??? Are... you... okay???";
        var result = _analyzer.Analyze(text);

        Assert.Multiple(() =>
        {
            Assert.That(result.WordCount, Is.EqualTo(5));
            Assert.That(result.Punctuation, Is.GreaterThan(5));
            Assert.That(result.SentenceCount, Is.GreaterThan(0));
        });
    }

    #endregion
}