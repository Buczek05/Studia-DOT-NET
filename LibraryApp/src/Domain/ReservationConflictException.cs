namespace LibraryApp.Domain;

public class ReservationConflictException : Exception
{
    public ReservationConflictException(string message) : base(message)
    {
    }

    public ReservationConflictException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
