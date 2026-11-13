using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StajEvrakSistemi.Models;
using StajEvrakSistemi.Services;

namespace StajEvrakSistemi.Pages;

public class StudentRecordModel : PageModel
{
    private readonly InternshipCalendarService _calendarService = new();

    [BindProperty]
    public DateTime? StartDate { get; set; }

    [BindProperty]
    public DateTime? EndDate { get; set; }

    [BindProperty]
    public DateTime? SubmissionDate { get; set; }

    [BindProperty]
    public StudentRecord StudentRecord { get; set; } = new();

    [BindProperty]
    public List<string> SelectedDays { get; set; } = new();

    [BindProperty]
    public IFormFile? UploadedFile { get; set; }

    public string UploadMessage { get; set; } = "";
    public bool IsUploadSuccess { get; set; }

    private const long MaxFileSize = 20 * 1024 * 1024; // 20 MB
    private readonly string[] allowedExtensions =
    {
        ".doc", ".docx", ".pdf", ".jpg", ".jpeg", ".png"
    };

    // Hesaplama sonucunu arayüze taşımak için
    public CalendarCalculationResult? CalculationResult { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        // temel tarihlerin girilip girilmediğini kontrol etmek için
        if (StartDate.HasValue && EndDate.HasValue && SubmissionDate.HasValue)
        {
            // seçilen günleri DayOfWeek türüne çevir
            var selectedDaysEnum = SelectedDays
                .Select(d => Enum.Parse<DayOfWeek>(d))
                .ToList();

            // takvim hesap motorunu çalıştır
            CalculationResult = _calendarService.Calculate(
                StartDate.Value,
                EndDate.Value,
                selectedDaysEnum,
                SubmissionDate.Value
            );
        }

        return Page();
    }
    private async Task ValidateAndSaveEk16Async()
    {
        if (UploadedFile == null || UploadedFile.Length == 0)
        {
            UploadMessage = "Lütfen bir dosya seçiniz.";
            IsUploadSuccess = false;
            return;
        }

        if (UploadedFile.Length > MaxFileSize)
        {
            UploadMessage = "Dosya boyutu 20 MB sınırını aşamaz.";
            IsUploadSuccess = false;
            return;
        }

        string extension = Path
            .GetExtension(UploadedFile.FileName)
            .ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            UploadMessage = "Geçersiz dosya formatı. Lütfen DOC, DOCX, PDF, JPG, JPEG veya PNG yükleyiniz.";
            IsUploadSuccess = false;
            return;
        }

        string uploadFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "UploadedDocuments"
        );

        Directory.CreateDirectory(uploadFolder);

        string safeFileName = $"{Guid.NewGuid()}{extension}";
        string filePath = Path.Combine(uploadFolder, safeFileName);

        try
        {
            using (var fileStream = new FileStream(filePath, FileMode.CreateNew))
            {
                await UploadedFile.CopyToAsync(fileStream);
            }

            UploadMessage = "Dosyanız ve başvurunuz başarıyla kaydedildi.";
            IsUploadSuccess = true;
        }
        catch (IOException)
        {
            UploadMessage = "Dosya kaydedilirken bir hata oluştu.";
            IsUploadSuccess = false;
        }
        catch (UnauthorizedAccessException)
        {
            UploadMessage = "Dosya kaydedilemedi. Klasöre erişim izni bulunmuyor.";
            IsUploadSuccess = false;
        }
    }
}