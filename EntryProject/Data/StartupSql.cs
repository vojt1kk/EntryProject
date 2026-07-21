using Microsoft.EntityFrameworkCore;

namespace EntryProject.Data;

/// <summary>
/// On application startup: EF migrations create/update the schema, then the genuine SQL
/// routines are applied idempotently (02_procedures.sql via CREATE OR REPLACE), and if the
/// table is empty, test data is loaded (03_seed.sql). This keeps the SQL scripts actually used.
/// </summary>
public static class StartupSql
{
    public static async Task InitializeAsync(IServiceProvider services, ILogger logger)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TriageDbContext>();

        // 1) The schema is driven by EF migrations (the sole authority over the schema).
        await db.Database.MigrateAsync();

        var sqlDir = FindSqlDirectory();
        if (sqlDir is null)
        {
            logger.LogWarning("Adresář sql/ nenalezen – procedury a seed nebyly aplikovány.");
            return;
        }

        // 2) Procedures/functions – always (CREATE OR REPLACE is idempotent).
        var proceduresScript = Path.Combine(sqlDir, "02_procedures.sql");
        if (File.Exists(proceduresScript))
        {
            await db.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(proceduresScript));
            logger.LogInformation("Aplikován skript 02_procedures.sql.");
        }

        // 3) Seed – only when the table is empty (idempotent guard).
        var count = await db.TriageResults.CountAsync();
        if (count == 0)
        {
            var seed = Path.Combine(sqlDir, "03_seed.sql");
            if (File.Exists(seed))
            {
                await db.Database.ExecuteSqlRawAsync(await File.ReadAllTextAsync(seed));
                logger.LogInformation("Nahrána testovací data (03_seed.sql).");
            }
        }
        else
        {
            logger.LogInformation("Tabulka již obsahuje {Count} záznamů – seed přeskočen.", count);
        }
    }

    /// <summary>Finds the sql/ directory walking up from the runtime directory (works from bin/ or the repo root).</summary>
    private static string? FindSqlDirectory()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, "sql");
            if (Directory.Exists(candidate) && File.Exists(Path.Combine(candidate, "02_procedures.sql")))
                return candidate;
            dir = dir.Parent;
        }
        return null;
    }
}
