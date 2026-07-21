using System.Text.Json;
using EntryProject.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EntryProject.Pages;

public class ResultModel : PageModel
{
    public Role Role { get; private set; }
    public ScoreResult Score { get; private set; } = null!;
    public Recommendation Recommendation { get; private set; } = null!;

    public string RoleName => RoleCatalog.Name(Role);
    public string LevelName => LevelCatalog.Name(Score.Level);

    public IActionResult OnGet()
    {
        if (TempData["result"] is not string json)
            return RedirectToPage("Index");

        var transfer = JsonSerializer.Deserialize<ResultTransfer>(json);
        if (transfer is null)
            return RedirectToPage("Index");

        Role = (Role)transfer.Role;

        var habitScores = transfer.HabitScores.ToDictionary(kv => (Habit)kv.Key, kv => kv.Value);
        Score = new ScoreResult(habitScores, transfer.SpScore, (Level)transfer.Level);
        Recommendation = Recommender.Build(Score, Role, transfer.Management);

        return Page();
    }
}
