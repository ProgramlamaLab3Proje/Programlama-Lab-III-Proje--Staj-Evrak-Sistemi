using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StajEvrakSistemi.Pages;

public class StudentModel : PageModel
{
    [BindProperty]
    public DateTime? StartDate { get; set; }

    [BindProperty]
    public DateTime? EndDate { get; set; }

    [BindProperty]
    public DateTime? SubmissionDate { get; set; }

    [BindProperty]
    public List<string> SelectedDays { get; set; } = new();

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        return Page();
    }
}