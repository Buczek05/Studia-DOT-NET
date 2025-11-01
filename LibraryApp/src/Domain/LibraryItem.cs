namespace LibraryApp.Domain;

public abstract class LibraryItem : IReservable
{
    public int Id { get; }
    public string Title { get; protected set; }
    public bool IsAvailable { get; protected set; } = true;

    protected LibraryItem(int id, string title)
    {
        Id = id;
        Title = title ?? throw new ArgumentNullException(nameof(title));
    }

    public abstract void DisplayInfo();

    public void SetAvailability(bool isAvailable)
    {
        IsAvailable = isAvailable;
    }

    // IReservable implementation - simplified, actual logic in LibraryService
    public virtual void Reserve(string userEmail, DateTime from, DateTime to)
    {
        if (!IsAvailable)
            throw new InvalidOperationException($"Item '{Title}' is not available for reservation.");

        if (from >= to)
            throw new ArgumentException("Start date must be before end date.");

        IsAvailable = false;
    }

    public virtual void CancelReservation(string userEmail)
    {
        IsAvailable = true;
    }

    bool IReservable.IsAvailable()
    {
        return IsAvailable;
    }
}
