namespace LibraryApp.Domain;

public class Book : LibraryItem
{
    public string Author { get; protected set; }
    public string Isbn { get; protected set; }

    public Book(int id, string title, string author, string isbn)
        : base(id, title)
    {
        Author = author ?? throw new ArgumentNullException(nameof(author));
        Isbn = isbn ?? throw new ArgumentNullException(nameof(isbn));
    }

    public override void DisplayInfo()
    {
        Console.WriteLine($"[Book] ID: {Id}, Title: {Title}, Author: {Author}, ISBN: {Isbn}, Available: {IsAvailable}");
    }
}