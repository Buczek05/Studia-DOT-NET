using LibraryApp.Domain;
using Xunit;

namespace LibraryApp.Tests;

public class LibraryItemTests
{
    private class TestLibraryItem : LibraryItem
    {
        public TestLibraryItem(int id, string title) : base(id, title) { }

        public override void DisplayInfo()
        {
            Console.WriteLine($"Test Item: {Title}");
        }
    }

    [Fact]
    public void LibraryItem_Constructor_ShouldSetIdAndTitle()
    {
        var item = new TestLibraryItem(1, "Test Title");

        Assert.Equal(1, item.Id);
        Assert.Equal("Test Title", item.Title);
        Assert.True(item.IsAvailable);
    }

    [Fact]
    public void LibraryItem_Constructor_ShouldThrowArgumentNullException_WhenTitleIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new TestLibraryItem(1, null));
    }

    [Fact]
    public void LibraryItem_IsAvailable_ShouldDefaultToTrue()
    {
        var item = new TestLibraryItem(1, "Test Title");

        Assert.True(item.IsAvailable);
    }
}

public class BookTests
{
    [Fact]
    public void Book_Constructor_ShouldSetAllProperties()
    {
        var book = new Book(1, "Clean Code", "Robert C. Martin", "978-0132350884");

        Assert.Equal(1, book.Id);
        Assert.Equal("Clean Code", book.Title);
        Assert.Equal("Robert C. Martin", book.Author);
        Assert.Equal("978-0132350884", book.Isbn);
        Assert.True(book.IsAvailable);
    }

    [Fact]
    public void Book_Constructor_ShouldThrowArgumentNullException_WhenTitleIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new Book(1, null, "Robert C. Martin", "978-0132350884"));
    }

    [Fact]
    public void Book_Constructor_ShouldThrowArgumentNullException_WhenAuthorIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new Book(1, "Clean Code", null, "978-0132350884"));
    }

    [Fact]
    public void Book_Constructor_ShouldThrowArgumentNullException_WhenIsbnIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new Book(1, "Clean Code", "Robert C. Martin", null));
    }

    [Fact]
    public void Book_DisplayInfo_ShouldNotThrow()
    {
        var book = new Book(1, "Clean Code", "Robert C. Martin", "978-0132350884");

        var exception = Record.Exception(() => book.DisplayInfo());

        Assert.Null(exception);
    }

    [Fact]
    public void Book_InheritsFromLibraryItem()
    {
        var book = new Book(1, "Test", "Author", "ISBN");

        Assert.IsAssignableFrom<LibraryItem>(book);
    }

    [Fact]
    public void Book_DisplayInfo_ShowsPolymorphism()
    {
        LibraryItem item = new Book(1, "Test Book", "Test Author", "123-456");

        var exception = Record.Exception(() => item.DisplayInfo());

        Assert.Null(exception);
    }
}