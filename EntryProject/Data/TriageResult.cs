using EntryProject.Domain;

namespace EntryProject.Data;

/// <summary>
/// Anonymized triage result stored in the relational DB.
/// Contains no personal data – just role, scores, level, and date.
/// </summary>
public class TriageResult
{
    public int Id { get; set; }

    public Role Role { get; set; }

    public int EarlyAdoptionScore { get; set; }
    public int ExperimentationScore { get; set; }
    public int IterationScore { get; set; }
    public int OutputVerificationScore { get; set; }
    public int SavedWorkflowsScore { get; set; }
    public int SharingScore { get; set; }

    public int TotalScore { get; set; }

    public Level Level { get; set; }

    /// <summary>Management flag (relevant only for non-clinical hospital staff).</summary>
    public bool Management { get; set; }

    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Keyless entity for reading the result of the fn_role_statistics() function via FromSqlRaw.
/// </summary>
public class RoleStat
{
    public int Role { get; set; }
    public long ResponseCount { get; set; }
    public double AvgEarlyAdoptionScore { get; set; }
    public double AvgExperimentationScore { get; set; }
    public double AvgIterationScore { get; set; }
    public double AvgOutputVerificationScore { get; set; }
    public double AvgSavedWorkflowsScore { get; set; }
    public double AvgSharingScore { get; set; }
    public double AvgTotalScore { get; set; }
}
