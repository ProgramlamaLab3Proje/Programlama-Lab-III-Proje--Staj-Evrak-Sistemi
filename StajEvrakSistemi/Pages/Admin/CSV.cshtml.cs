using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StajEvrakSistemi.Data;
using StajEvrakSistemi.Utility;

namespace StajEvrakSistemi.Pages.Admin;

[Authorize(Roles = StaticRoles.ROLE_ADMIN)]
public class StudentRecordsModel : PageModel
{
    private readonly Context _context;

    public StudentRecordsModel(Context context)
    {
        _context = context;
    }

    [BindProperty]
    public IFormFile? CsvFile { get; set; }

    public string? Message { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (CsvFile == null)
        {
            Message = "Dosya seçilmedi.";
            return Page();
        }

        Message = $"Dosya yüklendi\n {CsvFile.FileName} ({CsvFile.Length} bayt)";
        return Page();
    }
}