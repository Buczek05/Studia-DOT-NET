using LibraryApp.Domain;
using Xunit;

namespace LibraryApp.Tests;

public class ReservationTests
{
    [Fact]
    public void Reservation_Constructor_ShouldSetAllProperties()
    {
        var book = new Book(1, "Test Book", "Author", "ISBN123");
        var from = DateTime.Now;
        var to = from.AddDays(7);

        var reservation = new Reservation(book, "user@example.com", from, to);

        Assert.NotEqual(0, reservation.Id);
        Assert.Equal(book, reservation.Item);
        Assert.Equal("user@example.com", reservation.UserEmail);
        Assert.Equal(from, reservation.From);
        Assert.Equal(to, reservation.To);
        Assert.True(reservation.IsActive);
    }

    [Fact]
    public void Reservation_Constructor_ShouldThrowArgumentNullException_WhenItemIsNull()
    {
        var from = DateTime.Now;
        var to = from.AddDays(7);

        Assert.Throws<ArgumentNullException>(() =>
            new Reservation(null, "user@example.com", from, to));
    }

    [Fact]
    public void Reservation_Constructor_ShouldThrowArgumentException_WhenUserEmailIsNull()
    {
        var book = new Book(1, "Test Book", "Author", "ISBN123");
        var from = DateTime.Now;
        var to = from.AddDays(7);

        Assert.Throws<ArgumentException>(() =>
            new Reservation(book, null, from, to));
    }

    [Fact]
    public void Reservation_Constructor_ShouldThrowArgumentException_WhenUserEmailIsEmpty()
    {
        var book = new Book(1, "Test Book", "Author", "ISBN123");
        var from = DateTime.Now;
        var to = from.AddDays(7);

        Assert.Throws<ArgumentException>(() =>
            new Reservation(book, "", from, to));
    }

    [Fact]
    public void Reservation_Constructor_ShouldThrowArgumentException_WhenUserEmailIsWhitespace()
    {
        var book = new Book(1, "Test Book", "Author", "ISBN123");
        var from = DateTime.Now;
        var to = from.AddDays(7);

        Assert.Throws<ArgumentException>(() =>
            new Reservation(book, "   ", from, to));
    }

    [Fact]
    public void Reservation_Constructor_ShouldThrowArgumentException_WhenFromIsAfterTo()
    {
        var book = new Book(1, "Test Book", "Author", "ISBN123");
        var from = DateTime.Now;
        var to = from.AddDays(-1);

        var exception = Assert.Throws<ArgumentException>(() =>
            new Reservation(book, "user@example.com", from, to));

        Assert.Contains("Start date must be before end date", exception.Message);
    }

    [Fact]
    public void Reservation_Constructor_ShouldThrowArgumentException_WhenFromEqualsTo()
    {
        var book = new Book(1, "Test Book", "Author", "ISBN123");
        var from = DateTime.Now;
        var to = from;

        var exception = Assert.Throws<ArgumentException>(() =>
            new Reservation(book, "user@example.com", from, to));

        Assert.Contains("Start date must be before end date", exception.Message);
    }

    [Fact]
    public void Reservation_Cancel_ShouldSetIsActiveToFalse()
    {
        var book = new Book(1, "Test Book", "Author", "ISBN123");
        var from = DateTime.Now;
        var to = from.AddDays(7);
        var reservation = new Reservation(book, "user@example.com", from, to);

        reservation.Cancel();

        Assert.False(reservation.IsActive);
    }

    [Fact]
    public void Reservation_Cancel_ShouldThrowInvalidOperationException_WhenAlreadyCancelled()
    {
        var book = new Book(1, "Test Book", "Author", "ISBN123");
        var from = DateTime.Now;
        var to = from.AddDays(7);
        var reservation = new Reservation(book, "user@example.com", from, to);

        reservation.Cancel();

        var exception = Assert.Throws<InvalidOperationException>(() => reservation.Cancel());
        Assert.Contains("Cannot cancel an inactive reservation", exception.Message);
    }

    [Fact]
    public void Reservation_GetLengthInDays_ShouldReturnCorrectValue()
    {
        var book = new Book(1, "Test Book", "Author", "ISBN123");
        var from = new DateTime(2024, 1, 1);
        var to = new DateTime(2024, 1, 8);
        var reservation = new Reservation(book, "user@example.com", from, to);

        var length = reservation.GetLengthInDays();

        Assert.Equal(7, length);
    }

    [Fact]
    public void Reservation_GetLengthInDays_ShouldHandleSingleDay()
    {
        var book = new Book(1, "Test Book", "Author", "ISBN123");
        var from = new DateTime(2024, 1, 1, 9, 0, 0);
        var to = new DateTime(2024, 1, 1, 17, 0, 0);
        var reservation = new Reservation(book, "user@example.com", from, to);

        var length = reservation.GetLengthInDays();

        Assert.Equal(0, length);
    }

    [Fact]
    public void Reservation_Id_ShouldBeUnique()
    {
        var book1 = new Book(1, "Book 1", "Author", "ISBN1");
        var book2 = new Book(2, "Book 2", "Author", "ISBN2");
        var from = DateTime.Now;
        var to = from.AddDays(7);

        var reservation1 = new Reservation(book1, "user1@example.com", from, to);
        var reservation2 = new Reservation(book2, "user2@example.com", from, to);

        Assert.NotEqual(reservation1.Id, reservation2.Id);
    }

    [Fact]
    public void Reservation_IsActive_ShouldDefaultToTrue()
    {
        var book = new Book(1, "Test Book", "Author", "ISBN123");
        var from = DateTime.Now;
        var to = from.AddDays(7);

        var reservation = new Reservation(book, "user@example.com", from, to);

        Assert.True(reservation.IsActive);
    }
}