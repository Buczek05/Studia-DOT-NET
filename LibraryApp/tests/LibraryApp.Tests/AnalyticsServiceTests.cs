using LibraryApp.Domain;
using LibraryApp.Services;
using Xunit;

namespace LibraryApp.Tests;

public class AnalyticsServiceTests
{
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenLibraryServiceIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new AnalyticsService(null));
    }

    [Fact]
    public void AverageLoanLengthDays_ShouldReturnZero_WhenNoReservations()
    {
        var library = new LibraryService();
        var analytics = new AnalyticsService(library);

        var average = analytics.AverageLoanLengthDays();

        Assert.Equal(0, average);
    }

    [Fact]
    public void AverageLoanLengthDays_ShouldCalculateCorrectAverage()
    {
        var library = new LibraryService();
        var analytics = new AnalyticsService(library);

        var book1 = new Book(1, "Book 1", "Author", "ISBN1");
        var book2 = new Book(2, "Book 2", "Author", "ISBN2");
        var email = "test@example.com";

        library.AddItem(book1);
        library.AddItem(book2);
        library.RegisterUser(email);

        // First reservation: 7 days
        library.CreateReservation(book1.Id, email, new DateTime(2024, 1, 1), new DateTime(2024, 1, 8));

        book2.SetAvailability(true);
        // Second reservation: 14 days
        library.CreateReservation(book2.Id, email, new DateTime(2024, 1, 1), new DateTime(2024, 1, 15));

        var average = analytics.AverageLoanLengthDays();

        Assert.Equal(10.5, average); // (7 + 14) / 2 = 10.5
    }

    [Fact]
    public void TotalLoans_ShouldReturnZero_WhenNoReservations()
    {
        var library = new LibraryService();
        var analytics = new AnalyticsService(library);

        var total = analytics.TotalLoans();

        Assert.Equal(0, total);
    }

    [Fact]
    public void TotalLoans_ShouldReturnCorrectCount()
    {
        var library = new LibraryService();
        var analytics = new AnalyticsService(library);

        var book1 = new Book(1, "Book 1", "Author", "ISBN1");
        var book2 = new Book(2, "Book 2", "Author", "ISBN2");
        var email = "test@example.com";

        library.AddItem(book1);
        library.AddItem(book2);
        library.RegisterUser(email);

        library.CreateReservation(book1.Id, email, DateTime.Now, DateTime.Now.AddDays(7));
        book2.SetAvailability(true);
        library.CreateReservation(book2.Id, email, DateTime.Now, DateTime.Now.AddDays(7));

        var total = analytics.TotalLoans();

        Assert.Equal(2, total);
    }

    [Fact]
    public void TotalLoans_ShouldIncludeCancelledReservations()
    {
        var library = new LibraryService();
        var analytics = new AnalyticsService(library);

        var book = new Book(1, "Book 1", "Author", "ISBN1");
        var email = "test@example.com";

        library.AddItem(book);
        library.RegisterUser(email);

        var reservation = library.CreateReservation(book.Id, email, DateTime.Now, DateTime.Now.AddDays(7));
        library.CancelReservation(reservation.Id);

        var total = analytics.TotalLoans();

        Assert.Equal(1, total);
    }

    [Fact]
    public void MostPopularItemTitle_ShouldReturnNA_WhenNoReservations()
    {
        var library = new LibraryService();
        var analytics = new AnalyticsService(library);

        var mostPopular = analytics.MostPopularItemTitle();

        Assert.Equal("N/A", mostPopular);
    }

    [Fact]
    public void MostPopularItemTitle_ShouldReturnCorrectTitle()
    {
        var library = new LibraryService();
        var analytics = new AnalyticsService(library);

        var book1 = new Book(1, "Popular Book", "Author", "ISBN1");
        var book2 = new Book(2, "Less Popular Book", "Author", "ISBN2");
        var email = "test@example.com";

        library.AddItem(book1);
        library.AddItem(book2);
        library.RegisterUser(email);

        // Reserve book1 twice
        library.CreateReservation(book1.Id, email, new DateTime(2024, 1, 1), new DateTime(2024, 1, 8));
        library.CancelReservation(1);
        book1.SetAvailability(true);
        library.CreateReservation(book1.Id, email, new DateTime(2024, 2, 1), new DateTime(2024, 2, 8));

        // Reserve book2 once
        book2.SetAvailability(true);
        library.CreateReservation(book2.Id, email, new DateTime(2024, 1, 1), new DateTime(2024, 1, 8));

        var mostPopular = analytics.MostPopularItemTitle();

        Assert.Equal("Popular Book", mostPopular);
    }

    [Fact]
    public void MostPopularItemTitle_ShouldHandleTie()
    {
        var library = new LibraryService();
        var analytics = new AnalyticsService(library);

        var book1 = new Book(1, "Book A", "Author", "ISBN1");
        var book2 = new Book(2, "Book B", "Author", "ISBN2");
        var email = "test@example.com";

        library.AddItem(book1);
        library.AddItem(book2);
        library.RegisterUser(email);

        // Reserve both books once
        library.CreateReservation(book1.Id, email, DateTime.Now, DateTime.Now.AddDays(7));
        book2.SetAvailability(true);
        library.CreateReservation(book2.Id, email, DateTime.Now, DateTime.Now.AddDays(7));

        var mostPopular = analytics.MostPopularItemTitle();

        // Should return one of them (implementation returns first in group)
        Assert.True(mostPopular == "Book A" || mostPopular == "Book B");
    }

    [Fact]
    public void FulfillmentRate_ShouldReturnZero_WhenNoReservations()
    {
        var library = new LibraryService();
        var analytics = new AnalyticsService(library);

        var rate = analytics.FulfillmentRate();

        Assert.Equal(0, rate);
    }

    [Fact]
    public void FulfillmentRate_ShouldReturn100_WhenAllReservationsActive()
    {
        var library = new LibraryService();
        var analytics = new AnalyticsService(library);

        var book1 = new Book(1, "Book 1", "Author", "ISBN1");
        var book2 = new Book(2, "Book 2", "Author", "ISBN2");
        var email = "test@example.com";

        library.AddItem(book1);
        library.AddItem(book2);
        library.RegisterUser(email);

        library.CreateReservation(book1.Id, email, DateTime.Now, DateTime.Now.AddDays(7));
        book2.SetAvailability(true);
        library.CreateReservation(book2.Id, email, DateTime.Now, DateTime.Now.AddDays(7));

        var rate = analytics.FulfillmentRate();

        Assert.Equal(100, rate);
    }

    [Fact]
    public void FulfillmentRate_ShouldCalculateCorrectPercentage()
    {
        var library = new LibraryService();
        var analytics = new AnalyticsService(library);

        var book1 = new Book(1, "Book 1", "Author", "ISBN1");
        var book2 = new Book(2, "Book 2", "Author", "ISBN2");
        var book3 = new Book(3, "Book 3", "Author", "ISBN3");
        var book4 = new Book(4, "Book 4", "Author", "ISBN4");
        var email = "test@example.com";

        library.AddItem(book1);
        library.AddItem(book2);
        library.AddItem(book3);
        library.AddItem(book4);
        library.RegisterUser(email);

        var res1 = library.CreateReservation(book1.Id, email, DateTime.Now, DateTime.Now.AddDays(7));
        book2.SetAvailability(true);
        library.CreateReservation(book2.Id, email, DateTime.Now, DateTime.Now.AddDays(7));
        book3.SetAvailability(true);
        var res3 = library.CreateReservation(book3.Id, email, DateTime.Now, DateTime.Now.AddDays(7));
        book4.SetAvailability(true);
        var res4 = library.CreateReservation(book4.Id, email, DateTime.Now, DateTime.Now.AddDays(7));

        // Cancel 2 out of 4 reservations
        library.CancelReservation(res1.Id);
        library.CancelReservation(res3.Id);

        var rate = analytics.FulfillmentRate();

        Assert.Equal(50, rate); // 2 active out of 4 total = 50%
    }

    [Fact]
    public void LogPopularityScore_ShouldReturnZero_WhenNoReservationsForTitle()
    {
        var library = new LibraryService();
        var analytics = new AnalyticsService(library);

        var score = analytics.LogPopularityScore("Nonexistent Book");

        Assert.Equal(0, score);
    }

    [Fact]
    public void LogPopularityScore_ShouldCalculateCorrectScore()
    {
        var library = new LibraryService();
        var analytics = new AnalyticsService(library);

        var book = new Book(1, "Test Book", "Author", "ISBN");
        var email = "test@example.com";

        library.AddItem(book);
        library.RegisterUser(email);

        library.CreateReservation(book.Id, email, new DateTime(2024, 1, 1), new DateTime(2024, 1, 8));
        library.CancelReservation(1);
        book.SetAvailability(true);
        library.CreateReservation(book.Id, email, new DateTime(2024, 2, 1), new DateTime(2024, 2, 8));

        var score = analytics.LogPopularityScore("Test Book");

        // log(2 + 1) = log(3) ≈ 1.0986
        Assert.True(score > 1.0 && score < 1.1);
    }

    [Fact]
    public void LogPopularityScore_ShouldThrowArgumentException_WhenTitleIsNull()
    {
        var library = new LibraryService();
        var analytics = new AnalyticsService(library);

        Assert.Throws<ArgumentException>(() => analytics.LogPopularityScore(null));
    }

    [Fact]
    public void LogPopularityScore_ShouldThrowArgumentException_WhenTitleIsEmpty()
    {
        var library = new LibraryService();
        var analytics = new AnalyticsService(library);

        Assert.Throws<ArgumentException>(() => analytics.LogPopularityScore(""));
    }

    [Fact]
    public void LogPopularityScore_ShouldBeCaseInsensitive()
    {
        var library = new LibraryService();
        var analytics = new AnalyticsService(library);

        var book = new Book(1, "Test Book", "Author", "ISBN");
        var email = "test@example.com";

        library.AddItem(book);
        library.RegisterUser(email);

        library.CreateReservation(book.Id, email, DateTime.Now, DateTime.Now.AddDays(7));

        var score1 = analytics.LogPopularityScore("Test Book");
        var score2 = analytics.LogPopularityScore("test book");
        var score3 = analytics.LogPopularityScore("TEST BOOK");

        Assert.Equal(score1, score2);
        Assert.Equal(score1, score3);
    }
}
