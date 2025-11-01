using LibraryApp.Services;
using LibraryApp.Domain;
using LibraryApp.Extensions;

class Program
{
    static void Main()
    {
        var library = new LibraryService();
        var analytics = new AnalyticsService(library);

        // Subscribe to events
        library.OnNewReservation += r =>
            Console.WriteLine($"\n[INFO] Nowa rezerwacja: '{r.Item.Title}' dla {r.UserEmail} (od {r.From:yyyy-MM-dd} do {r.To:yyyy-MM-dd})\n");

        library.OnReservationCancelled += r =>
            Console.WriteLine($"\n[INFO] Anulowano rezerwację: '{r.Item.Title}' użytkownika {r.UserEmail}\n");

        // Seed some initial data
        SeedData(library);

        while (true)
        {
            try
            {
                Console.WriteLine("\n=== System Zarządzania Biblioteką ===");
                Console.WriteLine("1. Dodaj książkę");
                Console.WriteLine("2. Dodaj e-booka");
                Console.WriteLine("3. Zarejestruj użytkownika");
                Console.WriteLine("4. Pokaż dostępne pozycje");
                Console.WriteLine("5. Szukaj pozycji (tytuł/autor)");
                Console.WriteLine("6. Zarezerwuj pozycję");
                Console.WriteLine("7. Anuluj rezerwację");
                Console.WriteLine("8. Moje rezerwacje");
                Console.WriteLine("9. Statystyki");
                Console.WriteLine("0. Wyjście");
                Console.Write("> ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddBook(library);
                        break;
                    case "2":
                        AddEBook(library);
                        break;
                    case "3":
                        RegisterUser(library);
                        break;
                    case "4":
                        ShowAvailableItems(library);
                        break;
                    case "5":
                        SearchItems(library);
                        break;
                    case "6":
                        CreateReservation(library);
                        break;
                    case "7":
                        CancelReservation(library);
                        break;
                    case "8":
                        ShowUserReservations(library);
                        break;
                    case "9":
                        ShowStatistics(analytics);
                        break;
                    case "0":
                        Console.WriteLine("Do widzenia!");
                        return;
                    default:
                        Console.WriteLine("Nieznana opcja. Spróbuj ponownie.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[BŁĄD] {ex.Message}\n");
            }
        }
    }

    static void SeedData(LibraryService library)
    {
        // Add some books
        library.AddItem(new Book(library.NextId(), "Pan Tadeusz", "Adam Mickiewicz", "978-83-123-4567-8"));
        library.AddItem(new Book(library.NextId(), "Lalka", "Bolesław Prus", "978-83-234-5678-9"));
        library.AddItem(new EBook(library.NextId(), "Wiedźmin: Ostatnie życzenie", "Andrzej Sapkowski", "978-83-345-6789-0", "EPUB"));
        library.AddItem(new EBook(library.NextId(), "Solaris", "Stanisław Lem", "978-83-456-7890-1", "PDF"));

        // Register test users
        library.RegisterUser("jan.kowalski@example.com");
        library.RegisterUser("anna.nowak@example.com");
    }

    static void AddBook(LibraryService library)
    {
        Console.Write("Tytuł: ");
        var title = Console.ReadLine();
        Console.Write("Autor: ");
        var author = Console.ReadLine();
        Console.Write("ISBN: ");
        var isbn = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author) || string.IsNullOrWhiteSpace(isbn))
        {
            Console.WriteLine("Wszystkie pola są wymagane.");
            return;
        }

        library.AddItem(new Book(library.NextId(), title, author, isbn));
        Console.WriteLine("Książka została dodana.");
    }

    static void AddEBook(LibraryService library)
    {
        Console.Write("Tytuł: ");
        var title = Console.ReadLine();
        Console.Write("Autor: ");
        var author = Console.ReadLine();
        Console.Write("ISBN: ");
        var isbn = Console.ReadLine();
        Console.Write("Format (PDF/EPUB/MOBI): ");
        var format = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author) ||
            string.IsNullOrWhiteSpace(isbn) || string.IsNullOrWhiteSpace(format))
        {
            Console.WriteLine("Wszystkie pola są wymagane.");
            return;
        }

        library.AddItem(new EBook(library.NextId(), title, author, isbn, format));
        Console.WriteLine("E-book został dodany.");
    }

    static void RegisterUser(LibraryService library)
    {
        Console.Write("Email użytkownika: ");
        var email = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(email))
        {
            Console.WriteLine("Email nie może być pusty.");
            return;
        }

        library.RegisterUser(email);
        Console.WriteLine($"Użytkownik {email} został zarejestrowany.");
    }

    static void ShowAvailableItems(LibraryService library)
    {
        var items = library.ListAvailableItems().ToList();

        if (!items.Any())
        {
            Console.WriteLine("Brak dostępnych pozycji.");
            return;
        }

        Console.WriteLine("\n--- Dostępne pozycje ---");
        foreach (var item in items)
        {
            item.DisplayInfo();
        }
    }

    static void SearchItems(LibraryService library)
    {
        Console.WriteLine("1. Szukaj po tytule");
        Console.WriteLine("2. Szukaj po autorze");
        Console.Write("> ");
        var searchChoice = Console.ReadLine();

        Console.Write("Wpisz frazę do wyszukania: ");
        var searchTerm = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            Console.WriteLine("Fraza wyszukiwania nie może być pusta.");
            return;
        }

        IEnumerable<LibraryItem> results = Enumerable.Empty<LibraryItem>();

        if (searchChoice == "1")
        {
            results = library.GetAllItems().ByTitle(searchTerm);
        }
        else if (searchChoice == "2")
        {
            results = library.GetAllItems().ByAuthor(searchTerm);
        }
        else
        {
            Console.WriteLine("Nieprawidłowy wybór.");
            return;
        }

        var resultsList = results.ToList();
        if (!resultsList.Any())
        {
            Console.WriteLine("Nie znaleziono żadnych pozycji.");
            return;
        }

        Console.WriteLine("\n--- Wyniki wyszukiwania ---");
        foreach (var item in resultsList)
        {
            item.DisplayInfo();
        }
    }

    static void CreateReservation(LibraryService library)
    {
        Console.Write("ID pozycji do rezerwacji: ");
        if (!int.TryParse(Console.ReadLine(), out int itemId))
        {
            Console.WriteLine("Nieprawidłowy ID.");
            return;
        }

        Console.Write("Twój email: ");
        var email = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(email))
        {
            Console.WriteLine("Email nie może być pusty.");
            return;
        }

        Console.Write("Data rozpoczęcia (RRRR-MM-DD) [Enter = dzisiaj]: ");
        var fromStr = Console.ReadLine();
        DateTime from = string.IsNullOrWhiteSpace(fromStr) ? DateTime.Now : DateTime.Parse(fromStr);

        Console.Write("Data zakończenia (RRRR-MM-DD) [Enter = +7 dni]: ");
        var toStr = Console.ReadLine();
        DateTime to = string.IsNullOrWhiteSpace(toStr) ? DateTime.Now.AddDays(7) : DateTime.Parse(toStr);

        var reservation = library.CreateReservation(itemId, email, from, to);
        Console.WriteLine($"Rezerwacja utworzona! ID: {reservation.Id}");
    }

    static void CancelReservation(LibraryService library)
    {
        Console.Write("Twój email: ");
        var email = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(email))
        {
            Console.WriteLine("Email nie może być pusty.");
            return;
        }

        var userReservations = library.GetUserReservations(email).ToList();

        if (!userReservations.Any())
        {
            Console.WriteLine("Nie masz aktywnych rezerwacji.");
            return;
        }

        Console.WriteLine("\n--- Twoje rezerwacje ---");
        foreach (var res in userReservations)
        {
            Console.WriteLine($"ID: {res.Id}, Pozycja: '{res.Item.Title}', Od: {res.From:yyyy-MM-dd}, Do: {res.To:yyyy-MM-dd}");
        }

        Console.Write("\nID rezerwacji do anulowania: ");
        if (!int.TryParse(Console.ReadLine(), out int reservationId))
        {
            Console.WriteLine("Nieprawidłowy ID.");
            return;
        }

        library.CancelReservation(reservationId);
        Console.WriteLine("Rezerwacja została anulowana.");
    }

    static void ShowUserReservations(LibraryService library)
    {
        Console.Write("Twój email: ");
        var email = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(email))
        {
            Console.WriteLine("Email nie może być pusty.");
            return;
        }

        var reservations = library.GetUserReservations(email).ToList();

        if (!reservations.Any())
        {
            Console.WriteLine("Nie masz aktywnych rezerwacji.");
            return;
        }

        Console.WriteLine("\n--- Twoje aktywne rezerwacje ---");
        foreach (var res in reservations)
        {
            Console.WriteLine($"ID: {res.Id}, Pozycja: '{res.Item.Title}', Od: {res.From:yyyy-MM-dd}, Do: {res.To:yyyy-MM-dd}, Długość: {res.GetLengthInDays()} dni");
        }
    }

    static void ShowStatistics(AnalyticsService analytics)
    {
        Console.WriteLine("\n=== Statystyki Biblioteki ===");
        Console.WriteLine($"Łączna liczba rezerwacji: {analytics.TotalLoans()}");
        Console.WriteLine($"Średnia długość wypożyczenia: {analytics.AverageLoanLengthDays():F2} dni");
        Console.WriteLine($"Najpopularniejszy tytuł: {analytics.MostPopularItemTitle()}");
        Console.WriteLine($"Wskaźnik realizacji (aktywne rezerwacje): {analytics.FulfillmentRate():F2}%");
    }
}