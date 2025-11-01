using LibraryApp.Domain;
using LibraryApp.Services;
using Xunit;

namespace LibraryApp.Tests;

public class LibraryServiceTests
{
    [Fact]
    public void AddItem_ShouldAddItemToLibrary()
    {
        var service = new LibraryService();
        var book = new Book(1, "Test Book", "Author", "ISBN");

        service.AddItem(book);

        var items = service.GetAllItems();
        Assert.Contains(book, items);
    }

    [Fact]
    public void AddItem_ShouldThrowArgumentNullException_WhenItemIsNull()
    {
        var service = new LibraryService();

        Assert.Throws<ArgumentNullException>(() => service.AddItem(null));
    }

    [Fact]
    public void RegisterUser_ShouldAddUserToList()
    {
        var service = new LibraryService();
        var email = "test@example.com";

        service.RegisterUser(email);

        Assert.True(service.IsUserRegistered(email));
    }

    [Fact]
    public void RegisterUser_ShouldThrowArgumentException_WhenEmailIsNull()
    {
        var service = new LibraryService();

        Assert.Throws<ArgumentException>(() => service.RegisterUser(null));
    }

    [Fact]
    public void RegisterUser_ShouldThrowArgumentException_WhenEmailIsEmpty()
    {
        var service = new LibraryService();

        Assert.Throws<ArgumentException>(() => service.RegisterUser(""));
    }

    [Fact]
    public void RegisterUser_ShouldThrowInvalidOperationException_WhenUserAlreadyRegistered()
    {
        var service = new LibraryService();
        var email = "test@example.com";
        service.RegisterUser(email);

        var exception = Assert.Throws<InvalidOperationException>(() => service.RegisterUser(email));
        Assert.Contains("already registered", exception.Message);
    }

    [Fact]
    public void ListAvailableItems_ShouldReturnOnlyAvailableItems()
    {
        var service = new LibraryService();
        var book1 = new Book(1, "Available Book", "Author", "ISBN1");
        var book2 = new Book(2, "Unavailable Book", "Author", "ISBN2");

        service.AddItem(book1);
        service.AddItem(book2);
        book2.SetAvailability(false);

        var availableItems = service.ListAvailableItems().ToList();

        Assert.Single(availableItems);
        Assert.Contains(book1, availableItems);
        Assert.DoesNotContain(book2, availableItems);
    }

    [Fact]
    public void CreateReservation_ShouldCreateReservationSuccessfully()
    {
        var service = new LibraryService();
        var book = new Book(1, "Test Book", "Author", "ISBN");
        var email = "test@example.com";

        service.AddItem(book);
        service.RegisterUser(email);

        var from = DateTime.Now;
        var to = from.AddDays(7);

        var reservation = service.CreateReservation(book.Id, email, from, to);

        Assert.NotNull(reservation);
        Assert.Equal(book, reservation.Item);
        Assert.Equal(email, reservation.UserEmail);
        Assert.True(reservation.IsActive);
    }

    [Fact]
    public void CreateReservation_ShouldMarkItemAsUnavailable()
    {
        var service = new LibraryService();
        var book = new Book(1, "Test Book", "Author", "ISBN");
        var email = "test@example.com";

        service.AddItem(book);
        service.RegisterUser(email);

        var from = DateTime.Now;
        var to = from.AddDays(7);

        service.CreateReservation(book.Id, email, from, to);

        Assert.False(book.IsAvailable);
    }

    [Fact]
    public void CreateReservation_ShouldTriggerOnNewReservationEvent()
    {
        var service = new LibraryService();
        var book = new Book(1, "Test Book", "Author", "ISBN");
        var email = "test@example.com";

        service.AddItem(book);
        service.RegisterUser(email);

        Reservation? eventReservation = null;
        service.OnNewReservation += r => eventReservation = r;

        var from = DateTime.Now;
        var to = from.AddDays(7);

        var reservation = service.CreateReservation(book.Id, email, from, to);

        Assert.NotNull(eventReservation);
        Assert.Equal(reservation, eventReservation);
    }

    [Fact]
    public void CreateReservation_ShouldThrowInvalidOperationException_WhenUserNotRegistered()
    {
        var service = new LibraryService();
        var book = new Book(1, "Test Book", "Author", "ISBN");

        service.AddItem(book);

        var from = DateTime.Now;
        var to = from.AddDays(7);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            service.CreateReservation(book.Id, "unknown@example.com", from, to));

        Assert.Contains("not registered", exception.Message);
    }

    [Fact]
    public void CreateReservation_ShouldThrowArgumentException_WhenItemNotFound()
    {
        var service = new LibraryService();
        var email = "test@example.com";

        service.RegisterUser(email);

        var from = DateTime.Now;
        var to = from.AddDays(7);

        var exception = Assert.Throws<ArgumentException>(() =>
            service.CreateReservation(999, email, from, to));

        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public void CreateReservation_ShouldThrowInvalidOperationException_WhenItemNotAvailable()
    {
        var service = new LibraryService();
        var book = new Book(1, "Test Book", "Author", "ISBN");
        var email = "test@example.com";

        service.AddItem(book);
        service.RegisterUser(email);
        book.SetAvailability(false);

        var from = DateTime.Now;
        var to = from.AddDays(7);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            service.CreateReservation(book.Id, email, from, to));

        Assert.Contains("not available", exception.Message);
    }

    [Fact]
    public void CreateReservation_ShouldThrowReservationConflictException_WhenDatesOverlap()
    {
        var service = new LibraryService();
        var book = new Book(1, "Test Book", "Author", "ISBN");
        var email1 = "user1@example.com";
        var email2 = "user2@example.com";

        service.AddItem(book);
        service.RegisterUser(email1);
        service.RegisterUser(email2);

        var from1 = new DateTime(2024, 1, 1);
        var to1 = new DateTime(2024, 1, 10);

        service.CreateReservation(book.Id, email1, from1, to1);

        // Try to create overlapping reservation
        var from2 = new DateTime(2024, 1, 5);
        var to2 = new DateTime(2024, 1, 15);

        var exception = Assert.Throws<ReservationConflictException>(() =>
            service.CreateReservation(book.Id, email2, from2, to2));

        Assert.Contains("already reserved", exception.Message);
    }

    [Fact]
    public void CancelReservation_ShouldMarkReservationAsInactive()
    {
        var service = new LibraryService();
        var book = new Book(1, "Test Book", "Author", "ISBN");
        var email = "test@example.com";

        service.AddItem(book);
        service.RegisterUser(email);

        var from = DateTime.Now;
        var to = from.AddDays(7);

        var reservation = service.CreateReservation(book.Id, email, from, to);
        service.CancelReservation(reservation.Id);

        Assert.False(reservation.IsActive);
    }

    [Fact]
    public void CancelReservation_ShouldMarkItemAsAvailable()
    {
        var service = new LibraryService();
        var book = new Book(1, "Test Book", "Author", "ISBN");
        var email = "test@example.com";

        service.AddItem(book);
        service.RegisterUser(email);

        var from = DateTime.Now;
        var to = from.AddDays(7);

        var reservation = service.CreateReservation(book.Id, email, from, to);
        service.CancelReservation(reservation.Id);

        Assert.True(book.IsAvailable);
    }

    [Fact]
    public void CancelReservation_ShouldTriggerOnReservationCancelledEvent()
    {
        var service = new LibraryService();
        var book = new Book(1, "Test Book", "Author", "ISBN");
        var email = "test@example.com";

        service.AddItem(book);
        service.RegisterUser(email);

        Reservation? eventReservation = null;
        service.OnReservationCancelled += r => eventReservation = r;

        var from = DateTime.Now;
        var to = from.AddDays(7);

        var reservation = service.CreateReservation(book.Id, email, from, to);
        service.CancelReservation(reservation.Id);

        Assert.NotNull(eventReservation);
        Assert.Equal(reservation, eventReservation);
    }

    [Fact]
    public void CancelReservation_ShouldThrowArgumentException_WhenReservationNotFound()
    {
        var service = new LibraryService();

        var exception = Assert.Throws<ArgumentException>(() =>
            service.CancelReservation(999));

        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public void GetUserReservations_ShouldReturnOnlyActiveReservationsForUser()
    {
        var service = new LibraryService();
        var book1 = new Book(1, "Book 1", "Author", "ISBN1");
        var book2 = new Book(2, "Book 2", "Author", "ISBN2");
        var email = "test@example.com";

        service.AddItem(book1);
        service.AddItem(book2);
        service.RegisterUser(email);

        var from = DateTime.Now;
        var to = from.AddDays(7);

        var reservation1 = service.CreateReservation(book1.Id, email, from, to);
        service.CancelReservation(reservation1.Id);

        book2.SetAvailability(true); // Make available again for second reservation
        var reservation2 = service.CreateReservation(book2.Id, email, from, to);

        var userReservations = service.GetUserReservations(email).ToList();

        Assert.Single(userReservations);
        Assert.Contains(reservation2, userReservations);
        Assert.DoesNotContain(reservation1, userReservations);
    }

    [Fact]
    public void GetUserReservations_ShouldThrowArgumentException_WhenEmailIsEmpty()
    {
        var service = new LibraryService();

        Assert.Throws<ArgumentException>(() => service.GetUserReservations(""));
    }
}
