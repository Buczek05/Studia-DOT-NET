using LibraryApp.Domain;
using Xunit;

namespace LibraryApp.Tests;

public class EBookTests
{
    [Fact]
    public void EBook_Constructor_ShouldSetAllProperties()
    {
        var ebook = new EBook(1, "Clean Code", "Robert C. Martin", "978-0132350884", "PDF");

        Assert.Equal(1, ebook.Id);
        Assert.Equal("Clean Code", ebook.Title);
        Assert.Equal("Robert C. Martin", ebook.Author);
        Assert.Equal("978-0132350884", ebook.Isbn);
        Assert.Equal("PDF", ebook.FileFormat);
        Assert.True(ebook.IsAvailable);
    }

    [Fact]
    public void EBook_Constructor_ShouldThrowArgumentNullException_WhenFileFormatIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new EBook(1, "Clean Code", "Robert C. Martin", "978-0132350884", null));
    }

    [Fact]
    public void EBook_Constructor_ShouldThrowArgumentNullException_WhenTitleIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new EBook(1, null, "Robert C. Martin", "978-0132350884", "PDF"));
    }

    [Fact]
    public void EBook_Constructor_ShouldThrowArgumentNullException_WhenAuthorIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new EBook(1, "Clean Code", null, "978-0132350884", "PDF"));
    }

    [Fact]
    public void EBook_Constructor_ShouldThrowArgumentNullException_WhenIsbnIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new EBook(1, "Clean Code", "Robert C. Martin", null, "PDF"));
    }

    [Fact]
    public void EBook_InheritsFromBook()
    {
        var ebook = new EBook(1, "Test", "Author", "ISBN", "EPUB");

        Assert.IsAssignableFrom<Book>(ebook);
    }

    [Fact]
    public void EBook_InheritsFromLibraryItem()
    {
        var ebook = new EBook(1, "Test", "Author", "ISBN", "EPUB");

        Assert.IsAssignableFrom<LibraryItem>(ebook);
    }

    [Fact]
    public void EBook_DisplayInfo_ShouldNotThrow()
    {
        var ebook = new EBook(1, "Clean Code", "Robert C. Martin", "978-0132350884", "PDF");

        var exception = Record.Exception(() => ebook.DisplayInfo());

        Assert.Null(exception);
    }

    [Fact]
    public void EBook_SupportsPolymorphism_AsBook()
    {
        Book book = new EBook(1, "Test EBook", "Test Author", "123-456", "EPUB");

        var exception = Record.Exception(() => book.DisplayInfo());

        Assert.Null(exception);
        Assert.Equal("Test EBook", book.Title);
        Assert.Equal("Test Author", book.Author);
    }

    [Fact]
    public void EBook_SupportsPolymorphism_AsLibraryItem()
    {
        LibraryItem item = new EBook(1, "Test EBook", "Test Author", "123-456", "MOBI");

        var exception = Record.Exception(() => item.DisplayInfo());

        Assert.Null(exception);
        Assert.Equal("Test EBook", item.Title);
    }

    [Fact]
    public void EBook_OverridesDisplayInfo_IncludesFileFormat()
    {
        var ebook = new EBook(1, "Programming in C#", "Author Name", "111-222", "PDF");

        var originalOut = Console.Out;
        try
        {
            using var sw = new StringWriter();
            Console.SetOut(sw);

            ebook.DisplayInfo();

            var output = sw.ToString();
            Assert.Contains("PDF", output);
            Assert.Contains("EBook", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
