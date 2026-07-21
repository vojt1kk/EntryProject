namespace EntryProject.Domain;

/// <summary>One benchmark test question – belongs to one habit, rated on a 1–5 scale.</summary>
public record Question(int Id, Habit Habit, string Text);

/// <summary>
/// Catalog of 12 questions: 2 per each of the 6 habits, phrased for the healthcare context.
/// Scale 1 = "does not describe me at all" ... 5 = "describes me perfectly".
/// </summary>
public static class QuestionCatalog
{
    public static readonly IReadOnlyList<Question> All = new List<Question>
    {
        // Habit 1 – Early adoption of AI
        new(1, Habit.EarlyAdoption,
            "Když řeším nový pracovní úkol (např. přípravu edukace pacienta nebo shrnutí dokumentace), zapojím AI hned na začátku, ne až když si nevím rady."),
        new(2, Habit.EarlyAdoption,
            "U rutinních činností ve své roli aktivně přemýšlím, kde by mi AI mohla ušetřit čas, a zkusím ji nasadit dřív než ostatní."),

        // Habit 2 – Experimentation
        new(3, Habit.Experimentation,
            "Zkouším různé AI nástroje a různé formulace zadání, abych našel(a), co pro dané zdravotnické zadání funguje nejlépe."),
        new(4, Habit.Experimentation,
            "Nebojím se AI vyzkoušet i na netypickém úkolu ve své praxi, i když si nejsem jistý(á) výsledkem."),

        // Habit 3 – Iteration
        new(5, Habit.Iteration,
            "První odpověď AI beru jako návrh a dalšími pokyny ji postupně upřesňuji, dokud nesplňuje můj záměr."),
        new(6, Habit.Iteration,
            "Když výstup AI není použitelný pro klinický/pracovní kontext, přeformuluji zadání místo toho, abych to vzdal(a)."),

        // Habit 4 – Output verification
        new(7, Habit.OutputVerification,
            "Výstupy AI týkající se zdraví nebo dat pacientů si vždy ověřím proti důvěryhodnému zdroji, než je použiji."),
        new(8, Habit.OutputVerification,
            "Jsem si vědom(a) rizika, že AI může „halucinovat“, a aktivně kontroluji fakta i citace, které vygeneruje."),

        // Habit 5 – Saved workflows
        new(9, Habit.SavedWorkflows,
            "Osvědčené prompty a postupy práce s AI si ukládám, abych je mohl(a) příště znovu použít."),
        new(10, Habit.SavedWorkflows,
            "Mám (nebo bych si rád[a] vytvořil[a]) knihovnu opakovaně použitelných zadání pro typické úkoly ve své roli."),

        // Habit 6 – Sharing workflows
        new(11, Habit.Sharing,
            "Když najdu funkční způsob použití AI, podělím se o něj s kolegy nebo v týmu."),
        new(12, Habit.Sharing,
            "Aktivně přispívám k tomu, aby se dobrá praxe práce s AI šířila na mém pracovišti."),
    };

    public static IReadOnlyList<Question> ForHabit(Habit habit) =>
        All.Where(q => q.Habit == habit).ToList();

    public const int MinAnswer = 1;
    public const int MaxAnswer = 5;
}
