namespace LibraryApp.Services;

using LibraryApp.Domain;

public class LibraryService
{
    private readonly List<LibraryItem> _items = new();
    private readonly List<Reservation> _reservations = new();
    private readonly List<string> _users = new();
    private int _nextId = 1;

    // Events
    public event Action<Reservation>? OnNewReservation;
    public event Action<Reservation>? OnReservationCancelled;

    public int NextId() => _nextId++;

    public void AddItem(LibraryItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        _items.Add(item);
    }

    public void RegisterUser(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));

        if (_users.Contains(email))
            throw new InvalidOperationException($"User with email '{email}' is already registered.");

        _users.Add(email);
    }

    public IEnumerable<LibraryItem> ListAvailableItems()
    {
        return _items.Where(item => item.IsAvailable);
    }

    public IEnumerable<LibraryItem> GetAllItems()
    {
        return _items.AsReadOnly();
    }

    public IEnumerable<Reservation> GetAllReservations()
    {
        return _reservations.AsReadOnly();
    }

    public Reservation CreateReservation(int itemId, string userEmail, DateTime from, DateTime to)
    {
        if (string.IsNullOrWhiteSpace(userEmail))
            throw new ArgumentException("User email cannot be null or empty.", nameof(userEmail));

        if (!_users.Contains(userEmail))
            throw new InvalidOperationException($"User '{userEmail}' is not registered.");

        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new ArgumentException($"Item with ID {itemId} not found.", nameof(itemId));

        if (!item.IsAvailable)
            throw new InvalidOperationException($"Item '{item.Title}' is not available for reservation.");

        // Check for date conflicts with existing active reservations
        var hasConflict = _reservations.Any(r =>
            r.Item.Id == itemId &&
            r.IsActive &&
            ((from >= r.From && from < r.To) ||
             (to > r.From && to <= r.To) ||
             (from <= r.From && to >= r.To)));

        if (hasConflict)
            throw new ReservationConflictException(
                $"The item '{item.Title}' is already reserved for the requested period.");

        var reservation = new Reservation(item, userEmail, from, to);
        _reservations.Add(reservation);

        // Mark item as unavailable
        item.SetAvailability(false);

        // Trigger event
        OnNewReservation?.Invoke(reservation);

        return reservation;
    }

    public void CancelReservation(int reservationId)
    {
        var reservation = _reservations.FirstOrDefault(r => r.Id == reservationId);
        if (reservation == null)
            throw new ArgumentException($"Reservation with ID {reservationId} not found.", nameof(reservationId));

        reservation.Cancel();
        reservation.Item.SetAvailability(true);

        // Trigger event
        OnReservationCancelled?.Invoke(reservation);
    }

    public IEnumerable<Reservation> GetUserReservations(string userEmail)
    {
        if (string.IsNullOrWhiteSpace(userEmail))
            throw new ArgumentException("User email cannot be null or empty.", nameof(userEmail));

        return _reservations.Where(r => r.UserEmail == userEmail && r.IsActive);
    }

    public bool IsUserRegistered(string email)
    {
        return _users.Contains(email);
    }

    public LibraryItem? GetItemById(int id)
    {
        return _items.FirstOrDefault(i => i.Id == id);
    }
}
