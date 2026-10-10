using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infastructure.DbContext;

public static class DatabaseBootstrap
{
    public static async Task<string> EnsureReadyAsync()
    {
        // Портативная сборка / если PostgreSQL нет — встроенный файл carfix.db (SQLite)
        if (ConnectionHelper.IsPortableMode())
            return await UseSqliteAsync("portable mode");

        var errors = new List<string>();
        foreach (var candidate in ConnectionHelper.GetPostgresCandidates())
        {
            try
            {
                try { await EnsurePostgresDatabaseExistsAsync(candidate); }
                catch { /* ignore */ }

                var options = new DbContextOptionsBuilder<ContextDb>()
                    .UseNpgsql(candidate)
                    .Options;

                await using var test = new ContextDb(options);
                if (!await test.Database.CanConnectAsync())
                    continue;

                ConnectionHelper.SetConnectionString(candidate, sqlite: false);
                await DbInitializer.InitializeAsync(test);
                Console.WriteLine("PostgreSQL OK: " + Mask(candidate));
                return candidate;
            }
            catch (Exception ex)
            {
                errors.Add($"{Mask(candidate)} -> {ex.GetBaseException().Message}");
            }
        }

        Console.WriteLine("PostgreSQL недоступен — включаю встроенную БД carfix.db");
        Console.WriteLine(string.Join("\n", errors.Take(4)));
        return await UseSqliteAsync("fallback");
    }

    private static async Task<string> UseSqliteAsync(string reason)
    {
        var cs = ConnectionHelper.GetSqliteConnectionString();
        ConnectionHelper.SetConnectionString(cs, sqlite: true);

        var options = new DbContextOptionsBuilder<ContextDb>()
            .UseSqlite(cs)
            .Options;

        await using var db = new ContextDb(options);
        await DbInitializer.InitializeAsync(db);
        Console.WriteLine($"SQLite OK ({reason}): {ConnectionHelper.GetSqlitePath()}");
        return cs;
    }

    private static async Task EnsurePostgresDatabaseExistsAsync(string appConnection)
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
            if (await check.ExecuteScalarAsync() != null)
                return;
        }

        await using var create = conn.CreateCommand();
        create.CommandText = $"CREATE DATABASE \"{dbName.Replace("\"", "\"\"")}\"";
        await create.ExecuteNonQueryAsync();
    }

    private static string Mask(string cs)
    {
        try
        {
            var b = new NpgsqlConnectionStringBuilder(cs);
            if (!string.IsNullOrEmpty(b.Password)) b.Password = "***";
            return b.ToString();
        }
        catch { return cs; }
    }
}
