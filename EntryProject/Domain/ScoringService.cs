namespace EntryProject.Domain;

/// <summary>Portal education level based on the overall score (an analogue of SP Score).</summary>
public enum Level
{
    Basic = 1,      // 0–39
    Advanced = 2,   // 40–74
    Specialist = 3  // 75–100
}

public static class LevelCatalog
{
    public static readonly IReadOnlyDictionary<Level, string> Names = new Dictionary<Level, string>
    {
        [Level.Basic] = "Level 1 – Základní",
        [Level.Advanced] = "Level 2 – Pokročilý",
        [Level.Specialist] = "Level 3 – Specialista"
    };

    public static string Name(Level level) => Names[level];
}

/// <summary>Result of evaluating a single test.</summary>
public record ScoreResult(
    IReadOnlyDictionary<Habit, int> HabitScores,
    int SpScore,
    Level Level);

/// <summary>
/// Pure scoring logic (no HTTP/DB dependency), fully unit-testable.
/// Habit score = round((sum of 2 answers − 2) / 8 × 100). SP Score = average of the 6 habits.
/// </summary>
public static class ScoringService
{
    /// <summary>Score of a single habit (0–100) from the sum of its two answers (2–10).</summary>
    public static int HabitScore(int twoAnswerSum)
    {
        if (twoAnswerSum is < 2 or > 10)
            throw new ArgumentOutOfRangeException(nameof(twoAnswerSum),
                twoAnswerSum, "Součet dvou odpovědí musí být v rozsahu 2–10.");

        return (int)Math.Round((twoAnswerSum - 2) / 8.0 * 100, MidpointRounding.AwayFromZero);
    }

    /// <summary>Level assignment based on the overall score (0–39 / 40–74 / 75–100).</summary>
    public static Level LevelFromScore(int spScore) => spScore switch
    {
        < 40 => Level.Basic,
        < 75 => Level.Advanced,
        _ => Level.Specialist
    };

    /// <summary>
    /// Evaluates the whole test. <paramref name="answers"/> maps question Id (1–12) to a value 1–5.
    /// Must contain all 12 questions.
    /// </summary>
    public static ScoreResult Evaluate(IReadOnlyDictionary<int, int> answers)
    {
        ArgumentNullException.ThrowIfNull(answers);

        var habitScores = new Dictionary<Habit, int>();
        foreach (var habit in HabitCatalog.All)
        {
            var questions = QuestionCatalog.ForHabit(habit);
            var sum = 0;
            foreach (var q in questions)
            {
                if (!answers.TryGetValue(q.Id, out var val))
                    throw new ArgumentException($"Chybí odpověď na otázku #{q.Id}.", nameof(answers));
                if (val is < QuestionCatalog.MinAnswer or > QuestionCatalog.MaxAnswer)
                    throw new ArgumentOutOfRangeException(nameof(answers),
                        val, $"Odpověď na otázku #{q.Id} musí být 1–5.");
                sum += val;
            }
            habitScores[habit] = HabitScore(sum);
        }

        var spScore = (int)Math.Round(habitScores.Values.Average(), MidpointRounding.AwayFromZero);
        return new ScoreResult(habitScores, spScore, LevelFromScore(spScore));
    }

    /// <summary>Offer the Masterclass (Management) – only to management within the non-clinical staff role.</summary>
    public static bool OfferMasterclass(Role role, bool isManagement) =>
        RoleCatalog.IsManagementRelevant(role) && isManagement;
}
