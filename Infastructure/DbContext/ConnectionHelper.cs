namespace Infastructure.DbContext;

public static class ConnectionHelper
{
    public const string DefaultConnection =
        "Host=localhost;Port=5432;Database=carfixdb;Username=postgres;Password=123";

    public static string GetConnectionString()
    {
        return Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
               ?? Environment.GetEnvironmentVariable("CARFIX_CONNECTION")
               ?? DefaultConnection;
    }
}
