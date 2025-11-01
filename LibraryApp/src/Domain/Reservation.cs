namespace LibraryApp.Domain;

public class Reservation
{
    private static int _nextId = 1;

    public int Id { get; }
    public LibraryItem Item { get; private set; }
    public string UserEmail { get; private set; }
    public DateTime From { get; private set; }
    public DateTime To { get; private set; }
    public bool IsActive { get; private set; }

    public Reservation(LibraryItem item, string userEmail, DateTime from, DateTime to)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        if (string.IsNullOrWhiteSpace(userEmail))
            throw new ArgumentException("User email cannot be null or empty.", nameof(userEmail));

        if (from >= to)
            throw new ArgumentException("Start date must be before end date.", nameof(from));

        if (!item.IsAvailable)
            throw new InvalidOperationException($"Item '{item.Title}' is not available for reservation.");

        Id = _nextId++;
        Item = item;
        UserEmail = userEmail;
        From = from;
        To = to;
        IsActive = true;
    }

    public void Cancel()
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot cancel an inactive reservation.");

        IsActive = false;
    }

    public int GetLengthInDays()
    {
        return (To - From).Days;
    }
}