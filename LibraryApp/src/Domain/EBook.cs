namespace LibraryApp.Domain;

public class EBook : Book
{
    public string FileFormat { get; private set; }

    public EBook(int id, string title, string author, string isbn, string fileFormat)
        : base(id, title, author, isbn)
    {
        FileFormat = fileFormat ?? throw new ArgumentNullException(nameof(fileFormat));
    }

    public override void DisplayInfo()
    {
        Console.WriteLine($"[EBook] ID: {Id}, Title: {Title}, Author: {Author}, ISBN: {Isbn}, Format: {FileFormat}, Available: {IsAvailable}");
    }
}
