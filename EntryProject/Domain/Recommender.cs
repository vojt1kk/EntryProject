namespace EntryProject.Domain;

/// <summary>Weakest area + comment for the user.</summary>
public record WeakArea(Habit Habit, int Score, string Comment);

/// <summary>Recommended study plan.</summary>
public record Recommendation(
    Level Level,
    IReadOnlyList<string> Steps,
    string? RoleTip,
    IReadOnlyList<WeakArea> WeakestAreas,
    bool Masterclass,
    string? MasterclassText);

/// <summary>
/// Builds a study plan from the test result and role: 2–3 concrete steps (courses of the current
/// level + the next level) and the two weakest areas with a comment.
/// </summary>
public static class Recommender
{
    public static Recommendation Build(ScoreResult result, Role role, bool isManagement)
    {
        var steps = new List<string>();

        // 1–2 steps from the current level
        var currentLevelCourses = CourseCatalog.CoursesByLevel[result.Level];
        steps.Add(currentLevelCourses[0]);
        if (currentLevelCourses.Count > 1)
            steps.Add(currentLevelCourses[1]);

        // next level (if it exists) – one step ahead
        var nextLevel = CourseCatalog.NextLevel(result.Level);
        if (nextLevel is { } level)
        {
            steps.Add($"Až budete připraveni, navažte úrovní {LevelCatalog.Name(level)}: "
                      + CourseCatalog.CoursesByLevel[level][0]);
        }

        // role-specific tip (separate note, not a numbered step – to keep steps at 2–3)
        CourseCatalog.RoleTip.TryGetValue(role, out var roleTip);

        // two weakest areas (lowest score)
        var weakestAreas = result.HabitScores
            .OrderBy(kv => kv.Value)
            .ThenBy(kv => (int)kv.Key)
            .Take(2)
            .Select(kv => new WeakArea(
                kv.Key,
                kv.Value,
                $"Oblast „{HabitCatalog.Name(kv.Key)}“ máte zatím nejslabší (skóre {kv.Value}/100). "
                + $"Zaměřte se na to, abyste {HabitCatalog.Descriptions[kv.Key]}."))
            .ToList();

        var masterclass = ScoringService.OfferMasterclass(role, isManagement);

        return new Recommendation(
            result.Level,
            steps,
            roleTip,
            weakestAreas,
            masterclass,
            masterclass ? CourseCatalog.Masterclass : null);
    }
}
