namespace LibraryApp.Services;

using LibraryApp.Domain;

public class AnalyticsService
{
    private readonly LibraryService _libraryService;

    public AnalyticsService(LibraryService libraryService)
    {
        _libraryService = libraryService ?? throw new ArgumentNullException(nameof(libraryService));
    }

    public double AverageLoanLengthDays()
    {
        var reservations = _libraryService.GetAllReservations().ToList();

        if (reservations.Count == 0)
            return 0;

        return reservations.Average(r => r.GetLengthInDays());
    }

    public int TotalLoans()
    {
        return _libraryService.GetAllReservations().Count();
    }

    public string MostPopularItemTitle()
    {
        var reservations = _libraryService.GetAllReservations().ToList();

        if (reservations.Count == 0)
            return "N/A";

        var popularItem = reservations
            .GroupBy(r => r.Item.Id)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();

        return popularItem?.First().Item.Title ?? "N/A";
    }

    public double FulfillmentRate()
    {
        var reservations = _libraryService.GetAllReservations().ToList();

        if (reservations.Count == 0)
            return 0;

        var activeCount = reservations.Count(r => r.IsActive);
        return (double)activeCount / reservations.Count * 100;
    }

    public double LogPopularityScore(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or empty.", nameof(title));

        var reservations = _libraryService.GetAllReservations()
            .Where(r => r.Item.Title.Equals(title, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var count = reservations.Count;

        if (count <= 0)
            return 0;

        // Safe logarithm calculation - only for positive values
        return Math.Log(count + 1);
    }
}
