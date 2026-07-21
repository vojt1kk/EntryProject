using Microsoft.EntityFrameworkCore;

namespace EntryProject.Data;

public class TriageDbContext : DbContext
{
    public TriageDbContext(DbContextOptions<TriageDbContext> options) : base(options) { }

    public DbSet<TriageResult> TriageResults => Set<TriageResult>();

    /// <summary>Keyless entity mapped to the TABLE returned by the fn_role_statistics() function.</summary>
    public DbSet<RoleStat> RoleStats => Set<RoleStat>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Explicit snake_case mapping so the schema and the procedures match the SQL scripts.
        modelBuilder.Entity<TriageResult>(e =>
        {
            e.ToTable("triage_results");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Role).HasColumnName("role").HasConversion<int>();
            e.Property(x => x.EarlyAdoptionScore).HasColumnName("early_adoption_score");
            e.Property(x => x.ExperimentationScore).HasColumnName("experimentation_score");
            e.Property(x => x.IterationScore).HasColumnName("iteration_score");
            e.Property(x => x.OutputVerificationScore).HasColumnName("output_verification_score");
            e.Property(x => x.SavedWorkflowsScore).HasColumnName("saved_workflows_score");
            e.Property(x => x.SharingScore).HasColumnName("sharing_score");
            e.Property(x => x.TotalScore).HasColumnName("total_score");
            e.Property(x => x.Level).HasColumnName("level").HasConversion<int>();
            e.Property(x => x.Management).HasColumnName("management");
            e.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        });

        // Keyless – not mapped to a table, only to the result of the fn_role_statistics() function.
        // Explicit column names guarantee correct mapping via FromSqlRaw.
        modelBuilder.Entity<RoleStat>(e =>
        {
            e.HasNoKey();
            e.ToView(null);
            e.Property(x => x.Role).HasColumnName("role");
            e.Property(x => x.ResponseCount).HasColumnName("response_count");
            e.Property(x => x.AvgEarlyAdoptionScore).HasColumnName("avg_early_adoption_score");
            e.Property(x => x.AvgExperimentationScore).HasColumnName("avg_experimentation_score");
            e.Property(x => x.AvgIterationScore).HasColumnName("avg_iteration_score");
            e.Property(x => x.AvgOutputVerificationScore).HasColumnName("avg_output_verification_score");
            e.Property(x => x.AvgSavedWorkflowsScore).HasColumnName("avg_saved_workflows_score");
            e.Property(x => x.AvgSharingScore).HasColumnName("avg_sharing_score");
            e.Property(x => x.AvgTotalScore).HasColumnName("avg_total_score");
        });
    }
}
