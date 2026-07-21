using EntryProject.Data;
using EntryProject.Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EntryProject.Pages;

public class StatisticsModel : PageModel
{
    private readonly ResultRepository _repo;

    public StatisticsModel(ResultRepository repo) => _repo = repo;

    public List<RoleStat> Statistics { get; private set; } = new();
    public long TotalRecords => Statistics.Sum(s => s.ResponseCount);

    public string RoleName(int role) =>
        Enum.IsDefined(typeof(Role), role) ? RoleCatalog.Name((Role)role) : $"Role {role}";

    public async Task OnGetAsync(CancellationToken ct)
    {
        Statistics = await _repo.GetRoleStatisticsAsync(ct);
    }
}
