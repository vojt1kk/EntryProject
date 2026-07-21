using EntryProject.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace EntryProject.Data;

/// <summary>
/// Result persistence. Writing goes DELIBERATELY through the stored procedure sp_insert_triage_result
/// (CALL), not through EF SaveChanges – so the SQL procedure is actually used, not decorative.
/// Statistics are read through the fn_role_statistics() function.
/// </summary>
public class ResultRepository
{
    private readonly TriageDbContext _db;

    public ResultRepository(TriageDbContext db) => _db = db;

    /// <summary>Stores an anonymized result by calling the sp_insert_triage_result procedure.</summary>
    public async Task InsertResultAsync(
        Role role, ScoreResult score, bool management, CancellationToken ct = default)
    {
        var sql = "CALL sp_insert_triage_result(@role, @s1, @s2, @s3, @s4, @s5, @s6, @total, @level, @management)";

        var parameters = new[]
        {
            new NpgsqlParameter("role", NpgsqlDbType.Integer) { Value = (int)role },
            new NpgsqlParameter("s1", NpgsqlDbType.Integer) { Value = score.HabitScores[Habit.EarlyAdoption] },
            new NpgsqlParameter("s2", NpgsqlDbType.Integer) { Value = score.HabitScores[Habit.Experimentation] },
            new NpgsqlParameter("s3", NpgsqlDbType.Integer) { Value = score.HabitScores[Habit.Iteration] },
            new NpgsqlParameter("s4", NpgsqlDbType.Integer) { Value = score.HabitScores[Habit.OutputVerification] },
            new NpgsqlParameter("s5", NpgsqlDbType.Integer) { Value = score.HabitScores[Habit.SavedWorkflows] },
            new NpgsqlParameter("s6", NpgsqlDbType.Integer) { Value = score.HabitScores[Habit.Sharing] },
            new NpgsqlParameter("total", NpgsqlDbType.Integer) { Value = score.SpScore },
            new NpgsqlParameter("level", NpgsqlDbType.Integer) { Value = (int)score.Level },
            new NpgsqlParameter("management", NpgsqlDbType.Boolean) { Value = management },
        };

        await _db.Database.ExecuteSqlRawAsync(sql, parameters, ct);
    }

    /// <summary>Loads anonymized statistics (averages per role) through the fn_role_statistics() function.</summary>
    public async Task<List<RoleStat>> GetRoleStatisticsAsync(CancellationToken ct = default)
    {
        return await _db.RoleStats
            .FromSqlRaw("SELECT * FROM fn_role_statistics()")
            .AsNoTracking()
            .ToListAsync(ct);
    }
}
