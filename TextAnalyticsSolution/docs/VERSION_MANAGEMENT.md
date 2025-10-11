# Zarządzanie Wersjami i Aktualizacje Pakietów

## Semantic Versioning (SemVer)

### Format Wersji: MAJOR.MINOR.PATCH

Semantic Versioning to standard wersjonowania oprogramowania, który wykorzystuje trzyczęściowy numer wersji:

```
MAJOR.MINOR.PATCH
  │     │     │
  │     │     └─── Patch: Poprawki błędów (backward compatible)
  │     └───────── Minor: Nowe funkcje (backward compatible)
  └─────────────── Major: Zmiany łamiące kompatybilność API
```

### Przykłady

- **13.0.3 → 13.0.4** (PATCH)
  - Tylko poprawki błędów
  - Bezpieczna aktualizacja
  - Brak zmian w API
  - **Zalecenie**: Aktualizuj zawsze

- **13.0.4 → 13.1.0** (MINOR)
  - Nowe funkcje dodane
  - Backward compatible (stary kod nadal działa)
  - API rozszerzone, nie zmienione
  - **Zalecenie**: Aktualizuj po testach

- **13.1.0 → 14.0.0** (MAJOR)
  - Zmiany łamiące API (breaking changes)
  - Może wymagać modyfikacji kodu
  - Usunięte lub zmienione metody/właściwości
  - **Zalecenie**: Aktualizuj ostrożnie, po szczegółowych testach

## Wpływ Aktualizacji na Kompatybilność API

### PATCH (13.0.3 → 13.0.4)

**Bezpieczne**, zawierają tylko:
- Naprawy błędów
- Poprawki wydajności
- Drobne optymalizacje

**Przykład**: Newtonsoft.Json 13.0.3 → 13.0.4
```csharp
// Kod działa identycznie przed i po aktualizacji
var json = JsonConvert.SerializeObject(obj);
var result = JsonConvert.DeserializeObject<T>(json);
```

### MINOR (9.0.0 → 9.0.9)

**Stosunkowo bezpieczne**, mogą zawierać:
- Nowe metody/klasy
- Nowe opcjonalne parametry
- Nowe funkcjonalności
- Deprecation warnings (ostrzeżenia o przyszłych zmianach)

**Przykład**: Microsoft.Extensions.DependencyInjection 9.0.0 → 9.0.9
```csharp
// Stary kod nadal działa
services.AddSingleton<ILogger, ConsoleLogger>();

// Nowe metody mogą być dostępne (opcjonalne)
// services.AddKeyedSingleton<ILogger, ConsoleLogger>("console");
```

### MAJOR (12.x.x → 13.0.0)

**Wymagają uwagi**, mogą zawierać:
- Usunięte przestarzałe metody
- Zmienione sygnatury metod
- Zmienione zachowanie istniejących API
- Nowe wymagania (np. .NET version)

**Przykład hipotetyczny**: Newtonsoft.Json 12.x → 13.0.0
```csharp
// Przed (v12):
JsonConvert.SerializeObject(obj, Formatting.Indented);

// Po (v13) - hipotetyczna zmiana:
// Może wymagać dodatkowych parametrów lub ustawień
JsonConvert.SerializeObject(obj, new JsonSerializerSettings
{
    Formatting = Formatting.Indented
});
```

## Praktyczne Polecenia dotnet CLI

### 1. Sprawdzenie Obecnych Pakietów

```bash
# Lista wszystkich pakietów w projekcie
dotnet list TextAnalytics.App/TextAnalytics.App.csproj package

# Wynik:
# Top-level Package                           Requested   Resolved
# > Newtonsoft.Json                           13.0.3      13.0.3
```

### 2. Sprawdzenie Dostępnych Aktualizacji

```bash
# Sprawdzenie przestarzałych pakietów
dotnet list TextAnalytics.App/TextAnalytics.App.csproj package --outdated

# Wynik pokazuje dostępne aktualizacje:
# Top-level Package                           Requested   Resolved   Latest
# > Newtonsoft.Json                           13.0.3      13.0.3     13.0.4
```

### 3. Aktualizacja Pakietu

```bash
# Aktualizacja do konkretnej wersji
dotnet add TextAnalytics.App/TextAnalytics.App.csproj package Newtonsoft.Json --version 13.0.4

# Aktualizacja do najnowszej wersji
dotnet add TextAnalytics.App/TextAnalytics.App.csproj package Newtonsoft.Json
```

### 4. Usunięcie Pakietu

```bash
dotnet remove TextAnalytics.App/TextAnalytics.App.csproj package Newtonsoft.Json
```

### 5. Restore Pakietów

```bash
# Przywrócenie wszystkich pakietów zgodnie z plikami .csproj
dotnet restore
```

## Historia Aktualizacji w Projekcie

### 2025-10-11: Aktualizacja Newtonsoft.Json

**Przed aktualizacją:**
```xml
<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
```

**Po aktualizacji:**
```xml
<PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
```

**Typ zmian**: PATCH (13.0.3 → 13.0.4)
**Wpływ na kod**: Brak zmian wymaganych w kodzie
**Powód**: Poprawki błędów i bezpieczeństwa
**Status testów**: ✓ Wszystkie testy przeszły

### 2025-10-11: Aktualizacja Microsoft.Extensions.DependencyInjection

**Przed aktualizacją:**
```xml
<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="9.0.0" />
```

**Po aktualizacji:**
```xml
<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="9.0.9" />
```

**Typ zmian**: MINOR (9.0.0 → 9.0.9)
**Wpływ na kod**: Brak zmian wymaganych w kodzie
**Powód**: Nowe funkcje i poprawki
**Status testów**: ✓ Wszystkie testy przeszły

## Strategie Aktualizacji

### 1. Konserwatywna (Zalecana dla produkcji)

- Aktualizuj tylko wersje PATCH automatycznie
- Wersje MINOR po przeglądzie changelog
- Wersje MAJOR tylko z zaplanowanym czasem na testy i refactoring

### 2. Zbalansowana

- Automatyczne aktualizacje PATCH
- Regularne (co miesiąc/kwartał) aktualizacje MINOR
- MAJOR w zaplanowanych "okienkach aktualizacyjnych"

### 3. Agresywna (Tylko dla dev/testowych środowisk)

- Automatyczne aktualizacje wszystkich pakietów
- Ciągłe monitorowanie breaking changes
- Szybkie reagowanie na problemy

## Zależności Przechodnie (Transitive Dependencies)

Pakiety mogą mieć własne zależności, które są instalowane automatycznie:

```
TextAnalytics.App
└── Newtonsoft.Json 13.0.4
    └── Microsoft.CSharp >= 4.3.0
    └── System.ComponentModel.TypeConverter >= 4.3.0
```

**Ważne**: Aktualizacja pakietu głównego może zaktualizować zależności przechodnie.

## Najlepsze Praktyki

1. **Przed aktualizacją**:
   - Przeczytaj changelog/release notes
   - Sprawdź listę breaking changes
   - Upewnij się, że testy są aktualne

2. **Podczas aktualizacji**:
   - Aktualizuj po jednym pakiecie na raz
   - Uruchom testy po każdej aktualizacji
   - Commituj zmiany osobno dla każdego pakietu

3. **Po aktualizacji**:
   - Uruchom pełny zestaw testów
   - Sprawdź aplikację manualnie
   - Monitoruj logi na środowisku testowym

4. **Wersjonowanie w .csproj**:
   ```xml
   <!-- Dokładna wersja (zalecane) -->
   <PackageReference Include="Newtonsoft.Json" Version="13.0.4" />

   <!-- Zakres wersji (ostrożnie) -->
   <PackageReference Include="Newtonsoft.Json" Version="13.0.*" />

   <!-- Minimum (nie zalecane w produkcji) -->
   <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
   ```

## Narzędzia Pomocnicze

### dotnet outdated (narzędzie globalne)

```bash
# Instalacja
dotnet tool install --global dotnet-outdated-tool

# Użycie
dotnet outdated

# Pokazuje kolorowy raport z kategoryzacją aktualizacji
```

### Dependabot (GitHub)

Automatyczne pull requesty z aktualizacjami pakietów NuGet.

### NuGet Package Manager (Visual Studio)

GUI do zarządzania pakietami w Visual Studio.

## Podsumowanie

- **SemVer** zapewnia przewidywalność zmian
- **PATCH**: Bezpiecznie aktualizuj
- **MINOR**: Testuj przed aktualizacją
- **MAJOR**: Planuj czas na refactoring
- Zawsze uruchamiaj testy po aktualizacji
- Dokumentuj ważne aktualizacje

## Dalsze Zasoby

- [Semantic Versioning](https://semver.org/)
- [.NET Package Management](https://docs.microsoft.com/en-us/nuget/)
- [Newtonsoft.Json Changelog](https://github.com/JamesNK/Newtonsoft.Json/releases)