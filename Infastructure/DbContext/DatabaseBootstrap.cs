using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infastructure.DbContext;

/// <summary>
/// Сам находит PostgreSQL, создаёт БД carfixdb при необходимости и выбирает рабочую строку подключения.
/// </summary>
public static class DatabaseBootstrap
{
    public static async Task<string> EnsureReadyAsync()
    {
        var errors = new List<string>();

        foreach (var candidate in ConnectionHelper.GetCandidates())
        {
            try
            {
                try
                {
                    await EnsureDatabaseExistsAsync(candidate);
                }
                catch
                {
                    // На части Mac-установок CREATE DATABASE не нужен / недоступен — пробуем connect as-is
                }

                var options = new DbContextOptionsBuilder<ContextDb>()
                    .UseNpgsql(candidate)
                    .Options;

                await using var test = new ContextDb(options);
                if (!await test.Database.CanConnectAsync())
                    continue;

                ConnectionHelper.SetConnectionString(candidate);
                await DbInitializer.InitializeAsync(test);
                Console.WriteLine("PostgreSQL OK: " + Mask(candidate));
                return candidate;
            }
            catch (Exception ex)
            {
                errors.Add($"{Mask(candidate)} -> {ex.GetBaseException().Message}");
            }
        }

        throw new InvalidOperationException(
            "Не удалось подключиться к PostgreSQL.\n\n" +
            "На Mac открой Terminal в папке CarFix и выполни ДВЕ команды:\n" +
            "  chmod +x scripts/start-mac.sh\n" +
            "  ./scripts/start-mac.sh\n\n" +
            "Скрипт сам поставит PostgreSQL, создаст базу и откроет программу.\n\n" +
            "Детали:\n" + string.Join("\n", errors.Take(8)));
    }

    private static async Task EnsureDatabaseExistsAsync(string appConnection)
    {
        var builder = new NpgsqlConnectionStringBuilder(appConnection);
        var dbName = string.IsNullOrWhiteSpace(builder.Database) ? "carfixdb" : builder.Database;
        builder.Database = "postgres";

        await using var conn = new NpgsqlConnection(builder.ConnectionString);
        await conn.OpenAsync();

        await using (var check = conn.CreateCommand())
        {
            check.CommandText = "SELECT 1 FROM pg_database WHERE datname = @n";
            check.Parameters.AddWithValue("n", dbName);
            var exists = await check.ExecuteScalarAsync();
            if (exists != null)
                return;
        }

        await using var create = conn.CreateCommand();
        create.CommandText = $"CREATE DATABASE \"{dbName.Replace("\"", "\"\"")}\"";
        await create.ExecuteNonQueryAsync();
        Console.WriteLine($"Created database {dbName}");
    }

    private static string Mask(string cs)
    {
        try
        {
            var b = new NpgsqlConnectionStringBuilder(cs);
            if (!string.IsNullOrEmpty(b.Password))
                b.Password = "***";
            return b.ToString();
        }
        catch
        {
            return cs;
        }
    }
}
