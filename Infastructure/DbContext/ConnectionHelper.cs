namespace Infastructure.DbContext;

public static class ConnectionHelper
{
    private static string? _resolved;

    public const string DefaultConnection =
        "Host=localhost;Port=5432;Database=carfixdb;Username=postgres;Password=123";

    public static string GetConnectionString()
    {
        if (!string.IsNullOrWhiteSpace(_resolved))
            return _resolved;

        return Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
               ?? Environment.GetEnvironmentVariable("CARFIX_CONNECTION")
               ?? DefaultConnection;
    }

    public static void SetConnectionString(string connectionString)
    {
        _resolved = connectionString;
    }

    /// <summary>Варианты подключения для Windows / Mac (Homebrew) без ручной настройки.</summary>
    public static IEnumerable<string> GetCandidates()
    {
        var env = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                  ?? Environment.GetEnvironmentVariable("CARFIX_CONNECTION");
        if (!string.IsNullOrWhiteSpace(env))
            yield return env!;

        var user = Environment.UserName;

        // Windows / явная учётка из README
        yield return "Host=localhost;Port=5432;Database=carfixdb;Username=postgres;Password=123";
        yield return "Host=localhost;Port=5432;Database=carfixdb;Username=postgres;Password=postgres";
        yield return "Host=localhost;Port=5432;Database=carfixdb;Username=postgres;Password=";

        // macOS Homebrew: суперпользователь = имя пользователя Mac, пароль обычно не нужен
        yield return $"Host=localhost;Port=5432;Database=carfixdb;Username={user};Password=";
        yield return $"Host=/tmp;Database=carfixdb;Username={user}";
        yield return $"Host=/var/run/postgresql;Database=carfixdb;Username={user}";

        // Docker-проброс на хост
        yield return "Host=localhost;Port=5433;Database=carfixdb;Username=postgres;Password=123";
    }
}
