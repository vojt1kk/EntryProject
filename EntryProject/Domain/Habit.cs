namespace EntryProject.Domain;

/// <summary>
/// Six habits of working with AI per the Aibility "Superpowered Professional" methodology.
/// </summary>
public enum Habit
{
    EarlyAdoption = 1,
    Experimentation = 2,
    Iteration = 3,
    OutputVerification = 4,
    SavedWorkflows = 5,
    Sharing = 6
}

public static class HabitCatalog
{
    public static readonly IReadOnlyDictionary<Habit, string> Names = new Dictionary<Habit, string>
    {
        [Habit.EarlyAdoption] = "Včasné zapojení AI",
        [Habit.Experimentation] = "Experimentování",
        [Habit.Iteration] = "Iterace",
        [Habit.OutputVerification] = "Ověřování výstupů",
        [Habit.SavedWorkflows] = "Uložené postupy",
        [Habit.Sharing] = "Sdílení workflow"
    };

    /// <summary>Short description of the habit's meaning – used in the weakest-areas comment.</summary>
    public static readonly IReadOnlyDictionary<Habit, string> Descriptions = new Dictionary<Habit, string>
    {
        [Habit.EarlyAdoption] = "zapojujete AI hned na začátku úkolu, ne až když jste v koncích",
        [Habit.Experimentation] = "zkoušely různé přístupy a nástroje místo jednoho zaběhnutého",
        [Habit.Iteration] = "výstup AI postupně vylepšujete dalšími pokyny, ne berete první verzi",
        [Habit.OutputVerification] = "kriticky ověřujete správnost a bezpečnost výstupů AI",
        [Habit.SavedWorkflows] = "osvědčené prompty a postupy si ukládáte pro opakované použití",
        [Habit.Sharing] = "sdílíte funkční workflow s kolegy a týmem"
    };

    public static string Name(Habit habit) => Names[habit];

    public static IEnumerable<Habit> All => Enum.GetValues<Habit>();
}
