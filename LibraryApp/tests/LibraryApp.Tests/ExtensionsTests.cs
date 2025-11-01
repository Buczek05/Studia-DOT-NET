using LibraryApp.Domain;
using LibraryApp.Extensions;
using Xunit;

namespace LibraryApp.Tests;

public class ExtensionsTests
{
    [Fact]
    public void Available_ShouldReturnOnlyAvailableItems()
    {
        var book1 = new Book(1, "Available Book", "Author", "ISBN1");
        var book2 = new Book(2, "Unavailable Book", "Author", "ISBN2");
        var book3 = new Book(3, "Another Available", "Author", "ISBN3");

        book2.SetAvailability(false);

        var items = new List<Book> { book1, book2, book3 };

        var availableItems = items.Available().ToList();

        Assert.Equal(2, availableItems.Count);
        Assert.Contains(book1, availableItems);
        Assert.Contains(book3, availableItems);
        Assert.DoesNotContain(book2, availableItems);
    }

    [Fact]
    public void Available_ShouldReturnEmptyList_WhenNoAvailableItems()
    {
        var book1 = new Book(1, "Book 1", "Author", "ISBN1");
        var book2 = new Book(2, "Book 2", "Author", "ISBN2");

        book1.SetAvailability(false);
        book2.SetAvailability(false);

        var items = new List<Book> { book1, book2 };

        var availableItems = items.Available().ToList();

        Assert.Empty(availableItems);
    }

    [Fact]
    public void Available_ShouldReturnAllItems_WhenAllAvailable()
    {
        var book1 = new Book(1, "Book 1", "Author", "ISBN1");
        var book2 = new Book(2, "Book 2", "Author", "ISBN2");

        var items = new List<Book> { book1, book2 };

        var availableItems = items.Available().ToList();

        Assert.Equal(2, availableItems.Count);
    }

    [Fact]
    public void Available_ShouldWorkWithEBooks()
    {
        var ebook1 = new EBook(1, "Available EBook", "Author", "ISBN1", "PDF");
        var ebook2 = new EBook(2, "Unavailable EBook", "Author", "ISBN2", "EPUB");

        ebook2.SetAvailability(false);

        var items = new List<EBook> { ebook1, ebook2 };

        var availableItems = items.Available().ToList();

        Assert.Single(availableItems);
        Assert.Contains(ebook1, availableItems);
    }

    [Fact]
    public void Newest_ShouldReturnItemsInDescendingOrderById()
    {
        var book1 = new Book(1, "Oldest", "Author", "ISBN1");
        var book2 = new Book(2, "Middle", "Author", "ISBN2");
        var book3 = new Book(3, "Newest", "Author", "ISBN3");

        var items = new List<LibraryItem> { book1, book2, book3 };

        var newestItems = items.Newest(3).ToList();

        Assert.Equal(book3, newestItems[0]);
        Assert.Equal(book2, newestItems[1]);
        Assert.Equal(book1, newestItems[2]);
    }

    [Fact]
    public void Newest_ShouldReturnOnlyRequestedNumber()
    {
        var book1 = new Book(1, "Book 1", "Author", "ISBN1");
        var book2 = new Book(2, "Book 2", "Author", "ISBN2");
        var book3 = new Book(3, "Book 3", "Author", "ISBN3");
        var book4 = new Book(4, "Book 4", "Author", "ISBN4");

        var items = new List<LibraryItem> { book1, book2, book3, book4 };

        var newestItems = items.Newest(2).ToList();

        Assert.Equal(2, newestItems.Count);
        Assert.Equal(book4, newestItems[0]);
        Assert.Equal(book3, newestItems[1]);
    }

    [Fact]
    public void Newest_ShouldReturnAllItems_WhenTakeIsGreaterThanCount()
    {
        var book1 = new Book(1, "Book 1", "Author", "ISBN1");
        var book2 = new Book(2, "Book 2", "Author", "ISBN2");

        var items = new List<LibraryItem> { book1, book2 };

        var newestItems = items.Newest(10).ToList();

        Assert.Equal(2, newestItems.Count);
    }

    [Fact]
    public void Newest_ShouldReturnEmpty_WhenTakeIsZero()
    {
        var book1 = new Book(1, "Book 1", "Author", "ISBN1");

        var items = new List<LibraryItem> { book1 };

        var newestItems = items.Newest(0).ToList();

        Assert.Empty(newestItems);
    }

    [Fact]
    public void ByTitle_ShouldReturnMatchingItems()
    {
        var book1 = new Book(1, "The Great Gatsby", "Author", "ISBN1");
        var book2 = new Book(2, "Great Expectations", "Author", "ISBN2");
        var book3 = new Book(3, "To Kill a Mockingbird", "Author", "ISBN3");

        var items = new List<LibraryItem> { book1, book2, book3 };

        var results = items.ByTitle("Great").ToList();

        Assert.Equal(2, results.Count);
        Assert.Contains(book1, results);
        Assert.Contains(book2, results);
        Assert.DoesNotContain(book3, results);
    }

    [Fact]
    public void ByTitle_ShouldBeCaseInsensitive()
    {
        var book1 = new Book(1, "The Great Gatsby", "Author", "ISBN1");

        var items = new List<LibraryItem> { book1 };

        var results1 = items.ByTitle("great").ToList();
        var results2 = items.ByTitle("GREAT").ToList();
        var results3 = items.ByTitle("Great").ToList();

        Assert.Single(results1);
        Assert.Single(results2);
        Assert.Single(results3);
    }

    [Fact]
    public void ByTitle_ShouldReturnAllItems_WhenFragmentIsEmpty()
    {
        var book1 = new Book(1, "Book 1", "Author", "ISBN1");
        var book2 = new Book(2, "Book 2", "Author", "ISBN2");

        var items = new List<LibraryItem> { book1, book2 };

        var results = items.ByTitle("").ToList();

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void ByTitle_ShouldReturnAllItems_WhenFragmentIsNull()
    {
        var book1 = new Book(1, "Book 1", "Author", "ISBN1");
        var book2 = new Book(2, "Book 2", "Author", "ISBN2");

        var items = new List<LibraryItem> { book1, book2 };

        var results = items.ByTitle(null).ToList();

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void ByAuthor_ShouldReturnMatchingBooks()
    {
        var book1 = new Book(1, "Book 1", "John Smith", "ISBN1");
        var book2 = new Book(2, "Book 2", "Jane Smith", "ISBN2");
        var book3 = new Book(3, "Book 3", "Robert Johnson", "ISBN3");

        var items = new List<LibraryItem> { book1, book2, book3 };

        var results = items.ByAuthor("Smith").ToList();

        Assert.Equal(2, results.Count);
        Assert.Contains(book1, results);
        Assert.Contains(book2, results);
        Assert.DoesNotContain(book3, results);
    }

    [Fact]
    public void ByAuthor_ShouldBeCaseInsensitive()
    {
        var book = new Book(1, "Book", "John Smith", "ISBN");

        var items = new List<LibraryItem> { book };

        var results1 = items.ByAuthor("smith").ToList();
        var results2 = items.ByAuthor("SMITH").ToList();

        Assert.Single(results1);
        Assert.Single(results2);
    }

    [Fact]
    public void ByAuthor_ShouldReturnEmpty_WhenFragmentIsNull()
    {
        var book = new Book(1, "Book", "Author", "ISBN");

        var items = new List<LibraryItem> { book };

        var results = items.ByAuthor(null).ToList();

        Assert.Empty(results);
    }

    [Fact]
    public void ByAuthor_ShouldReturnEmpty_WhenFragmentIsEmpty()
    {
        var book = new Book(1, "Book", "Author", "ISBN");

        var items = new List<LibraryItem> { book };

        var results = items.ByAuthor("").ToList();

        Assert.Empty(results);
    }

    [Fact]
    public void ByAuthor_ShouldWorkWithEBooks()
    {
        var ebook1 = new EBook(1, "EBook 1", "John Doe", "ISBN1", "PDF");
        var ebook2 = new EBook(2, "EBook 2", "Jane Doe", "ISBN2", "EPUB");

        var items = new List<LibraryItem> { ebook1, ebook2 };

        var results = items.ByAuthor("Doe").ToList();

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void ExtensionMethods_CanBeChained()
    {
        var book1 = new Book(1, "Great Book", "John Smith", "ISBN1");
        var book2 = new Book(2, "Great Novel", "Jane Smith", "ISBN2");
        var book3 = new Book(3, "Amazing Story", "John Smith", "ISBN3");
        var book4 = new Book(4, "Great Story", "Robert Johnson", "ISBN4");

        book3.SetAvailability(false);

        var items = new List<LibraryItem> { book1, book2, book3, book4 };

        // Chain multiple extension methods
        var results = items
            .Available()
            .ByTitle("Great")
            .Newest(2)
            .ToList();

        Assert.Equal(2, results.Count);
        Assert.Contains(book4, results);
        Assert.Contains(book2, results);
    }
}
