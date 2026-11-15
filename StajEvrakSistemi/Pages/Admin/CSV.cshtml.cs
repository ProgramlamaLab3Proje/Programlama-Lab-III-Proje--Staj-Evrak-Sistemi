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
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB
    public string? Message { get; set; }
    public bool IsSuccess { get; set; }

    public StudentRecordsModel(Context context)
    {
        _context = context;
    }

    [BindProperty]
    public IFormFile? Csvdosyasi { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        // dosya seçilmiş mi null mı
        if (Csvdosyasi == null || Csvdosyasi.Length == 0)
        {
            Message = "Lütfen bir CSV dosyası seçili olduğundan emin olunuz.";
            IsSuccess = false;
            return Page();
        }

        // uzantı .csv mi
        string extension = Path.GetExtension(Csvdosyasi.FileName).ToLowerInvariant(); // tolowerinvariant ile küçük harfe çevrilir invariant kısmı türkçe karakterin I ı dönüşümünü engeller
        if (extension != ".csv")
        {
            Message = "Yalnızca .csv uzantılı dosyalar yüklenebilmektedir.";
            IsSuccess = false;
            return Page();
        }

        // boyut sınırı aşılmış mı
        if (Csvdosyasi.Length > MaxFileSize)
        {
            Message = "Dosya boyutunun 5 mb sınırını aşmadığından emin olunuz.";
            IsSuccess = false;
            return Page();
        }

        Message = $"Dosyanız başarıyla yüklenmiştir: {Csvdosyasi.FileName}";
        IsSuccess = true;
        return Page();
    }
}