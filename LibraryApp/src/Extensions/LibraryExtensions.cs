namespace LibraryApp.Extensions;

using LibraryApp.Domain;

public static class LibraryExtensions
{
    public static IEnumerable<T> Available<T>(this IEnumerable<T> items) where T : LibraryItem
    {
        return items.Where(i => i.IsAvailable);
    }

    public static IEnumerable<LibraryItem> Newest(this IEnumerable<LibraryItem> items, int take)
    {
        return items.OrderByDescending(i => i.Id).Take(take);
    }

    public static IEnumerable<LibraryItem> ByTitle(this IEnumerable<LibraryItem> items, string titleFragment)
    {
        if (string.IsNullOrWhiteSpace(titleFragment))
            return items;

        return items.Where(i => i.Title.Contains(titleFragment, StringComparison.OrdinalIgnoreCase));
    }

    public static IEnumerable<Book> ByAuthor(this IEnumerable<LibraryItem> items, string authorFragment)
    {
        if (string.IsNullOrWhiteSpace(authorFragment))
            return Enumerable.Empty<Book>();

        return items.OfType<Book>()
            .Where(b => b.Author.Contains(authorFragment, StringComparison.OrdinalIgnoreCase));
    }
}
