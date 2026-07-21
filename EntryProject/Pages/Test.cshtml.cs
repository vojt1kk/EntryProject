using System.Text.Json;
using EntryProject.Data;
using EntryProject.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EntryProject.Pages;

public class TestModel : PageModel
{
    private readonly ResultRepository _repo;

    public TestModel(ResultRepository repo) => _repo = repo;

    [BindProperty(SupportsGet = true)]
    public int Role { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool Management { get; set; }

    /// <summary>Answers: key = question Id (1–12), value = 1–5.</summary>
    [BindProperty]
    public Dictionary<int, int> Answers { get; set; } = new();

    public IReadOnlyList<Question> Questions => QuestionCatalog.All;

    public string RoleName => Enum.IsDefined(typeof(Role), Role)
        ? RoleCatalog.Name((Role)Role)
        : "—";

    public IActionResult OnGet()
    {
        if (!Enum.IsDefined(typeof(Role), Role))
            return RedirectToPage("Index");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!Enum.IsDefined(typeof(Role), Role))
            return RedirectToPage("Index");

        // Server-side validation: all 12 questions answered and within range 1–5.
        foreach (var q in QuestionCatalog.All)
        {
            if (!Answers.TryGetValue(q.Id, out var val) ||
                val < QuestionCatalog.MinAnswer || val > QuestionCatalog.MaxAnswer)
            {
                ModelState.AddModelError(string.Empty,
                    "Vyplňte prosím všech 12 otázek (u každé zvolte hodnotu 1–5).");
                return Page();
            }
        }

        var role = (Role)Role;
        var management = RoleCatalog.IsManagementRelevant(role) && Management;

        var score = ScoringService.Evaluate(Answers);

        // Writing goes through the stored procedure (not through EF SaveChanges).
        await _repo.InsertResultAsync(role, score, management, ct);

        var transfer = new ResultTransfer(
            (int)role,
            management,
            score.HabitScores.ToDictionary(kv => (int)kv.Key, kv => kv.Value),
            score.SpScore,
            (int)score.Level);

        TempData["result"] = JsonSerializer.Serialize(transfer);
        return RedirectToPage("Result");
    }
}

/// <summary>Small DTO for transferring the result to the Result page via TempData (PRG pattern).</summary>
public record ResultTransfer(
    int Role,
    bool Management,
    Dictionary<int, int> HabitScores,
    int SpScore,
    int Level);
