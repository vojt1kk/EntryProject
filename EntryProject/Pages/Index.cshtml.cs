using EntryProject.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EntryProject.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public Role? SelectedRole { get; set; }

    [BindProperty]
    public bool Management { get; set; }

    public IReadOnlyList<Role> Role => RoleCatalog.All.ToList();

    public void OnGet() { }

    public IActionResult OnPost()
    {
        if (SelectedRole is null)
        {
            ModelState.AddModelError(string.Empty, "Vyberte prosím svou roli.");
            return Page();
        }

        // The management flag only makes sense for non-clinical hospital staff.
        var management = RoleCatalog.IsManagementRelevant(SelectedRole.Value) && Management;
        return RedirectToPage("Test", new { role = (int)SelectedRole.Value, management });
    }
}
