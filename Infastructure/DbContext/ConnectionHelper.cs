namespace Infastructure.DbContext;

public static class ConnectionHelper
{
    private static string? _resolved;
    private static bool _useSqlite;

    public const string DefaultConnection =
        "Host=localhost;Port=5432;Database=carfixdb;Username=postgres;Password=123";

    public static bool UseSqlite => _useSqlite;

    public static string GetConnectionString()
    {
        if (!string.IsNullOrWhiteSpace(_resolved))
            return _resolved;

        return Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
               ?? Environment.GetEnvironmentVariable("CARFIX_CONNECTION")
               ?? DefaultConnection;
    }

    public static void SetConnectionString(string connectionString, bool sqlite = false)
    {
        _resolved = connectionString;
        _useSqlite = sqlite;
    }

    public static string GetSqlitePath()
    {
        var dir = AppContext.BaseDirectory;
        return Path.Combine(dir, "carfix.db");
    }

    public static string GetSqliteConnectionString()
        => $"Data Source={GetSqlitePath()}";

    public static bool IsPortableMode()
    {
        if (string.Equals(Environment.GetEnvironmentVariable("CARFIX_PORTABLE"), "1", StringComparison.OrdinalIgnoreCase))
            return true;
        return File.Exists(Path.Combine(AppContext.BaseDirectory, "portable.marker"));
    }

    public static IEnumerable<string> GetPostgresCandidates()
    {
        var env = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                  ?? Environment.GetEnvironmentVariable("CARFIX_CONNECTION");
        if (!string.IsNullOrWhiteSpace(env) && !env.Contains("Data Source=", StringComparison.OrdinalIgnoreCase))
            yield return env!;

        var user = Environment.UserName;
        yield return "Host=localhost;Port=5432;Database=carfixdb;Username=postgres;Password=123";
        yield return "Host=localhost;Port=5432;Database=carfixdb;Username=postgres;Password=postgres";
        yield return "Host=localhost;Port=5432;Database=carfixdb;Username=postgres;Password=";
        yield return $"Host=localhost;Port=5432;Database=carfixdb;Username={user};Password=";
        yield return "Host=localhost;Port=5433;Database=carfixdb;Username=postgres;Password=123";
    }
}
